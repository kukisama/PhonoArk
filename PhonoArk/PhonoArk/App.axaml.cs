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
        var audioService = new AudioService();
        var localizationService = Resources["Loc"] as LocalizationService;
        if (localizationService == null)
        {
            localizationService = new LocalizationService();
            Resources["Loc"] = localizationService;
        }
        var phonemeDataService = new PhonemeDataService(localizationService);
        localizationService.PropertyChanged += (_, _) => phonemeDataService.RefreshLocalizedDescriptions();
        var favoriteService = new FavoriteService(new AppDbContext(optionsBuilder.Options));
        var examService = new ExamService(phonemeDataService, favoriteService);
        var examHistoryService = new ExamHistoryService(new AppDbContext(optionsBuilder.Options));
        var settingsService = new SettingsService(new AppDbContext(optionsBuilder.Options));

        // Initialize ViewModels
        var ipaChartViewModel = new IpaChartViewModel(phonemeDataService, audioService, favoriteService, localizationService);
        var examViewModel = new ExamViewModel(examService, examHistoryService, audioService, localizationService);
        var examHistoryViewModel = new ExamHistoryViewModel(examHistoryService, localizationService);
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
            var mobileMainView = new MainView
            {
                DataContext = mainViewModel
            };

            mobileMainView.Classes.Add("mobile");
            singleViewPlatform.MainView = mobileMainView;
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