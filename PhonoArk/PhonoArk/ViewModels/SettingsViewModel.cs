using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PhonoArk.Models;
using PhonoArk.Services;
using System.Threading.Tasks;

namespace PhonoArk.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly SettingsService _settingsService;
    private readonly AudioService _audioService;

    [ObservableProperty]
    private Accent _selectedAccent;

    [ObservableProperty]
    private double _volume;

    [ObservableProperty]
    private int _examQuestionCount;

    [ObservableProperty]
    private bool _darkMode;

    [ObservableProperty]
    private bool _remindersEnabled;

    public SettingsViewModel(SettingsService settingsService, AudioService audioService)
    {
        _settingsService = settingsService;
        _audioService = audioService;
    }

    public async Task LoadSettingsAsync()
    {
        var settings = await _settingsService.GetSettingsAsync();
        SelectedAccent = settings.DefaultAccent;
        Volume = settings.Volume;
        ExamQuestionCount = settings.ExamQuestionCount;
        DarkMode = settings.DarkMode;
        RemindersEnabled = settings.RemindersEnabled;

        _audioService.CurrentAccent = SelectedAccent;
        _audioService.Volume = Volume;
    }

    [RelayCommand]
    private async Task SaveSettingsAsync()
    {
        var settings = new AppSettings
        {
            DefaultAccent = SelectedAccent,
            Volume = Volume,
            ExamQuestionCount = ExamQuestionCount,
            DarkMode = DarkMode,
            RemindersEnabled = RemindersEnabled
        };

        await _settingsService.UpdateSettingsAsync(settings);

        _audioService.CurrentAccent = SelectedAccent;
        _audioService.Volume = Volume;
    }

    partial void OnVolumeChanged(double value)
    {
        _audioService.Volume = value;
    }

    partial void OnSelectedAccentChanged(Accent value)
    {
        _audioService.CurrentAccent = value;
    }
}
