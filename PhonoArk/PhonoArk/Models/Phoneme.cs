using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PhonoArk.Models;

public partial class Phoneme : ObservableObject
{
    public string Symbol { get; set; } = string.Empty;
    public PhonemeType Type { get; set; }
    public List<ExampleWord> ExampleWords { get; set; } = new();
    
    // Audio file paths for each accent
    public string GenAmAudioPath { get; set; } = string.Empty;
    public string RpAudioPath { get; set; } = string.Empty;
    public Dictionary<string, string> VoiceAudioPaths { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private bool _isFavorite;

    [ObservableProperty]
    private string _description = string.Empty;
}

