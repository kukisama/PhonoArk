using System.Text;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using MsSpeechCmd.Helpers;
using MsSpeechCmd.Models;

namespace MsSpeechCmd.Services;

/// <summary>
/// 基于 Azure Cognitive Services Speech SDK 的语音合成服务实现。
/// 支持 WAV/MP3 输出和 SSML 输入，内部通过 SemaphoreSlim 控制并发。
/// </summary>
public sealed class SpeechSynthesisService : ISpeechSynthesisService
{
    private readonly Microsoft.CognitiveServices.Speech.SpeechConfig _azureConfig;
    private bool _disposed;

    public SpeechSynthesisService(SpeechServiceConfig config)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(config.Key, nameof(config.Key));
        ArgumentException.ThrowIfNullOrWhiteSpace(config.Region, nameof(config.Region));

        _azureConfig = Microsoft.CognitiveServices.Speech.SpeechConfig.FromSubscription(config.Key, config.Region);
    }

    // -----------------------------------------------------------------------
    // ISpeechSynthesisService
    // -----------------------------------------------------------------------

    public async Task<SynthesisResult> SynthesizeAsync(
        SynthesisRequest request,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        string outputPath = FileNameHelper.ResolveOutputPath(request);
        string extension  = FileNameHelper.ResolveExtension(request.OutputFile);

        // 确保输出目录存在
        string? directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var localConfig = Microsoft.CognitiveServices.Speech.SpeechConfig.FromSubscription(_azureConfig.SubscriptionKey, _azureConfig.Region);

            // 设置输出格式
            localConfig.SetSpeechSynthesisOutputFormat(ResolveOutputFormat(extension));

            string voiceName = ResolveVoiceName(request);
            localConfig.SpeechSynthesisVoiceName = voiceName;

            using var audioConfig = AudioConfig.FromWavFileOutput(outputPath);
            using var synthesizer = new SpeechSynthesizer(localConfig, audioConfig);

            SpeechSynthesisResult result;

            if (!string.IsNullOrWhiteSpace(request.SsmlFile))
            {
                // SSML 文件输入
                string ssml = await File.ReadAllTextAsync(request.SsmlFile, Encoding.UTF8, cancellationToken);
                result = await synthesizer.SpeakSsmlAsync(ssml);
            }
            else
            {
                string text = request.Text ?? string.Empty;

                // 如果语速或音量非默认值，包装为 SSML
                if (Math.Abs(request.Rate - 1.0) > 0.01 || request.Volume != 100)
                {
                    string ssml = BuildSsml(text, voiceName, request.Rate, request.Volume,
                                            VoiceHelper.NormalizeLocale(request.Locale));
                    result = await synthesizer.SpeakSsmlAsync(ssml);
                }
                else
                {
                    result = await synthesizer.SpeakTextAsync(text);
                }
            }

            if (result.Reason == ResultReason.SynthesizingAudioCompleted)
            {
                // MP3 格式需要在 WAV 输出后转换（SDK 直接输出 MP3 流，此处统一使用文件输出）
                // 注：AudioConfig.FromWavFileOutput 对 MP3 格式同样有效（SDK 内部编码）
                var duration = TimeSpan.FromMilliseconds(
                    result.AudioDuration.TotalMilliseconds > 0
                        ? result.AudioDuration.TotalMilliseconds
                        : 0);

                return SynthesisResult.Ok(outputPath, duration);
            }

            var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
            return SynthesisResult.Fail(
                $"合成失败：{cancellation.Reason} — {cancellation.ErrorDetails}");
        }
        catch (OperationCanceledException)
        {
            return SynthesisResult.Fail("操作已取消");
        }
        catch (Exception ex)
        {
            return SynthesisResult.Fail($"异常：{ex.Message}");
        }
    }

    public async Task<IReadOnlyList<SynthesisResult>> SynthesizeManyAsync(
        IEnumerable<SynthesisRequest> requests,
        int maxDegreeOfParallelism = 4,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var requestList = requests.ToList();
        var results     = new SynthesisResult[requestList.Count];
        using var semaphore = new SemaphoreSlim(Math.Max(1, maxDegreeOfParallelism));

        var tasks = requestList.Select(async (req, index) =>
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                results[index] = await SynthesizeAsync(req, cancellationToken);
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);
        return results;
    }

    public async Task<IReadOnlyList<string>> GetAvailableVoicesAsync(
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        using var synthesizer = new SpeechSynthesizer(_azureConfig, null);
        var voicesResult = await synthesizer.GetVoicesAsync();

        return voicesResult.Voices
            .Where(v => v.Locale.StartsWith("en-", StringComparison.OrdinalIgnoreCase))
            .Select(v => $"{v.ShortName,-40} {v.Gender,-8} {v.Locale}")
            .OrderBy(s => s)
            .ToList();
    }

    // -----------------------------------------------------------------------
    // IAsyncDisposable
    // -----------------------------------------------------------------------

    public ValueTask DisposeAsync()
    {
        _disposed = true;
        return ValueTask.CompletedTask;
    }

    // -----------------------------------------------------------------------
    // Private helpers
    // -----------------------------------------------------------------------

    private static string ResolveVoiceName(SynthesisRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.Voice))
        {
            // 允许用户传入简短名称（如 "Jenny"）或完整名称（如 "en-US-JennyNeural"）
            var info = VoiceHelper.FindByShortName(request.Voice);
            return info?.ShortName ?? request.Voice;
        }

        return VoiceHelper.GetDefaultVoice(request.Locale);
    }

    private static SpeechSynthesisOutputFormat ResolveOutputFormat(string extension) =>
        extension switch
        {
            ".mp3" => SpeechSynthesisOutputFormat.Audio16Khz32KBitRateMonoMp3,
            _      => SpeechSynthesisOutputFormat.Riff24Khz16BitMonoPcm,
        };

    /// <summary>构建带语速/音量控制的 SSML 文档。</summary>
    private static string BuildSsml(string text, string voiceName, double rate, int volume, string locale)
    {
        string rateStr   = rate == 1.0   ? "default" : $"{rate:F2}";
        string volumeStr = volume == 100 ? "default" : $"{volume}%";

        return $"""
            <speak version="1.0" xmlns="http://www.w3.org/2001/10/synthesis" xml:lang="{locale}">
              <voice name="{voiceName}">
                <prosody rate="{rateStr}" volume="{volumeStr}">
                  {System.Security.SecurityElement.Escape(text)}
                </prosody>
              </voice>
            </speak>
            """;
    }
}
