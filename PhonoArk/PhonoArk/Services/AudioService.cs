using PhonoArk.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace PhonoArk.Services;

public class AudioService
{
    private Accent _currentAccent = Accent.GenAm;
    private double _volume = 0.8;

    public Accent CurrentAccent
    {
        get => _currentAccent;
        set => _currentAccent = value;
    }

    public double Volume
    {
        get => _volume;
        set => _volume = Math.Clamp(value, 0.0, 1.0);
    }

    public async Task PlayPhonemeAsync(Phoneme phoneme)
    {
        string audioPath = _currentAccent == Accent.GenAm 
            ? phoneme.GenAmAudioPath 
            : phoneme.RpAudioPath;

        var fallbackWord = phoneme.ExampleWords.FirstOrDefault()?.Word;
        var ttsText = string.IsNullOrWhiteSpace(fallbackWord) ? phoneme.Symbol : fallbackWord;

        await PlayAudioAsync(audioPath, ttsText, phoneme.Symbol);
    }

    public async Task PlayWordAsync(ExampleWord word)
    {
        string audioPath = _currentAccent == Accent.GenAm 
            ? word.GenAmAudioPath 
            : word.RpAudioPath;

        await PlayAudioAsync(audioPath, word.Word, word.Word);
    }

    private async Task PlayAudioAsync(string audioPath, string ttsText, string fallbackSeed)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(audioPath) && File.Exists(audioPath))
            {
                // TODO: real audio playback implementation for file path.
                // Current behavior still provides audible feedback via fallback tone.
                Debug.WriteLine($"Audio file exists but direct playback is not implemented yet: {audioPath}");
            }
            else
            {
                Debug.WriteLine("Audio file path unavailable, using fallback tone playback.");
            }

            if (await TrySpeakWithWindowsTtsAsync(ttsText))
            {
                return;
            }

            await PlayFallbackToneAsync(fallbackSeed);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error playing audio: {ex.Message}");
            await PlayFallbackToneAsync(fallbackSeed);
        }
    }

    private async Task<bool> TrySpeakWithWindowsTtsAsync(string text)
    {
        if (!OperatingSystem.IsWindows() || string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        return await RunOnStaThreadAsync(() => TrySpeakWithWindowsTtsCore(text));
    }

    private static Task<bool> RunOnStaThreadAsync(Func<bool> action)
    {
        var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        var thread = new Thread(() =>
        {
            try
            {
                var result = action();
                tcs.TrySetResult(result);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"STA TTS thread failed: {ex.Message}");
                tcs.TrySetResult(false);
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
            var synthesizerType = Type.GetType("System.Speech.Synthesis.SpeechSynthesizer, System.Speech", throwOnError: false);
            if (synthesizerType == null)
            {
                Debug.WriteLine("System.Speech not available. TTS skipped.");
                return false;
            }

            var synthesizer = Activator.CreateInstance(synthesizerType);
            if (synthesizer is not IDisposable disposableSynthesizer)
            {
                return false;
            }

            using (disposableSynthesizer)
            {
                synthesizerType.GetMethod("SetOutputToDefaultAudioDevice")?.Invoke(synthesizer, null);

                var volumeProperty = synthesizerType.GetProperty("Volume", BindingFlags.Instance | BindingFlags.Public);
                var volumePercent = Math.Clamp((int)Math.Round(_volume * 100), 0, 100);
                volumeProperty?.SetValue(synthesizer, volumePercent);

                ConfigureWindowsVoiceByAccent(synthesizerType, synthesizer);

                var speakMethod = synthesizerType.GetMethod("Speak", new[] { typeof(string) });
                if (speakMethod == null)
                {
                    return false;
                }

                speakMethod.Invoke(synthesizer, new object[] { text });
                return true;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Windows TTS failed: {ex.Message}");
            return false;
        }
    }

    private void ConfigureWindowsVoiceByAccent(Type synthesizerType, object synthesizer)
    {
        try
        {
            var voiceGenderType = Type.GetType("System.Speech.Synthesis.VoiceGender, System.Speech", throwOnError: false);
            var voiceAgeType = Type.GetType("System.Speech.Synthesis.VoiceAge, System.Speech", throwOnError: false);
            var selectVoiceMethod = synthesizerType.GetMethod(
                "SelectVoiceByHints",
                new[] { voiceGenderType ?? typeof(object), voiceAgeType ?? typeof(object), typeof(int), typeof(System.Globalization.CultureInfo) });

            if (voiceGenderType == null || voiceAgeType == null || selectVoiceMethod == null)
            {
                return;
            }

            var notSetGender = Enum.Parse(voiceGenderType, "NotSet");
            var notSetAge = Enum.Parse(voiceAgeType, "NotSet");
            var culture = _currentAccent == Accent.RP
                ? new System.Globalization.CultureInfo("en-GB")
                : new System.Globalization.CultureInfo("en-US");

            selectVoiceMethod.Invoke(synthesizer, new[] { notSetGender, notSetAge, 0, culture });
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Voice selection by accent failed: {ex.Message}");
        }
    }

    private async Task PlayFallbackToneAsync(string seed)
    {
        if (_volume <= 0.01)
        {
            return;
        }

        var hash = string.IsNullOrWhiteSpace(seed) ? 0 : seed.GetHashCode(StringComparison.Ordinal);
        var frequency = 500 + Math.Abs(hash % 800);
        var duration = 120 + (int)(_volume * 180);

        try
        {
            if (OperatingSystem.IsWindows())
            {
#pragma warning disable CA1416
                await Task.Run(() => Console.Beep(frequency, duration));
#pragma warning restore CA1416
            }
            else
            {
                Debug.WriteLine("Fallback tone is only enabled on Windows.");
            }
        }
        catch (PlatformNotSupportedException)
        {
            Debug.WriteLine("Fallback tone is not supported on current platform.");
        }
    }

    public void StopPlayback()
    {
        // Stop any currently playing audio
        Debug.WriteLine("Stopping audio playback");
    }
}
