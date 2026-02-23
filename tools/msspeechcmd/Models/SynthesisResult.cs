namespace MsSpeechCmd.Models;

/// <summary>单次合成任务的执行结果。</summary>
public sealed class SynthesisResult
{
    /// <summary>是否成功。</summary>
    public bool Success { get; init; }

    /// <summary>输出文件完整路径（成功时有效）。</summary>
    public string? OutputFilePath { get; init; }

    /// <summary>音频时长（成功时有效）。</summary>
    public TimeSpan Duration { get; init; }

    /// <summary>错误信息（失败时有效）。</summary>
    public string? ErrorMessage { get; init; }

    public static SynthesisResult Ok(string path, TimeSpan duration) =>
        new() { Success = true, OutputFilePath = path, Duration = duration };

    public static SynthesisResult Fail(string error) =>
        new() { Success = false, ErrorMessage = error };
}
