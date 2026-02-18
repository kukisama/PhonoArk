using Microsoft.EntityFrameworkCore;
using PhonoArk.Data;
using PhonoArk.Models;
using System.Threading.Tasks;

namespace PhonoArk.Services;

public class SettingsService
{
    private readonly AppDbContext _context;
    private AppSettings? _cachedSettings;

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
}
