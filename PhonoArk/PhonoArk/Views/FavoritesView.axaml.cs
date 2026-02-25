using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia;
using System;

namespace PhonoArk.Views;

public partial class FavoritesView : UserControl
{
    private const double TabletBreakpoint = 900;

    public FavoritesView()
    {
        InitializeComponent();

        if (Application.Current?.ApplicationLifetime is ISingleViewApplicationLifetime)
        {
            Classes.Add("mobile");
        }

        Loaded += OnLoadedAsync;
        AttachedToVisualTree += (_, _) => ApplyPhoneLandscapeLayoutIfNeeded();
        SizeChanged += (_, _) => ApplyPhoneLandscapeLayoutIfNeeded();
    }

    private async void OnLoadedAsync(object? s, EventArgs e)
    {
        try
        {
            if (DataContext is ViewModels.FavoritesViewModel vm)
            {
                await vm.LoadFavoritesAsync();
            }
        }
        catch (Exception ex)
        {
            // Log error or show user-friendly message
            System.Diagnostics.Debug.WriteLine($"Error loading favorites: {ex.Message}");
        }
    }

    private void ApplyPhoneLandscapeLayoutIfNeeded()
    {
        if (!Classes.Contains("mobile"))
        {
            return;
        }

        var isPhoneLandscape = Bounds.Width > Bounds.Height && Bounds.Width < TabletBreakpoint;

        if (isPhoneLandscape)
        {
            FavoritesRootGrid.ColumnDefinitions = new ColumnDefinitions("0.95*,1.55*");
            FavoritesRootGrid.RowDefinitions = new RowDefinitions("*");
            FavoritesRootGrid.RowSpacing = 0;
            FavoritesRootGrid.Margin = new Thickness(10);

            FavoritesFilterPanel.Spacing = 8;
            FavoritesFilterPanel.Margin = new Thickness(0, 0, 0, 0);
            GroupFilterBorder.Padding = new Thickness(10, 8);
            ClearAllBorder.Padding = new Thickness(10, 8);

            FavoritesListScrollViewer.Margin = new Thickness(8, 0, 0, 0);

            Grid.SetColumn(FavoritesFilterPanel, 0);
            Grid.SetRow(FavoritesFilterPanel, 0);

            Grid.SetColumn(FavoritesListScrollViewer, 1);
            Grid.SetRow(FavoritesListScrollViewer, 0);

            Grid.SetColumn(NoFavoritesText, 1);
            Grid.SetRow(NoFavoritesText, 0);
        }
        else
        {
            FavoritesRootGrid.ColumnDefinitions = new ColumnDefinitions("*");
            FavoritesRootGrid.RowDefinitions = new RowDefinitions("Auto,*");
            FavoritesRootGrid.RowSpacing = 10;
            FavoritesRootGrid.Margin = new Thickness(20);

            FavoritesFilterPanel.Spacing = 10;
            FavoritesFilterPanel.Margin = new Thickness(0, 0, 0, 4);
            GroupFilterBorder.Padding = new Thickness(18);
            ClearAllBorder.Padding = new Thickness(18);

            FavoritesListScrollViewer.Margin = new Thickness(0);

            Grid.SetColumn(FavoritesFilterPanel, 0);
            Grid.SetRow(FavoritesFilterPanel, 0);

            Grid.SetColumn(FavoritesListScrollViewer, 0);
            Grid.SetRow(FavoritesListScrollViewer, 1);

            Grid.SetColumn(NoFavoritesText, 0);
            Grid.SetRow(NoFavoritesText, 1);
        }
    }
}
