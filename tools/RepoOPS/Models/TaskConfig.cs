namespace RepoOPS.Models;

/// <summary>
/// 任务配置文件的根对象。
/// </summary>
public sealed class TaskConfig
{
    /// <summary>配置描述。</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>所有任务定义。</summary>
    public List<TaskDefinition> Tasks { get; set; } = new();
}
