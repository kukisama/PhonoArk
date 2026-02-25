using System.CommandLine;
using System.Text.Json;
using RepoOPS.Helpers;
using RepoOPS.Models;
using RepoOPS.Services;

// ═══════════════════════════════════════════════════════════════════════════
//  RepoOPS — 仓库脚本任务执行器
// ═══════════════════════════════════════════════════════════════════════════

var rootCommand = new RootCommand("RepoOPS — 仓库脚本任务执行器 (repoops)");

// ── 全局选项 ─────────────────────────────────────────────────────────────
var configOption = new Option<FileInfo?>(
    name: "--config",
    description: "任务配置文件路径（默认读取当前目录下的 tasks.json）");
configOption.AddAlias("-c");

rootCommand.AddGlobalOption(configOption);

// ═══════════════════════════════════════════════════════════════════════════
//  子命令：list（列出所有任务）
// ═══════════════════════════════════════════════════════════════════════════
var listCommand = new Command("list", "列出配置文件中定义的所有任务");

listCommand.SetHandler((context) =>
{
    var configFile = context.ParseResult.GetValueForOption(configOption);
    var config = LoadAndValidateConfig(configFile);
    if (config == null)
    {
        context.ExitCode = 1;
        return;
    }

    Console.WriteLine($"配置描述：{config.Description}");
    Console.WriteLine($"\n{"任务名称",-20} {"命令",-30} {"描述"}");
    Console.WriteLine(new string('─', 80));

    foreach (var task in config.Tasks)
    {
        string cmd = $"{task.Command} {task.Arguments}".Trim();
        if (cmd.Length > 28) cmd = cmd[..25] + "...";
        Console.WriteLine($"{task.Name,-20} {cmd,-30} {task.Description}");

        if (task.DependsOn.Count > 0)
            Console.WriteLine($"{"",20} └─ 依赖：{string.Join(", ", task.DependsOn)}");
    }

    Console.WriteLine($"\n共 {config.Tasks.Count} 个任务。");
});

rootCommand.AddCommand(listCommand);

// ═══════════════════════════════════════════════════════════════════════════
//  子命令：run（运行指定任务）
// ═══════════════════════════════════════════════════════════════════════════
var runCommand = new Command("run", "运行指定名称的任务（包括其依赖任务）");

var taskNameArg = new Argument<string>(
    name: "task-name",
    description: "要运行的任务名称");
runCommand.AddArgument(taskNameArg);

var verboseOption = new Option<bool>(
    name: "--verbose",
    description: "显示任务的标准输出和标准错误",
    getDefaultValue: () => false);
verboseOption.AddAlias("-V");
runCommand.AddOption(verboseOption);

runCommand.SetHandler(async (context) =>
{
    var configFile = context.ParseResult.GetValueForOption(configOption);
    var taskName = context.ParseResult.GetValueForArgument(taskNameArg);
    var verbose = context.ParseResult.GetValueForOption(verboseOption);
    var ct = context.GetCancellationToken();

    var config = LoadAndValidateConfig(configFile);
    if (config == null)
    {
        context.ExitCode = 1;
        return;
    }

    // 解析任务及其依赖链
    var tasksToRun = ResolveDependencyChain(config.Tasks, taskName);
    if (tasksToRun == null)
    {
        Console.Error.WriteLine($"错误：未找到任务 [{taskName}]。");
        context.ExitCode = 1;
        return;
    }

    string basePath = ResolveBasePath(configFile);
    var runner = new TaskRunner(basePath);

    Console.WriteLine($"准备执行 {tasksToRun.Count} 个任务...\n");

    var results = await runner.RunAllAsync(tasksToRun, ct);
    PrintResults(results, verbose);

    if (results.Any(r => !r.Success))
        context.ExitCode = 1;
});

rootCommand.AddCommand(runCommand);

// ═══════════════════════════════════════════════════════════════════════════
//  子命令：run-all（运行全部任务）
// ═══════════════════════════════════════════════════════════════════════════
var runAllCommand = new Command("run-all", "按依赖顺序运行所有任务");

var runAllVerboseOption = new Option<bool>(
    name: "--verbose",
    description: "显示任务的标准输出和标准错误",
    getDefaultValue: () => false);
runAllVerboseOption.AddAlias("-V");
runAllCommand.AddOption(runAllVerboseOption);

runAllCommand.SetHandler(async (context) =>
{
    var configFile = context.ParseResult.GetValueForOption(configOption);
    var verbose = context.ParseResult.GetValueForOption(runAllVerboseOption);
    var ct = context.GetCancellationToken();

    var config = LoadAndValidateConfig(configFile);
    if (config == null)
    {
        context.ExitCode = 1;
        return;
    }

    string basePath = ResolveBasePath(configFile);
    var runner = new TaskRunner(basePath);

    Console.WriteLine($"准备执行全部 {config.Tasks.Count} 个任务...\n");

    var results = await runner.RunAllAsync(config.Tasks, ct);
    PrintResults(results, verbose);

    if (results.Any(r => !r.Success))
        context.ExitCode = 1;
});

rootCommand.AddCommand(runAllCommand);

// ═══════════════════════════════════════════════════════════════════════════
//  子命令：validate（验证配置文件）
// ═══════════════════════════════════════════════════════════════════════════
var validateCommand = new Command("validate", "验证任务配置文件是否合法");

