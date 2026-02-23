using MsSpeechCmd.Models;

namespace MsSpeechCmd.Services;

/// <summary>语音合成服务接口。</summary>
public interface ISpeechSynthesisService : IAsyncDisposable
{
    /// <summary>执行单次语音合成并将结果写入文件。</summary>
    Task<SynthesisResult> SynthesizeAsync(SynthesisRequest request, CancellationToken cancellationToken = default);

    /// <summary>并发执行多个合成请求，最大并发数由 maxDegreeOfParallelism 控制。</summary>
    Task<IReadOnlyList<SynthesisResult>> SynthesizeManyAsync(
        IEnumerable<SynthesisRequest> requests,
        int maxDegreeOfParallelism = 4,
        CancellationToken cancellationToken = default);

    /// <summary>从 API 获取区域内可用的语音列表（需要联网）。</summary>
    Task<IReadOnlyList<string>> GetAvailableVoicesAsync(CancellationToken cancellationToken = default);
}
