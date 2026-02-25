using RepoOPS.Models;

namespace RepoOPS.Services;

/// <summary>
/// 任务执行器接口。
/// </summary>
public interface ITaskRunner
{
    /// <summary>执行单个任务。</summary>
    Task<TaskResult> RunAsync(TaskDefinition task, CancellationToken cancellationToken = default);

    /// <summary>按依赖顺序执行所有任务。</summary>
    Task<IReadOnlyList<TaskResult>> RunAllAsync(IReadOnlyList<TaskDefinition> tasks, CancellationToken cancellationToken = default);
}
