using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia;
using Avalonia.Styling;
using PhonoArk.Models;
using PhonoArk.Services;
using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading.Tasks;

namespace PhonoArk.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly SettingsService _settingsService;
    private readonly AudioService _audioService;
    private readonly LocalizationService _localizationService;

    public string GitHubUrl { get; } = "https://github.com/kukisama/PhonoArk";

    public string AppVersion { get; } = GetAppVersion();

    private static string GetAppVersion()
    {
        var version = typeof(SettingsViewModel).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion;
        // Strip metadata after '+' (e.g. commit hash appended by SDK)
        if (version != null)
        {
            var plusIndex = version.IndexOf('+');
            if (plusIndex >= 0)
                version = version[..plusIndex];
        }
        return version ?? "dev";
    }

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

    [ObservableProperty]
    private string _voiceDiagnosticsSummary = "尚未运行诊断。";

    [ObservableProperty]
    private ObservableCollection<string> _voiceDiagnostics = new();

    [ObservableProperty]
    private string _voiceDiagnosticsPlatform = "-";

    [ObservableProperty]
    private string _voiceDiagnosticsRequestedCulture = "-";

    [ObservableProperty]
    private string _voiceDiagnosticsSelectedVoice = "-";

    [ObservableProperty]
    private int _voiceDiagnosticsCount;

    /// <summary>
    /// 平台专用 URI 打开回调。Android 端在 MainActivity 中注入原生 Intent 实现，
    /// 避免 Avalonia Launcher 在部分安卓设备上不可用导致闪退。
    /// 桌面端不需要注入，直接走 Avalonia TopLevel.Launcher 回退路径。
    /// </summary>
    public static Action<Uri>? PlatformOpenUri { get; set; }

    [RelayCommand]
    private void OpenGitHub()
    {
        _ = OpenGitHubCoreAsync();
    }

    private async Task OpenGitHubCoreAsync()
    {
        try
        {
            var uri = new System.Uri(GitHubUrl);

            // 优先使用平台原生实现（Android Intent）
            if (PlatformOpenUri is { } handler)
            {
                handler(uri);
                return;
            }

            // 回退到 Avalonia Launcher（桌面端）
            Avalonia.Controls.TopLevel? topLevel = Avalonia.Application.Current?.ApplicationLifetime switch
            {
                Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
                    => desktop.MainWindow,
                Avalonia.Controls.ApplicationLifetimes.ISingleViewApplicationLifetime { MainView: { } mv }
                    => Avalonia.Controls.TopLevel.GetTopLevel(mv),
                _ => null
            };
            if (topLevel?.Launcher is { } launcher)
                await launcher.LaunchUriAsync(uri);
        }
        catch
        {
            // Best-effort; if browser cannot be opened, silently ignore.
        }
    }

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
            new() { Value = Accent.USJenny, DisplayName = _localizationService.GetString("AccentUsJenny") },
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
        ApplyThemeVariant(DarkMode);

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
            RefreshLocalizedOptions();
        }
    }

    partial void OnDarkModeChanged(bool value)
    {
        ApplyThemeVariant(value);
    }

    [RelayCommand]
    private async Task RunVoiceDiagnosticsAsync()
    {
        var diagnostics = await _audioService.GetVoiceDiagnosticsAsync();
        VoiceDiagnosticsSummary = diagnostics.Summary;
        VoiceDiagnostics = new ObservableCollection<string>(diagnostics.Details);
        VoiceDiagnosticsPlatform = diagnostics.IsWindows ? "Windows" : "Android/Other";
        VoiceDiagnosticsRequestedCulture = string.IsNullOrWhiteSpace(diagnostics.RequestedCulture)
            ? "-"
            : diagnostics.RequestedCulture;
        VoiceDiagnosticsSelectedVoice = string.IsNullOrWhiteSpace(diagnostics.SelectedVoice)
            ? "-"
            : diagnostics.SelectedVoice;
        VoiceDiagnosticsCount = diagnostics.Details?.Length ?? 0;
    }

    private static void ApplyThemeVariant(bool darkMode)
    {
        if (Application.Current == null)
        {
            return;
        }

        Application.Current.RequestedThemeVariant = darkMode ? ThemeVariant.Dark : ThemeVariant.Light;
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

    private void RefreshLocalizedOptions()
    {
        var currentAccent = SelectedAccent;
        AccentOptions = new ObservableCollection<AccentOption>
        {
            new() { Value = Accent.USJenny, DisplayName = _localizationService.GetString("AccentUsJenny") },
            new() { Value = Accent.GenAm, DisplayName = _localizationService.GetString("AccentGenAm") },
            new() { Value = Accent.RP, DisplayName = _localizationService.GetString("AccentRp") }
        };

        SelectedAccentOption = FindAccentOption(currentAccent);
    }
}
