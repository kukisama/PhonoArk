using Avalonia.Controls;
using System;

namespace PhonoArk.Views;

public partial class FavoritesView : UserControl
{
    public FavoritesView()
    {
        InitializeComponent();
        Loaded += OnLoadedAsync;
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
}
