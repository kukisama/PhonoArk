using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Layout;
using PhonoArk.ViewModels;
using System;

namespace PhonoArk.Views;

public partial class MainView : UserControl
{
    private const double TabletBreakpoint = 900;
    private readonly IPageTransition _desktopTransition = new CrossFade { Duration = TimeSpan.FromMilliseconds(120) };

    public MainView()
    {
        InitializeComponent();
        AttachedToVisualTree += (_, _) => ApplyMobileLayoutIfNeeded();
        SizeChanged += (_, _) => ApplyMobileLayoutIfNeeded();
    }

    private void ApplyMobileLayoutIfNeeded()
    {
        if (!Classes.Contains("mobile"))
        {
            return;
        }

        var isTablet = Bounds.Width >= TabletBreakpoint;

        MainLayoutGrid.Margin = new Thickness(10);
        MainLayoutGrid.ColumnSpacing = 0;
        MainLayoutGrid.RowSpacing = 10;

        if (isTablet)
        {
            MainContentHost.PageTransition = _desktopTransition;

            MainLayoutGrid.ColumnDefinitions = new ColumnDefinitions("160,*");
            MainLayoutGrid.RowDefinitions = new RowDefinitions("*");

            Grid.SetColumn(NavPanelBorder, 0);
            Grid.SetRow(NavPanelBorder, 0);
            Grid.SetColumn(ContentPanelBorder, 1);
            Grid.SetRow(ContentPanelBorder, 0);

            NavPanelBorder.Padding = new Thickness(10);
            ContentPanelBorder.Padding = new Thickness(10);

            DesktopNavScroll.IsVisible = true;
            MobileBottomNavGrid.IsVisible = false;
            DesktopNavStack.Orientation = Orientation.Vertical;
            DesktopNavStack.Spacing = 6;

            BrandTitleText.IsVisible = false;
            BrandSubtitleText.IsVisible = false;
            DesktopComingSoonButton.IsVisible = false;
        }
        else
        {
            MainContentHost.PageTransition = null;

            MainLayoutGrid.ColumnDefinitions = new ColumnDefinitions("*");
            MainLayoutGrid.RowDefinitions = new RowDefinitions("*,Auto");

            Grid.SetColumn(ContentPanelBorder, 0);
            Grid.SetRow(ContentPanelBorder, 0);
            Grid.SetColumn(NavPanelBorder, 0);
            Grid.SetRow(NavPanelBorder, 1);

            ContentPanelBorder.Padding = new Thickness(8);
            NavPanelBorder.Padding = new Thickness(6);

            DesktopNavScroll.IsVisible = false;
            MobileBottomNavGrid.IsVisible = true;
        }
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