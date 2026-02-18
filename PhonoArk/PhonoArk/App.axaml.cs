using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using PhonoArk.ViewModels;
using PhonoArk.Views;
using PhonoArk.Services;
using PhonoArk.Data;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System;

namespace PhonoArk;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Initialize database
        var dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "PhonoArk",
            "phonoark.db");
        
        var directory = Path.GetDirectoryName(dbPath);
        if (directory != null && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
        var dbContext = new AppDbContext(optionsBuilder.Options);
        
        // Ensure database is created
        dbContext.Database.EnsureCreated();

        // Initialize services
        var phonemeDataService = new PhonemeDataService();
        var audioService = new AudioService();
        var localizationService = new LocalizationService();
        var favoriteService = new FavoriteService(dbContext);
        var examService = new ExamService(phonemeDataService, favoriteService);
        var examHistoryService = new ExamHistoryService(dbContext);
        var settingsService = new SettingsService(dbContext);

        // Initialize ViewModels
        var ipaChartViewModel = new IpaChartViewModel(phonemeDataService, audioService, favoriteService);
        var examViewModel = new ExamViewModel(examService, examHistoryService, audioService, localizationService);
        var examHistoryViewModel = new ExamHistoryViewModel(examHistoryService);
        var favoritesViewModel = new FavoritesViewModel(favoriteService, phonemeDataService);
        var settingsViewModel = new SettingsViewModel(settingsService, audioService, localizationService);

        var mainViewModel = new MainViewModel(
            ipaChartViewModel,
            examViewModel,
            examHistoryViewModel,
            favoritesViewModel,
            settingsViewModel);

        // Load settings
        try
        {
            _ = settingsViewModel.LoadSettingsAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading settings: {ex.Message}");
        }

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = mainViewModel
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}