using PhonoArk.Models;
using System;
using System.Diagnostics;
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

        await PlayAudioAsync(audioPath);
    }

    public async Task PlayWordAsync(ExampleWord word)
    {
        string audioPath = _currentAccent == Accent.GenAm 
            ? word.GenAmAudioPath 
            : word.RpAudioPath;

        await PlayAudioAsync(audioPath);
    }

    private async Task PlayAudioAsync(string audioPath)
    {
        // For now, we'll use a simple approach with system audio player
        // In production, you might want to use a more sophisticated audio library
        
        if (string.IsNullOrEmpty(audioPath))
        {
            Debug.WriteLine($"Audio path is empty or null");
            return;
        }

        try
        {
            // This is a placeholder. In a real app, you'd use platform-specific audio APIs
            // or a cross-platform library like NAudio, CSCore, or similar
            Debug.WriteLine($"Playing audio: {audioPath} at volume {_volume}");
            
            // Simulate audio playback delay
            await Task.Delay(500);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error playing audio: {ex.Message}");
        }
    }

    public void StopPlayback()
    {
        // Stop any currently playing audio
        Debug.WriteLine("Stopping audio playback");
    }
}
