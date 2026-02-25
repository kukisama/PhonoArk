using Android.App;
using Android.Media;
using Android.OS;
using Android.Speech.Tts;
using Java.Util;
using PhonoArk.Models;
using PhonoArk.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PhonoArk.Android.Services;

internal static class AndroidTtsBridge
{
    private static readonly object Gate = new();

    private static TextToSpeech? _tts;
    private static MediaPlayer? _mediaPlayer;
    private static TaskCompletionSource<bool>? _initTcs;
    private static bool _configured;

    public static void Configure()
    {
        lock (Gate)
        {
            if (_configured)
            {
                return;
            }

            AudioService.ConfigurePlatformSpeechHandlers(
                speakHandler: PlayAsync,
                playAudioFileHandler: PlayAudioFileAsync,
                stopHandler: StopAsync,
                diagnosticsHandler: GetDiagnosticsAsync);

            _configured = true;
        }
    }

    private static Task<bool> EnsureInitializedAsync()
    {
        lock (Gate)
        {
            if (_initTcs is { Task.IsCompletedSuccessfully: true } && _tts != null)
            {
                return Task.FromResult(true);
            }

            if (_initTcs != null)
            {
                return _initTcs.Task;
            }

            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            _initTcs = tcs;

            try
            {
                _tts = new TextToSpeech(Application.Context, new TtsInitListener(tcs));
            }
            catch (Exception)
            {
                _initTcs = null;
                _tts?.Dispose();
                _tts = null;
                return Task.FromResult(false);
            }

            return tcs.Task;
        }
    }

    private static async Task<bool> PlayAsync(string text, Accent accent, double volume)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        if (!await EnsureInitializedAsync())
        {
            return false;
        }

        lock (Gate)
        {
            if (_tts == null)
            {
                return false;
            }

            ApplyLanguage(_tts, accent);

            var clampedVolume = (float)Math.Clamp(volume, 0.0, 1.0);
            var parameters = new Bundle();
            parameters.PutFloat(TextToSpeech.Engine.KeyParamVolume, clampedVolume);

            var utteranceId = Guid.NewGuid().ToString("N");
            var result = _tts.Speak(text, QueueMode.Flush, parameters, utteranceId);
            return result != OperationResult.Error;
        }
    }

    private static Task<bool> PlayAudioFileAsync(string filePath, double volume)
    {
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
        {
            return Task.FromResult(false);
        }

        try
        {
            lock (Gate)
            {
                _tts?.Stop();
                StopMediaPlayerCore();

                var mediaPlayer = new MediaPlayer();
                mediaPlayer.SetAudioAttributes(
                    new global::Android.Media.AudioAttributes.Builder()
                        .SetUsage(global::Android.Media.AudioUsageKind.Media)!
                        .SetContentType(global::Android.Media.AudioContentType.Music)!
                        .Build()!);
                var fileUri = global::Android.Net.Uri.FromFile(new Java.IO.File(filePath));
                if (fileUri == null)
                {
                    mediaPlayer.Dispose();
                    return Task.FromResult(false);
                }

                mediaPlayer.SetDataSource(Application.Context, fileUri);
                mediaPlayer.Prepare();

                var clampedVolume = (float)Math.Clamp(volume, 0.0, 1.0);
                mediaPlayer.SetVolume(clampedVolume, clampedVolume);
                mediaPlayer.Completion += (_, _) =>
                {
                    lock (Gate)
                    {
                        StopMediaPlayerCore();
                    }
                };

                mediaPlayer.Start();
                _mediaPlayer = mediaPlayer;
                return Task.FromResult(true);
            }
        }
        catch
        {
            lock (Gate)
            {
                StopMediaPlayerCore();
            }

            return Task.FromResult(false);
        }
    }

    private static Task StopAsync()
    {
        lock (Gate)
        {
            _tts?.Stop();
            StopMediaPlayerCore();
        }

        return Task.CompletedTask;
    }

    private static void StopMediaPlayerCore()
    {
        if (_mediaPlayer == null)
        {
            return;
        }

        try
        {
            _mediaPlayer.Stop();
        }
        catch
        {
            // ignore
        }

        try
        {
            _mediaPlayer.Release();
        }
        catch
        {
            // ignore
        }

        _mediaPlayer.Dispose();
        _mediaPlayer = null;
    }

    private static async Task<AudioService.VoiceDiagnosticsResult> GetDiagnosticsAsync(Accent accent)
    {
        var requestedCulture = accent == Accent.RP ? "en-GB" : "en-US";

        if (!await EnsureInitializedAsync())
        {
            return new AudioService.VoiceDiagnosticsResult
            {
                IsWindows = false,
                SystemSpeechAvailable = false,
                RequestedCulture = requestedCulture,
                Summary = "Android TTS 初始化失败。",
                SelectedVoice = string.Empty,
                Details = new[] { "TextToSpeech init failed" }
            };
        }

        lock (Gate)
        {
            if (_tts == null)
            {
                return new AudioService.VoiceDiagnosticsResult
                {
                    IsWindows = false,
                    SystemSpeechAvailable = false,
                    RequestedCulture = requestedCulture,
                    Summary = "Android TTS 不可用。",
                    SelectedVoice = string.Empty,
                    Details = new[] { "TextToSpeech instance is null" }
                };
            }

            var languageSupport = GetLanguageSupport(_tts, accent);

            var voices = new List<string>();
            var availableVoices = _tts.Voices;
            if (availableVoices != null)
            {
                voices.AddRange(
                    availableVoices
                        .Select(v => $"{v.Name} ({v.Locale?.ToLanguageTag() ?? "unknown"})"));
            }

            var selectedVoice = _tts.Voice?.Name ?? string.Empty;

            var summary = languageSupport >= 0
                ? $"Android TTS 可用，目标语言 {requestedCulture} 支持状态正常。"
                : $"Android TTS 可用，但目标语言 {requestedCulture} 支持有限（code={languageSupport}）。";

            if (voices.Count == 0)
            {
                voices.Add("No voices enumerated by Android TextToSpeech");
            }

            return new AudioService.VoiceDiagnosticsResult
            {
                IsWindows = false,
                SystemSpeechAvailable = true,
                RequestedCulture = requestedCulture,
                Summary = summary,
                SelectedVoice = selectedVoice,
                Details = voices.ToArray()
            };
        }
    }

    private static int ApplyLanguage(TextToSpeech tts, Accent accent)
    {
        var preferredLocale = accent == Accent.RP ? Locale.Uk : Locale.Us;
        var support = (int)tts.IsLanguageAvailable(preferredLocale);

        if (support >= 0)
        {
            tts.SetLanguage(preferredLocale);
            return support;
        }

        var fallbackLocale = Locale.English;
        var fallbackSupport = (int)tts.IsLanguageAvailable(fallbackLocale);
        if (fallbackSupport >= 0)
        {
            tts.SetLanguage(fallbackLocale);
        }

        return fallbackSupport;
    }

    private static int GetLanguageSupport(TextToSpeech tts, Accent accent)
    {
        var preferredLocale = accent == Accent.RP ? Locale.Uk : Locale.Us;
        return (int)tts.IsLanguageAvailable(preferredLocale);
    }

    private sealed class TtsInitListener(TaskCompletionSource<bool> tcs) : Java.Lang.Object, TextToSpeech.IOnInitListener
    {
        private readonly TaskCompletionSource<bool> _tcs = tcs;

        public void OnInit(OperationResult status)
        {
            _tcs.TrySetResult(status == OperationResult.Success);
        }
    }
}
