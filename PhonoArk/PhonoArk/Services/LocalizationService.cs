using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Runtime.CompilerServices;

namespace PhonoArk.Services;

public class LocalizationService : INotifyPropertyChanged
{
    private static readonly HashSet<string> SupportedCultures = new(StringComparer.OrdinalIgnoreCase)
    {
        "en-US",
        "zh-CN"
    };

    private readonly ResourceManager _resourceManager = new("PhonoArk.Resources.Strings", typeof(LocalizationService).Assembly);
    private CultureInfo _currentCulture;
    private readonly string _settingsFilePath;

    public LocalizationService()
    {
        var appDataDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "PhonoArk");
        Directory.CreateDirectory(appDataDirectory);
        _settingsFilePath = Path.Combine(appDataDirectory, "language.txt");

        _currentCulture = ResolveInitialCulture();
        ApplyCulture(_currentCulture);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string CurrentCultureName => _currentCulture.Name;

    public IReadOnlyList<string> AvailableCultures => new[] { "en-US", "zh-CN" };

    public string this[string key] => GetString(key);

    public string GetString(string key)
    {
        return _resourceManager.GetString(key, _currentCulture) ?? key;
    }

    public string Format(string key, params object[] args)
    {
        var template = GetString(key);
        return string.Format(_currentCulture, template, args);
    }

    public void SetCulture(string cultureName)
    {
        if (string.IsNullOrWhiteSpace(cultureName))
        {
            return;
        }

        var normalized = NormalizeCulture(cultureName);
        if (string.Equals(_currentCulture.Name, normalized, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        _currentCulture = new CultureInfo(normalized);
        ApplyCulture(_currentCulture);
        PersistCulture(_currentCulture.Name);

        OnPropertyChanged(nameof(CurrentCultureName));
        OnPropertyChanged("Item[]");
    }

    private static CultureInfo ResolveInitialCulture()
    {
        var persisted = TryReadPersistedCulture();
        if (!string.IsNullOrWhiteSpace(persisted))
        {
            var persistedNormalized = NormalizeCulture(persisted);
            return new CultureInfo(persistedNormalized);
        }

        var ui = CultureInfo.CurrentUICulture;
        var normalized = NormalizeCulture(ui.Name);
        return new CultureInfo(normalized);
    }

    private static string NormalizeCulture(string cultureName)
    {
        if (cultureName.StartsWith("zh", StringComparison.OrdinalIgnoreCase))
        {
            return "zh-CN";
        }

        return SupportedCultures.Contains(cultureName) ? cultureName : "en-US";
    }

    private static void ApplyCulture(CultureInfo culture)
    {
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
    }

    private static string? TryReadPersistedCulture()
    {
        try
        {
            var appDataDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "PhonoArk");
            var filePath = Path.Combine(appDataDirectory, "language.txt");

            if (!File.Exists(filePath))
            {
                return null;
            }

            return File.ReadAllText(filePath).Trim();
        }
        catch
        {
            return null;
        }
    }

    private void PersistCulture(string cultureName)
    {
        try
        {
            File.WriteAllText(_settingsFilePath, cultureName);
        }
        catch
        {
            // Ignore persistence errors to avoid blocking UI flows.
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
