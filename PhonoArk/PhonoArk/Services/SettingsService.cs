using Microsoft.EntityFrameworkCore;
using PhonoArk.Data;
using PhonoArk.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PhonoArk.Services;

public class SettingsService
{
    private readonly DbContextOptions<AppDbContext> _dbOptions;
    private AppSettings? _cachedSettings;
    private const string UsJennyDefaultMigrationMarkerFileName = "settings-usjenny-default-v1.marker";

    public SettingsService(DbContextOptions<AppDbContext> dbOptions)
    {
        _dbOptions = dbOptions;
    }

    private AppDbContext CreateContext() => new AppDbContext(_dbOptions);

    public async Task<AppSettings> GetSettingsAsync()
    {
        if (_cachedSettings != null)
            return _cachedSettings;

        using var context = CreateContext();
        _cachedSettings = await context.Settings.FirstOrDefaultAsync();
        
        if (_cachedSettings == null)
        {
            _cachedSettings = new AppSettings();
            context.Settings.Add(_cachedSettings);
            await context.SaveChangesAsync();
        }

        await ApplyUsJennyDefaultMigrationIfNeededAsync(_cachedSettings);

        return _cachedSettings;
    }

    public async Task UpdateSettingsAsync(AppSettings settings)
    {
        using var context = CreateContext();
        var existing = await context.Settings.FirstOrDefaultAsync();
        if (existing != null)
        {
            existing.DefaultAccent = settings.DefaultAccent;
            existing.Volume = settings.Volume;
            existing.ExamQuestionCount = settings.ExamQuestionCount;
            existing.DarkMode = settings.DarkMode;
            existing.RemindersEnabled = settings.RemindersEnabled;
            _cachedSettings = existing;
        }
        else
        {
            context.Settings.Add(settings);
            _cachedSettings = settings;
        }

        await context.SaveChangesAsync();
    }

    public void ClearCache()
    {
        _cachedSettings = null;
    }

    private async Task ApplyUsJennyDefaultMigrationIfNeededAsync(AppSettings settings)
    {
        try
        {
            var markerPath = GetMigrationMarkerPath();
            if (File.Exists(markerPath))
            {
                return;
            }

            if (settings.DefaultAccent != Accent.USJenny)
            {
                settings.DefaultAccent = Accent.USJenny;
                using var context = CreateContext();
                context.Settings.Update(settings);
                await context.SaveChangesAsync();
                _cachedSettings = settings;
            }

            var markerDir = Path.GetDirectoryName(markerPath);
            if (!string.IsNullOrWhiteSpace(markerDir) && !Directory.Exists(markerDir))
            {
                Directory.CreateDirectory(markerDir);
            }

            await File.WriteAllTextAsync(markerPath, DateTime.UtcNow.ToString("O"));
        }
        catch
        {
            // ignore migration marker errors to avoid blocking settings load
        }
    }

    private static string GetMigrationMarkerPath()
    {
        var rootDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "PhonoArk");

        return Path.Combine(rootDir, UsJennyDefaultMigrationMarkerFileName);
    }
}