validateCommand.SetHandler((context) =>
{
    var configFile = context.ParseResult.GetValueForOption(configOption);
    var config = LoadAndValidateConfig(configFile);
    if (config == null)
    {
        context.ExitCode = 1;
        return;
    }

    Console.WriteLine($"✔ 配置文件验证通过，共 {config.Tasks.Count} 个任务。");
});

rootCommand.AddCommand(validateCommand);

// ═══════════════════════════════════════════════════════════════════════════
//  子命令：init（生成示例配置文件）
// ═══════════════════════════════════════════════════════════════════════════
var initCommand = new Command("init", "在当前目录生成示例 tasks.json 配置文件");

initCommand.SetHandler((context) =>
{
    string targetPath = Path.Combine(Directory.GetCurrentDirectory(), "tasks.json");
    if (File.Exists(targetPath))
    {
        Console.Error.WriteLine($"错误：{targetPath} 已存在，请先删除或重命名。");
        context.ExitCode = 1;
        return;
    }

    var sample = new TaskConfig
    {
        Description = "示例任务配置",
        Tasks = new List<TaskDefinition>
        {
            new()
            {
                Name = "clean",
                Description = "清理构建产物",
                Command = "dotnet",
                Arguments = "clean",
                UseShell = false,
                TimeoutSeconds = 60,
            },
            new()
            {
                Name = "build",
                Description = "构建项目",
                Command = "dotnet",
                Arguments = "build --configuration Release",
                DependsOn = new List<string> { "clean" },
                UseShell = false,
                TimeoutSeconds = 120,
            },
            new()
            {
                Name = "test",
                Description = "运行测试",
                Command = "dotnet",
                Arguments = "test --no-build --configuration Release",
                DependsOn = new List<string> { "build" },
                UseShell = false,
                TimeoutSeconds = 300,
            },
        },
    };

    string json = JsonSerializer.Serialize(sample, new JsonSerializerOptions
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    });

    File.WriteAllText(targetPath, json);
    Console.WriteLine($"✔ 已生成示例配置文件：{targetPath}");
});

rootCommand.AddCommand(initCommand);

// ═══════════════════════════════════════════════════════════════════════════
//  入口
// ═══════════════════════════════════════════════════════════════════════════
return await rootCommand.InvokeAsync(args);

// ── 辅助方法 ─────────────────────────────────────────────────────────────

static TaskConfig? LoadAndValidateConfig(FileInfo? configFile)
{
    string configPath = configFile?.FullName
        ?? Path.Combine(Directory.GetCurrentDirectory(), "tasks.json");

    TaskConfig config;
    try
    {
        config = ConfigLoader.Load(configPath);
    }
    catch (FileNotFoundException)
    {
        Console.Error.WriteLine(
            "错误：未找到任务配置文件。\n" +
            "  请在当前目录创建 tasks.json，或使用 --config 参数指定配置文件路径。\n" +
            "  可使用 repoops init 生成示例配置文件。");
        return null;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"错误：无法解析配置文件：{ex.Message}");
        return null;
    }

    var errors = ConfigLoader.Validate(config);
    if (errors.Count > 0)
    {
        Console.Error.WriteLine("配置文件验证失败：");
        foreach (var err in errors)
            Console.Error.WriteLine($"  - {err}");
        return null;
    }

    return config;
}

static string ResolveBasePath(FileInfo? configFile)
{
    if (configFile != null)
        return configFile.DirectoryName ?? Directory.GetCurrentDirectory();
    return Directory.GetCurrentDirectory();
}

static List<TaskDefinition>? ResolveDependencyChain(List<TaskDefinition> allTasks, string targetName)
{
    var target = allTasks.FirstOrDefault(t =>
        t.Name.Equals(targetName, StringComparison.OrdinalIgnoreCase));

    if (target == null) return null;

    var result = new List<TaskDefinition>();
    var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    ResolveDeps(allTasks, target, result, visited);
    return result;
}

static void ResolveDeps(
    List<TaskDefinition> allTasks,
    TaskDefinition current,
    List<TaskDefinition> result,
    HashSet<string> visited)
{
    if (!visited.Add(current.Name)) return;

    foreach (var depName in current.DependsOn)
    {
        var dep = allTasks.FirstOrDefault(t =>
            t.Name.Equals(depName, StringComparison.OrdinalIgnoreCase));
        if (dep != null)
            ResolveDeps(allTasks, dep, result, visited);
    }

    result.Add(current);
}

static void PrintResults(IReadOnlyList<TaskResult> results, bool verbose)
{
    int success = 0, failure = 0;

    foreach (var r in results)
    {
        if (r.Success)
        {
            Console.WriteLine($"  ✔ [{r.TaskName}] 成功（{r.Duration.TotalSeconds:F2}s，退出码 {r.ExitCode}）");
            success++;
        }
        else
        {
            string reason = r.ErrorMessage ?? $"退出码 {r.ExitCode}";
            Console.WriteLine($"  ✘ [{r.TaskName}] 失败（{reason}）");
            failure++;
        }

        if (verbose)
        {
            if (!string.IsNullOrWhiteSpace(r.StandardOutput))
            {
                Console.WriteLine("    ── stdout ──");
                foreach (var line in r.StandardOutput.TrimEnd().Split('\n'))
                    Console.WriteLine($"    {line}");
            }
            if (!string.IsNullOrWhiteSpace(r.StandardError))
            {
                Console.WriteLine("    ── stderr ──");
                foreach (var line in r.StandardError.TrimEnd().Split('\n'))
                    Console.WriteLine($"    {line}");
            }
        }
    }

    Console.WriteLine($"\n完成：成功 {success}，失败 {failure}。");
}
