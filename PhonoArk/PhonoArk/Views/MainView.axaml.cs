using Avalonia.Controls;
using Avalonia.Interactivity;
using PhonoArk.ViewModels;

namespace PhonoArk.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    private void OnIpaChartClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
            vm.NavigateToIpaChart();
    }

    private void OnExamClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
            vm.NavigateToExam();
    }

    private void OnFavoritesClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
            vm.NavigateToFavorites();
    }

    private void OnHistoryClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
            vm.NavigateToHistory();
    }

    private void OnSettingsClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
            vm.NavigateToSettings();
    }
}