using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PhonoArk;

/// <summary>
/// 为 fire-and-forget 异步调用提供安全包裹，避免异常被静默吞掉。
/// </summary>
public static class TaskExtensions
{
    /// <summary>
    /// 安全地 fire-and-forget 一个 Task，捕获并记录异常而非静默丢弃。
    /// </summary>
    public static async void SafeFireAndForget(this Task task, string? callerHint = null)
    {
        try
        {
            await task;
        }
        catch (OperationCanceledException)
        {
            // 取消操作属于正常流程，不需要记录
        }
        catch (Exception ex)
        {
            var hint = string.IsNullOrWhiteSpace(callerHint) ? "unknown" : callerHint;
            Debug.WriteLine($"[SafeFireAndForget] ({hint}) unhandled exception: {ex}");
        }
    }
}
