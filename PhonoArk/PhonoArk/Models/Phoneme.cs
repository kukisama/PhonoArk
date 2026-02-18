using System;
using System.Collections.Generic;

namespace PhonoArk.Models;

public class Phoneme
{
    public string Symbol { get; set; } = string.Empty;
    public PhonemeType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<ExampleWord> ExampleWords { get; set; } = new();
    
    // Audio file paths for each accent
    public string GenAmAudioPath { get; set; } = string.Empty;
    public string RpAudioPath { get; set; } = string.Empty;
}

