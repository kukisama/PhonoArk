using Microsoft.EntityFrameworkCore;
using PhonoArk.Data;
using PhonoArk.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PhonoArk.Services;

public class SettingsService
{
    private readonly AppDbContext _context;
    private AppSettings? _cachedSettings;
    private const string UsJennyDefaultMigrationMarkerFileName = "settings-usjenny-default-v1.marker";

    public SettingsService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AppSettings> GetSettingsAsync()
    {
        if (_cachedSettings != null)
            return _cachedSettings;

        _cachedSettings = await _context.Settings.FirstOrDefaultAsync();
        
        if (_cachedSettings == null)
        {
            _cachedSettings = new AppSettings();
            _context.Settings.Add(_cachedSettings);
            await _context.SaveChangesAsync();
        }

        await ApplyUsJennyDefaultMigrationIfNeededAsync(_cachedSettings);

        return _cachedSettings;
    }

    public async Task UpdateSettingsAsync(AppSettings settings)
    {
        var existing = await _context.Settings.FirstOrDefaultAsync();
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
            _context.Settings.Add(settings);
            _cachedSettings = settings;
        }

        await _context.SaveChangesAsync();
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
                await _context.SaveChangesAsync();
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
