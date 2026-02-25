namespace RepoOPS.Models;

/// <summary>
/// 单个可执行任务的定义。
/// </summary>
public sealed class TaskDefinition
{
    /// <summary>任务唯一名称。</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>任务描述。</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>要执行的命令（可执行文件路径或命令名称）。</summary>
    public string Command { get; set; } = string.Empty;

    /// <summary>命令参数。</summary>
    public string Arguments { get; set; } = string.Empty;

    /// <summary>工作目录（相对于配置文件所在目录或绝对路径）。</summary>
    public string WorkingDirectory { get; set; } = string.Empty;

    /// <summary>超时时间（秒），0 表示无超时。</summary>
    public int TimeoutSeconds { get; set; }

    /// <summary>附加的环境变量。</summary>
    public Dictionary<string, string> EnvironmentVariables { get; set; } = new();

    /// <summary>前置依赖任务名称列表（先执行依赖，再执行本任务）。</summary>
    public List<string> DependsOn { get; set; } = new();

    /// <summary>是否在 Windows 上通过 cmd /c 执行。</summary>
    public bool UseShell { get; set; }
}
