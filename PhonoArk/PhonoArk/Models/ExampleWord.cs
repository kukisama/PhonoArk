namespace PhonoArk.Models;

public class ExampleWord
{
    public string Word { get; set; } = string.Empty;
    public string IpaTranscription { get; set; } = string.Empty;
    
    // Audio file paths for each accent
    public string GenAmAudioPath { get; set; } = string.Empty;
    public string RpAudioPath { get; set; } = string.Empty;
}

