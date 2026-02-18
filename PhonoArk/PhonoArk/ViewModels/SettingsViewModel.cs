using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PhonoArk.Models;
using PhonoArk.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace PhonoArk.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly SettingsService _settingsService;
    private readonly AudioService _audioService;
    private readonly LocalizationService _localizationService;

    public sealed class LanguageOption
    {
        public string Code { get; init; } = string.Empty;
        public string DisplayName { get; init; } = string.Empty;
    }

    public sealed class AccentOption
    {
        public Accent Value { get; init; }
        public string DisplayName { get; init; } = string.Empty;
    }

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

    [ObservableProperty]
    private ObservableCollection<LanguageOption> _languageOptions = new();

    [ObservableProperty]
    private LanguageOption? _selectedLanguageOption;

    [ObservableProperty]
    private ObservableCollection<AccentOption> _accentOptions = new();

    [ObservableProperty]
    private AccentOption? _selectedAccentOption;

    public SettingsViewModel(SettingsService settingsService, AudioService audioService, LocalizationService localizationService)
    {
        _settingsService = settingsService;
        _audioService = audioService;
        _localizationService = localizationService;

        LanguageOptions = new ObservableCollection<LanguageOption>
        {
            new() { Code = "zh-CN", DisplayName = "简体中文" },
            new() { Code = "en-US", DisplayName = "English" }
        };

        AccentOptions = new ObservableCollection<AccentOption>
        {
            new() { Value = Accent.GenAm, DisplayName = _localizationService.GetString("AccentGenAm") },
            new() { Value = Accent.RP, DisplayName = _localizationService.GetString("AccentRp") }
        };

        SelectedLanguageOption = FindLanguageOption(_localizationService.CurrentCultureName);
        SelectedAccentOption = FindAccentOption(SelectedAccent);
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

        SelectedLanguageOption = FindLanguageOption(_localizationService.CurrentCultureName);
        SelectedAccentOption = FindAccentOption(SelectedAccent);
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

        if (SelectedLanguageOption != null)
        {
            _localizationService.SetCulture(SelectedLanguageOption.Code);
        }
    }

    partial void OnVolumeChanged(double value)
    {
        _audioService.Volume = value;
    }

    partial void OnSelectedAccentChanged(Accent value)
    {
        _audioService.CurrentAccent = value;

        if (SelectedAccentOption?.Value != value)
        {
            SelectedAccentOption = FindAccentOption(value);
        }
    }

    partial void OnSelectedAccentOptionChanged(AccentOption? value)
    {
        if (value != null && SelectedAccent != value.Value)
        {
            SelectedAccent = value.Value;
        }
    }

    partial void OnSelectedLanguageOptionChanged(LanguageOption? value)
    {
        if (value != null)
        {
            _localizationService.SetCulture(value.Code);
        }
    }

    private LanguageOption? FindLanguageOption(string cultureCode)
    {
        foreach (var option in LanguageOptions)
        {
            if (option.Code.Equals(cultureCode, System.StringComparison.OrdinalIgnoreCase))
            {
                return option;
            }
        }

        return LanguageOptions.Count > 0 ? LanguageOptions[0] : null;
    }

    private AccentOption? FindAccentOption(Accent accent)
    {
        foreach (var option in AccentOptions)
        {
            if (option.Value == accent)
            {
                return option;
            }
        }

        return AccentOptions.Count > 0 ? AccentOptions[0] : null;
    }
}
