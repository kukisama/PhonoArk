using System.Diagnostics;
using RepoOPS.Models;

namespace RepoOPS.Services;

/// <summary>
/// 通过 Process 启动外部命令来执行任务。
/// </summary>
public sealed class TaskRunner : ITaskRunner
{
    private readonly string _basePath;

    public TaskRunner(string basePath)
    {
        _basePath = basePath;
    }

    public async Task<TaskResult> RunAsync(TaskDefinition task, CancellationToken cancellationToken = default)
    {
        string command = task.Command;
        string arguments = task.Arguments;

        // Windows shell 模式
        if (task.UseShell && OperatingSystem.IsWindows())
        {
            arguments = $"/c {command} {arguments}";
            command = "cmd.exe";
        }
        else if (task.UseShell && !OperatingSystem.IsWindows())
        {
            arguments = $"-c \"{command} {arguments}\"";
            command = "/bin/sh";
        }

        string workingDir = ResolveWorkingDirectory(task.WorkingDirectory);

        var psi = new ProcessStartInfo
        {
            FileName = command,
            Arguments = arguments,
            WorkingDirectory = workingDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        foreach (var (key, value) in task.EnvironmentVariables)
            psi.Environment[key] = value;

        var sw = Stopwatch.StartNew();
        try
        {
            using var process = new Process { StartInfo = psi };
            process.Start();

            var stdoutTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
            var stderrTask = process.StandardError.ReadToEndAsync(cancellationToken);

            if (task.TimeoutSeconds > 0)
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(TimeSpan.FromSeconds(task.TimeoutSeconds));

                try
                {
                    await process.WaitForExitAsync(cts.Token);
                }
                catch (OperationCanceledException)
                {
                    TryKillProcess(process);
                    sw.Stop();
                    return TaskResult.Fail(task.Name, $"任务超时（{task.TimeoutSeconds}秒）");
                }
            }
            else
            {
                await process.WaitForExitAsync(cancellationToken);
            }

            sw.Stop();

            string stdout = await stdoutTask;
            string stderr = await stderrTask;

            return TaskResult.Ok(task.Name, process.ExitCode, sw.Elapsed, stdout, stderr);
        }
        catch (OperationCanceledException)
        {
            sw.Stop();
            return TaskResult.Fail(task.Name, "任务被取消");
        }
        catch (Exception ex)
        {
            sw.Stop();
            return TaskResult.Fail(task.Name, $"启动进程失败：{ex.Message}");
        }
    }

    public async Task<IReadOnlyList<TaskResult>> RunAllAsync(
        IReadOnlyList<TaskDefinition> tasks,
        CancellationToken cancellationToken = default)
    {
        var results = new List<TaskResult>();
        var completed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var remaining = new List<TaskDefinition>(tasks);

        while (remaining.Count > 0)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // 找到所有依赖已满足的任务
            var ready = remaining
                .Where(t => t.DependsOn.All(d => completed.Contains(d)))
                .ToList();

            if (ready.Count == 0)
            {
                // 死锁：剩余任务的依赖无法满足
                foreach (var t in remaining)
                    results.Add(TaskResult.Fail(t.Name, $"依赖无法满足：{string.Join(", ", t.DependsOn.Where(d => !completed.Contains(d)))}"));
                break;
            }

            foreach (var task in ready)
            {
                // 检查依赖是否全部成功
                var failedDeps = task.DependsOn
                    .Where(d => results.Any(r => r.TaskName.Equals(d, StringComparison.OrdinalIgnoreCase) && !r.Success))
                    .ToList();

                if (failedDeps.Count > 0)
                {
                    results.Add(TaskResult.Fail(task.Name, $"前置任务失败：{string.Join(", ", failedDeps)}"));
                    completed.Add(task.Name);
                    remaining.Remove(task);
                    continue;
                }

                var result = await RunAsync(task, cancellationToken);
                results.Add(result);
                completed.Add(task.Name);
                remaining.Remove(task);
            }
        }

        return results;
    }

    private string ResolveWorkingDirectory(string workingDir)
    {
        if (string.IsNullOrWhiteSpace(workingDir))
            return _basePath;

        if (Path.IsPathRooted(workingDir))
            return workingDir;

        return Path.GetFullPath(Path.Combine(_basePath, workingDir));
    }

    private static void TryKillProcess(Process process)
    {
        try { process.Kill(entireProcessTree: true); }
        catch { /* 忽略 kill 失败 */ }
    }
}
