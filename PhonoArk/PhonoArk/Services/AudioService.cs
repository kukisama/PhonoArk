using PhonoArk.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Platform;

namespace PhonoArk.Services;

public class AudioService
{
    public event EventHandler<Accent>? AccentChanged;

    public sealed class VoiceDiagnosticsResult
    {
        public bool IsWindows { get; init; }
        public bool SystemSpeechAvailable { get; init; }
        public string RequestedCulture { get; init; } = string.Empty;
        public string Summary { get; init; } = string.Empty;
        public string SelectedVoice { get; init; } = string.Empty;
        public string[] Details { get; init; } = Array.Empty<string>();
    }

    private sealed class VoiceEntry
    {
        public string Name { get; init; } = string.Empty;
        public string CultureName { get; init; } = string.Empty;
        public bool Enabled { get; init; }
    }

    private Accent _currentAccent = Accent.GenAm;
    private double _volume = 0.8;
    private readonly object _speechGate = new();
    private object? _activeSynthesizer;
    private object? _activeSoundPlayer;
    private readonly ConcurrentDictionary<string, string> _assetExtractedFileCache = new(StringComparer.OrdinalIgnoreCase);
    private static Func<string, Accent, double, Task<bool>>? _platformSpeakHandler;
    private static Func<string, double, Task<bool>>? _platformPlayAudioFileHandler;
    private static Func<Task>? _platformStopHandler;
    private static Func<Accent, Task<VoiceDiagnosticsResult>>? _platformDiagnosticsHandler;

    public static void ConfigurePlatformSpeechHandlers(
        Func<string, Accent, double, Task<bool>>? speakHandler = null,
        Func<string, double, Task<bool>>? playAudioFileHandler = null,
        Func<Task>? stopHandler = null,
        Func<Accent, Task<VoiceDiagnosticsResult>>? diagnosticsHandler = null)
    {
        _platformSpeakHandler = speakHandler;
        _platformPlayAudioFileHandler = playAudioFileHandler;
        _platformStopHandler = stopHandler;
        _platformDiagnosticsHandler = diagnosticsHandler;
    }

    public Accent CurrentAccent
    {
        get => _currentAccent;
        set
        {
            if (_currentAccent == value)
            {
                return;
            }

            _currentAccent = value;
            AccentChanged?.Invoke(this, _currentAccent);
        }
    }

    public double Volume
    {
        get => _volume;
        set => _volume = Math.Clamp(value, 0.0, 1.0);
    }

    public async Task PlayPhonemeAsync(Phoneme phoneme)
    {
        if (phoneme.VoiceAudioPaths.TryGetValue(_currentAccent.ToString(), out var mappedAudioPath) &&
            await TryPlayRecordedAudioAsync(mappedAudioPath))
        {
            return;
        }

        var fallbackWord = phoneme.ExampleWords.FirstOrDefault()?.Word;
        var ttsText = string.IsNullOrWhiteSpace(fallbackWord) ? phoneme.Symbol : fallbackWord;
        await PlayAudioAsync(ttsText);
    }

    public async Task PlayWordAsync(ExampleWord word)
    {
        if (word.VoiceAudioPaths.TryGetValue(_currentAccent.ToString(), out var mappedAudioPath) &&
            await TryPlayRecordedAudioAsync(mappedAudioPath))
        {
            return;
        }

        await PlayAudioAsync(word.Word);
    }

    public void StopPlayback()
    {
        if (!OperatingSystem.IsWindows())
        {
            if (_platformStopHandler != null)
            {
                _platformStopHandler().SafeFireAndForget("PlatformStopHandler");
            }

            return;
        }

        RunOnStaThreadAsync(() =>
        {
            lock (_speechGate)
            {
                StopPlaybackCore();
                return true;
            }
        }).SafeFireAndForget("StopPlaybackSta");
    }

