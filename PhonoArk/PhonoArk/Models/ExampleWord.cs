using CommunityToolkit.Mvvm.ComponentModel;

namespace PhonoArk.Models;

public partial class ExampleWord : ObservableObject
{
    public string Word { get; set; } = string.Empty;
    public string IpaTranscription { get; set; } = string.Empty;
    
    // Audio file paths for each accent
    public string GenAmAudioPath { get; set; } = string.Empty;
    public string RpAudioPath { get; set; } = string.Empty;

    [ObservableProperty]
    private bool _isPlaying;

    [ObservableProperty]
    private bool _isExamCorrect;

    [ObservableProperty]
    private bool _isExamWrong;

    public string ExamDisplayWord
    {
        get
        {
            if (IsExamCorrect)
            {
                return $"✓ {Word}";
            }

            if (IsExamWrong)
            {
                return $"✗ {Word}";
            }

            return Word;
        }
    }

    public bool IsExamNeutral => !IsExamCorrect && !IsExamWrong;

    public string ExamBackground => IsExamCorrect
        ? "#1D8A3A"
        : IsExamWrong
            ? "#C62828"
            : "#E7EEF8";

    public string ExamBorderBrush => IsExamCorrect
        ? "#1D8A3A"
        : IsExamWrong
            ? "#C62828"
            : "#B5CCE8";

    public string ExamForeground => IsExamNeutral ? "#111827" : "White";

    partial void OnIsExamCorrectChanged(bool value)
    {
        OnPropertyChanged(nameof(ExamDisplayWord));
        OnPropertyChanged(nameof(IsExamNeutral));
        OnPropertyChanged(nameof(ExamBackground));
        OnPropertyChanged(nameof(ExamBorderBrush));
        OnPropertyChanged(nameof(ExamForeground));
    }

    partial void OnIsExamWrongChanged(bool value)
    {
        OnPropertyChanged(nameof(ExamDisplayWord));
        OnPropertyChanged(nameof(IsExamNeutral));
        OnPropertyChanged(nameof(ExamBackground));
        OnPropertyChanged(nameof(ExamBorderBrush));
        OnPropertyChanged(nameof(ExamForeground));
    }
}

