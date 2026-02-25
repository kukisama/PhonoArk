using System.Text.Json;
using RepoOPS.Models;

namespace RepoOPS.Helpers;

/// <summary>
/// 从 JSON 文件加载任务配置。
/// </summary>
public static class ConfigLoader
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    /// <summary>
    /// 加载任务配置文件。
    /// </summary>
    public static TaskConfig Load(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"配置文件不存在：{filePath}");

        string json = File.ReadAllText(filePath);
        var config = JsonSerializer.Deserialize<TaskConfig>(json, JsonOptions);

        return config ?? throw new InvalidOperationException("配置文件为空或格式无效。");
    }

    /// <summary>
    /// 验证配置中的任务定义是否合法。
    /// </summary>
    public static IReadOnlyList<string> Validate(TaskConfig config)
    {
        var errors = new List<string>();
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (config.Tasks.Count == 0)
        {
            errors.Add("配置文件中没有定义任何任务。");
            return errors;
        }

        foreach (var task in config.Tasks)
        {
            if (string.IsNullOrWhiteSpace(task.Name))
            {
                errors.Add("存在未命名的任务。");
                continue;
            }

            if (!names.Add(task.Name))
                errors.Add($"任务名称重复：{task.Name}");

            if (string.IsNullOrWhiteSpace(task.Command))
                errors.Add($"任务 [{task.Name}] 未指定 Command。");

            if (task.TimeoutSeconds < 0)
                errors.Add($"任务 [{task.Name}] 的 TimeoutSeconds 不能为负数。");
        }

        // 验证依赖引用
        foreach (var task in config.Tasks)
        {
            foreach (var dep in task.DependsOn)
            {
                if (!names.Contains(dep))
                    errors.Add($"任务 [{task.Name}] 依赖了不存在的任务：{dep}");

                if (dep.Equals(task.Name, StringComparison.OrdinalIgnoreCase))
                    errors.Add($"任务 [{task.Name}] 不能依赖自身。");
            }
        }

        return errors;
    }
}