    public async Task<VoiceDiagnosticsResult> GetVoiceDiagnosticsAsync()
    {
        if (!OperatingSystem.IsWindows())
        {
            if (_platformDiagnosticsHandler != null)
            {
                try
                {
                    return await _platformDiagnosticsHandler(_currentAccent);
                }
                catch (Exception ex)
                {
                    return new VoiceDiagnosticsResult
                    {
                        IsWindows = false,
                        SystemSpeechAvailable = false,
                        RequestedCulture = _currentAccent == Accent.RP ? "en-GB" : "en-US",
                        Summary = $"平台语音诊断失败：{ex.Message}",
                        SelectedVoice = "",
                        Details = new[] { ex.ToString() }
                    };
                }
            }

            return new VoiceDiagnosticsResult
            {
                IsWindows = false,
                SystemSpeechAvailable = false,
                RequestedCulture = _currentAccent == Accent.RP ? "en-GB" : "en-US",
                Summary = "当前平台不是 Windows，System.Speech 不可用。",
                SelectedVoice = "",
                Details = new[] { "Platform: non-Windows" }
            };
        }

        try
        {
            return await RunOnStaThreadAsync(GetVoiceDiagnosticsCore);
        }
        catch (Exception ex)
        {
            return new VoiceDiagnosticsResult
            {
                IsWindows = true,
                SystemSpeechAvailable = false,
                RequestedCulture = _currentAccent == Accent.RP ? "en-GB" : "en-US",
                Summary = $"诊断线程执行失败：{ex.Message}",
                SelectedVoice = "",
                Details = new[] { ex.ToString() }
            };
        }
    }

    private async Task<bool> TryPlayRecordedAudioAsync(string mappedPath)
    {
        if (string.IsNullOrWhiteSpace(mappedPath))
        {
            return false;
        }

        var playablePath = ResolvePlayableAudioPath(mappedPath);
        if (string.IsNullOrWhiteSpace(playablePath) || !File.Exists(playablePath))
        {
            return false;
        }

        if (!OperatingSystem.IsWindows())
        {
            if (_platformPlayAudioFileHandler == null)
            {
                return false;
            }

            try
            {
                return await _platformPlayAudioFileHandler(playablePath, _volume);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"AudioService: platform file playback failed: {ex.Message}");
                return false;
            }
        }

