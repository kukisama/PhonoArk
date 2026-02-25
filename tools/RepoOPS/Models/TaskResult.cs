namespace RepoOPS.Models;

/// <summary>
/// 任务执行结果。
/// </summary>
public sealed class TaskResult
{
    public string TaskName { get; init; } = string.Empty;
    public bool Success { get; init; }
    public int ExitCode { get; init; }
    public TimeSpan Duration { get; init; }
    public string StandardOutput { get; init; } = string.Empty;
    public string StandardError { get; init; } = string.Empty;
    public string? ErrorMessage { get; init; }

    public static TaskResult Ok(string name, int exitCode, TimeSpan duration, string stdout, string stderr) =>
        new()
        {
            TaskName = name,
            Success = exitCode == 0,
            ExitCode = exitCode,
            Duration = duration,
            StandardOutput = stdout,
            StandardError = stderr,
        };

    public static TaskResult Fail(string name, string error) =>
        new()
        {
            TaskName = name,
            Success = false,
            ExitCode = -1,
            ErrorMessage = error,
        };
}