        return await TryPlayWindowsWavAsync(playablePath);
    }

    private string? ResolvePlayableAudioPath(string mappedPath)
    {
        try
        {
            if (Path.IsPathRooted(mappedPath) && File.Exists(mappedPath))
            {
                return mappedPath;
            }

            var normalizedRelative = mappedPath
                .Replace('/', Path.DirectorySeparatorChar)
                .Replace('\\', Path.DirectorySeparatorChar)
                .TrimStart(Path.DirectorySeparatorChar);

            var diskPath = Path.Combine(AppContext.BaseDirectory, normalizedRelative);
            if (File.Exists(diskPath))
            {
                return diskPath;
            }

            var assetPath = normalizedRelative.Replace(Path.DirectorySeparatorChar, '/');
            var assetUri = new Uri($"avares://PhonoArk/{assetPath}");

            if (!AssetLoader.Exists(assetUri))
            {
                return null;
            }

            return _assetExtractedFileCache.GetOrAdd(assetPath, _ =>
            {
                var tempRoot = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "PhonoArk",
                    "audio-cache");

                Directory.CreateDirectory(tempRoot);

                var extension = Path.GetExtension(assetPath);
                var tempFile = Path.Combine(tempRoot, $"{Guid.NewGuid():N}{extension}");

                using var source = AssetLoader.Open(assetUri);
                using var target = File.Create(tempFile);
                source.CopyTo(target);

                return tempFile;
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"ResolvePlayableAudioPath failed: {ex.Message}");
            return null;
        }
    }

    private async Task<bool> TryPlayWindowsWavAsync(string filePath)
    {
        if (!OperatingSystem.IsWindows())
        {
            return false;
        }

        try
        {
            return await RunOnStaThreadAsync(() => TryPlayWindowsWavCore(filePath));
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"TryPlayWindowsWavAsync failed: {ex.Message}");
            return false;
        }
    }

    private bool TryPlayWindowsWavCore(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return false;
        }

        try
        {
            var soundPlayerType = Type.GetType("System.Media.SoundPlayer, System.Windows.Extensions", throwOnError: false)
                ?? Type.GetType("System.Media.SoundPlayer, System");

            if (soundPlayerType == null)
            {
                return false;
            }

            var soundPlayer = Activator.CreateInstance(soundPlayerType, filePath);
            if (soundPlayer == null)
            {
                return false;
            }

            lock (_speechGate)
            {
                StopPlaybackCore();

                soundPlayerType.GetMethod("Play", Type.EmptyTypes)?.Invoke(soundPlayer, null);
                _activeSoundPlayer = soundPlayer;
                return true;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Windows wav playback failed: {ex.Message}");
            return false;
        }
    }

    private async Task PlayAudioAsync(string ttsText)
    {
        if (string.IsNullOrWhiteSpace(ttsText))
        {
            return;
        }

        if (!OperatingSystem.IsWindows())
        {
            if (_platformSpeakHandler == null)
            {
                Debug.WriteLine("AudioService: no platform speech handler is registered for non-Windows runtime.");
                return;
            }

            try
            {
                var handled = await _platformSpeakHandler(ttsText, _currentAccent, _volume);
                if (!handled)
                {
                    Debug.WriteLine("AudioService: platform speech handler returned false.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"AudioService: platform speech handler failed: {ex.Message}");
            }

            return;
        }

        var spoken = await TrySpeakWithWindowsTtsAsync(ttsText);
        if (!spoken)
        {
            Debug.WriteLine("AudioService: System.Speech playback failed.");
        }
    }

    private async Task<bool> TrySpeakWithWindowsTtsAsync(string text)
    {
        if (!OperatingSystem.IsWindows() || string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        try
        {
            return await RunOnStaThreadAsync(() => TrySpeakWithWindowsTtsCore(text));
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"TrySpeakWithWindowsTtsAsync failed: {ex.Message}");
            return false;
        }
    }

    private static Task<T> RunOnStaThreadAsync<T>(Func<T> action)
    {
        var tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);

        var thread = new Thread(() =>
        {
            try
            {
                var result = action();
                tcs.TrySetResult(result);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"STA thread failed: {ex.Message}");
                tcs.TrySetException(ex);
            }
        });

        thread.IsBackground = true;
        if (OperatingSystem.IsWindows())
        {
#pragma warning disable CA1416
            thread.SetApartmentState(ApartmentState.STA);
#pragma warning restore CA1416
        }

        thread.Start();
        return tcs.Task;
    }

    private bool TrySpeakWithWindowsTtsCore(string text)
    {
        try
        {
            var synthesizerType = ResolveSpeechSynthesizerType();
            if (synthesizerType == null)
            {
                Debug.WriteLine("System.Speech is not available.");
                return false;
            }

            var synthesizer = Activator.CreateInstance(synthesizerType);
            if (synthesizer == null)
            {
                return false;
            }

            lock (_speechGate)
            {
                StopPlaybackCore();

                synthesizerType.GetMethod("SetOutputToDefaultAudioDevice")?.Invoke(synthesizer, null);

                var volumeProperty = synthesizerType.GetProperty("Volume", BindingFlags.Instance | BindingFlags.Public);
                var volumePercent = Math.Clamp((int)Math.Round(_volume * 100), 0, 100);
                volumeProperty?.SetValue(synthesizer, volumePercent);

                ConfigureWindowsVoiceByAccent(synthesizerType, synthesizer);

                var speakMethod = synthesizerType.GetMethod("SpeakAsync", new[] { typeof(string) });
                if (speakMethod == null)
                {
                    if (synthesizer is IDisposable disposableFallback)
                    {
                        disposableFallback.Dispose();
                    }

                    return false;
                }

                speakMethod.Invoke(synthesizer, new object[] { text });
                _activeSynthesizer = synthesizer;
                return true;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Windows TTS failed: {ex.Message}");
            return false;
        }
    }

    private static Type? ResolveSpeechSynthesizerType()
    {
        var type = Type.GetType("System.Speech.Synthesis.SpeechSynthesizer, System.Speech", throwOnError: false);
        if (type != null)
        {
            return type;
        }

        try
        {
            var assembly = Assembly.Load("System.Speech");
            return assembly.GetType("System.Speech.Synthesis.SpeechSynthesizer", throwOnError: false);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to load System.Speech assembly: {ex.Message}");
            return null;
        }
    }

    private void ConfigureWindowsVoiceByAccent(Type synthesizerType, object synthesizer)
    {
        try
        {
            if (TrySelectVoiceByInstalledCulture(synthesizerType, synthesizer))
            {
                return;
            }

            TrySelectVoiceByHints(synthesizerType, synthesizer);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Voice selection by accent failed: {ex.Message}");
        }
    }

    private bool TrySelectVoiceByInstalledCulture(Type synthesizerType, object synthesizer)
    {
        try
        {
            var targetCulture = _currentAccent == Accent.RP ? "en-GB" : "en-US";
            var targetLanguage = "en";

            var entries = GetInstalledVoiceEntries(synthesizer, synthesizerType);
            var selectVoiceByName = synthesizerType.GetMethod("SelectVoice", new[] { typeof(string) });
            if (selectVoiceByName == null)
            {
                return false;
            }

            var exact = entries.FirstOrDefault(v => v.Enabled && v.CultureName.Equals(targetCulture, StringComparison.OrdinalIgnoreCase));
            if (exact != null)
            {
                selectVoiceByName.Invoke(synthesizer, new object[] { exact.Name });
                return true;
            }

            var languageFallback = entries.FirstOrDefault(v => v.Enabled &&
                v.CultureName.StartsWith(targetLanguage, StringComparison.OrdinalIgnoreCase));
            if (languageFallback != null)
            {
                selectVoiceByName.Invoke(synthesizer, new object[] { languageFallback.Name });
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Select voice by installed culture failed: {ex.Message}");
            return false;
        }
    }

    private void TrySelectVoiceByHints(Type synthesizerType, object synthesizer)
    {
        var voiceGenderType = Type.GetType("System.Speech.Synthesis.VoiceGender, System.Speech", throwOnError: false);
        var voiceAgeType = Type.GetType("System.Speech.Synthesis.VoiceAge, System.Speech", throwOnError: false);
        var selectVoiceMethod = synthesizerType.GetMethod(
            "SelectVoiceByHints",
            new[] { voiceGenderType ?? typeof(object), voiceAgeType ?? typeof(object), typeof(int), typeof(CultureInfo) });

        if (voiceGenderType == null || voiceAgeType == null || selectVoiceMethod == null)
        {
            return;
        }

        var notSetGender = Enum.Parse(voiceGenderType, "NotSet");
        var notSetAge = Enum.Parse(voiceAgeType, "NotSet");
        var culture = _currentAccent == Accent.RP
            ? new CultureInfo("en-GB")
            : new CultureInfo("en-US");

        selectVoiceMethod.Invoke(synthesizer, new[] { notSetGender, notSetAge, 0, culture });
    }

    private void StopPlaybackCore()
    {
        if (_activeSoundPlayer != null)
        {
            try
            {
                var soundPlayerType = _activeSoundPlayer.GetType();
                soundPlayerType.GetMethod("Stop", Type.EmptyTypes)?.Invoke(_activeSoundPlayer, null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to stop wav playback: {ex.Message}");
            }
            finally
            {
                if (_activeSoundPlayer is IDisposable soundDisposable)
                {
                    soundDisposable.Dispose();
                }

                _activeSoundPlayer = null;
            }
        }

        if (_activeSynthesizer == null)
        {
            return;
        }

        try
        {
            var activeType = _activeSynthesizer.GetType();
            activeType.GetMethod("SpeakAsyncCancelAll", Type.EmptyTypes)?.Invoke(_activeSynthesizer, null);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to cancel speech: {ex.Message}");
        }
        finally
        {
            if (_activeSynthesizer is IDisposable disposable)
            {
                disposable.Dispose();
            }

            _activeSynthesizer = null;
        }
    }

    private VoiceDiagnosticsResult GetVoiceDiagnosticsCore()
    {
        var requestedCulture = _currentAccent == Accent.RP ? "en-GB" : "en-US";

        try
        {
            var synthesizerType = ResolveSpeechSynthesizerType();
            if (synthesizerType == null)
            {
                return new VoiceDiagnosticsResult
                {
                    IsWindows = true,
                    SystemSpeechAvailable = false,
                    RequestedCulture = requestedCulture,
                    Summary = "未加载到 System.Speech 程序集。",
                    SelectedVoice = "",
                    Details = new[] { "System.Speech not found" }
                };
            }

            var synthesizer = Activator.CreateInstance(synthesizerType);
            if (synthesizer == null)
            {
                return new VoiceDiagnosticsResult
                {
                    IsWindows = true,
                    SystemSpeechAvailable = true,
                    RequestedCulture = requestedCulture,
                    Summary = "SpeechSynthesizer 初始化失败。",
                    SelectedVoice = "",
                    Details = new[] { "CreateInstance returned null" }
                };
            }

            using (synthesizer as IDisposable)
            {
                var entries = GetInstalledVoiceEntries(synthesizer, synthesizerType);
                ConfigureWindowsVoiceByAccent(synthesizerType, synthesizer);

                var selectedVoiceName = "";
                try
                {
                    var voice = synthesizerType.GetProperty("Voice", BindingFlags.Instance | BindingFlags.Public)?.GetValue(synthesizer);
                    selectedVoiceName = voice?.GetType().GetProperty("Name", BindingFlags.Instance | BindingFlags.Public)?.GetValue(voice)?.ToString() ?? "";
                }
                catch
                {
                    selectedVoiceName = "";
                }

                var matches = entries.Where(v => v.CultureName.Equals(requestedCulture, StringComparison.OrdinalIgnoreCase)).ToArray();

                var summary = matches.Length > 0
                    ? $"检测到 {matches.Length} 个 {requestedCulture} 语音，当前选中：{(string.IsNullOrWhiteSpace(selectedVoiceName) ? "未知" : selectedVoiceName)}"
                    : $"未检测到 {requestedCulture} 语音，当前可能回退到其他英文语音。";

                var details = entries
                    .Select(v => $"{v.Name} ({v.CultureName}) {(v.Enabled ? "Enabled" : "Disabled")}")
                    .ToArray();

                return new VoiceDiagnosticsResult
                {
                    IsWindows = true,
                    SystemSpeechAvailable = true,
                    RequestedCulture = requestedCulture,
                    Summary = summary,
                    SelectedVoice = selectedVoiceName,
                    Details = details
                };
            }
        }
        catch (Exception ex)
        {
            return new VoiceDiagnosticsResult
            {
                IsWindows = true,
                SystemSpeechAvailable = true,
                RequestedCulture = requestedCulture,
                Summary = $"语音诊断失败：{ex.Message}",
                SelectedVoice = "",
                Details = new[] { ex.ToString() }
            };
        }
    }

    private static VoiceEntry[] GetInstalledVoiceEntries(object synthesizer, Type synthesizerType)
    {
        var getInstalledVoices = synthesizerType.GetMethod("GetInstalledVoices", Type.EmptyTypes);
        if (getInstalledVoices == null)
        {
            return Array.Empty<VoiceEntry>();
        }

        var installedVoices = getInstalledVoices.Invoke(synthesizer, null);
        if (installedVoices == null)
        {
            return Array.Empty<VoiceEntry>();
        }

        var collectionType = installedVoices.GetType();
        var countProperty = collectionType.GetProperty("Count", BindingFlags.Instance | BindingFlags.Public);
        var getItemMethod = collectionType.GetMethod("get_Item", new[] { typeof(int) })
                           ?? collectionType.GetMethod("Item", new[] { typeof(int) });

        if (countProperty == null || getItemMethod == null)
        {
            return Array.Empty<VoiceEntry>();
        }

        var count = (int)(countProperty.GetValue(installedVoices) ?? 0);
        var list = new List<VoiceEntry>(count);

        for (var i = 0; i < count; i++)
        {
            var installedVoice = getItemMethod.Invoke(installedVoices, new object[] { i });
            if (installedVoice == null)
            {
                continue;
            }

            var installedVoiceType = installedVoice.GetType();
            var enabled = installedVoiceType.GetProperty("Enabled", BindingFlags.Instance | BindingFlags.Public)?.GetValue(installedVoice) as bool? ?? false;
            var voiceInfo = installedVoiceType.GetProperty("VoiceInfo", BindingFlags.Instance | BindingFlags.Public)?.GetValue(installedVoice);
            if (voiceInfo == null)
            {
                continue;
            }

            var voiceInfoType = voiceInfo.GetType();
            var voiceName = voiceInfoType.GetProperty("Name", BindingFlags.Instance | BindingFlags.Public)?.GetValue(voiceInfo)?.ToString() ?? "Unknown";
            var culture = voiceInfoType.GetProperty("Culture", BindingFlags.Instance | BindingFlags.Public)?.GetValue(voiceInfo) as CultureInfo;

            list.Add(new VoiceEntry
            {
                Name = voiceName,
                CultureName = culture?.Name ?? "Unknown",
                Enabled = enabled
            });
        }

        return list.ToArray();
    }
}
