using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia;
using CommunityToolkit.Mvvm.Input;
using System;

namespace PhonoArk.Views;

public partial class ExamHistoryView : UserControl
{
    private const double TabletBreakpoint = 900;

    public ExamHistoryView()
    {
        InitializeComponent();

        if (Application.Current?.ApplicationLifetime is ISingleViewApplicationLifetime)
        {
            Classes.Add("mobile");
        }

        Loaded += OnLoadedAsync;
        AttachedToVisualTree += (_, _) => ApplyPhoneLandscapeLayoutIfNeeded();
        DetachedFromVisualTree += OnDetachedFromVisualTree;
        SizeChanged += (_, _) => ApplyPhoneLandscapeLayoutIfNeeded();
        SessionsScrollViewer.ScrollChanged += OnSessionsScrollChanged;
    }

    private async void OnLoadedAsync(object? s, EventArgs e)
    {
        try
        {
            if (DataContext is ViewModels.ExamHistoryViewModel vm)
            {
                await vm.EnsureHistoryLoadedAsync();
            }
        }
        catch (Exception ex)
        {
            // Log error or show user-friendly message
            System.Diagnostics.Debug.WriteLine($"Error loading exam history: {ex.Message}");
        }
    }

    private void OnDetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (DataContext is ViewModels.ExamHistoryViewModel vm)
        {
            vm.OnViewDeactivated();
        }
    }

    private async void OnSessionsScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        if (DataContext is not ViewModels.ExamHistoryViewModel vm)
        {
            return;
        }

        if (!vm.HasMoreSessions || vm.IsLoadingMoreSessions)
        {
            return;
        }

        var remaining = SessionsScrollViewer.Extent.Height - (SessionsScrollViewer.Offset.Y + SessionsScrollViewer.Viewport.Height);
        if (remaining > 140)
        {
            return;
        }

        if (vm.LoadMoreSessionsCommand is IAsyncRelayCommand asyncCommand)
        {
            await asyncCommand.ExecuteAsync(null);
        }
        else
        {
            vm.LoadMoreSessionsCommand.Execute(null);
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
            if (!Classes.Contains("phone-landscape"))
            {
                Classes.Add("phone-landscape");
            }

            HistoryRootGrid.ColumnDefinitions = new ColumnDefinitions("0.95*,1.55*");
            HistoryRootGrid.RowDefinitions = new RowDefinitions("Auto,Auto,*");
            HistoryRootGrid.RowSpacing = 4;
            HistoryRootGrid.Margin = new Thickness(10);

            HistoryTitleText.IsVisible = false;
            SummaryBorder.Margin = new Thickness(0, 0, 0, 2);
            SummaryBorder.Padding = new Thickness(8, 6);
            FilterBorder.Margin = new Thickness(0, 0, 0, 2);
            FilterBorder.Padding = new Thickness(8, 4);
            ErrorStatsExpander.Margin = new Thickness(0, 0, 0, 0);
            ErrorStatsExpander.IsExpanded = false;
            ErrorStatsExpander.MaxHeight = 148;
            SessionsScrollViewer.Margin = new Thickness(8, 8, 0, 0);

            // Compact fonts
            AvgScoreLabelText.FontSize = 15;
            AvgScoreValueText.FontSize = 15;

            // Left column: Summary + Filter
            Grid.SetColumn(SummaryBorder, 0);
            Grid.SetRow(SummaryBorder, 0);

            Grid.SetColumn(FilterBorder, 0);
            Grid.SetRow(FilterBorder, 1);

            // Right column: ErrorStats (top, auto) + Sessions (below, fills)
            Grid.SetColumn(ErrorStatsExpander, 1);
            Grid.SetRow(ErrorStatsExpander, 0);
            Grid.SetRowSpan(ErrorStatsExpander, 1);

            Grid.SetColumn(SessionsScrollViewer, 1);
            Grid.SetRow(SessionsScrollViewer, 1);
            Grid.SetRowSpan(SessionsScrollViewer, 2);

            Grid.SetColumn(NoHistoryText, 1);
            Grid.SetRow(NoHistoryText, 1);
            Grid.SetRowSpan(NoHistoryText, 2);

            // Hide title row
            Grid.SetColumn(HistoryTitleText, 0);
            Grid.SetRow(HistoryTitleText, 2);
        }
        else
        {
            Classes.Remove("phone-landscape");

            HistoryRootGrid.ColumnDefinitions = new ColumnDefinitions("*");
            HistoryRootGrid.RowDefinitions = new RowDefinitions("Auto,Auto,Auto,Auto,*");
            HistoryRootGrid.RowSpacing = 10;
            HistoryRootGrid.Margin = new Thickness(20);

            HistoryTitleText.IsVisible = true;
            SummaryBorder.Margin = new Thickness(0, 0, 0, 4);
            SummaryBorder.Padding = new Thickness(20);
            FilterBorder.Margin = new Thickness(0, 0, 0, 4);
            FilterBorder.Padding = new Thickness(20);
            ErrorStatsExpander.Margin = new Thickness(0, 0, 0, 0);
            ErrorStatsExpander.MaxHeight = double.PositiveInfinity;
            SessionsScrollViewer.Margin = new Thickness(0);

            // Restore fonts
            AvgScoreLabelText.FontSize = 18;
            AvgScoreValueText.FontSize = 18;
            SummaryBorder.Padding = new Thickness(20);

            Grid.SetColumn(HistoryTitleText, 0);
            Grid.SetRow(HistoryTitleText, 0);
            Grid.SetRowSpan(HistoryTitleText, 1);

            Grid.SetColumn(SummaryBorder, 0);
            Grid.SetRow(SummaryBorder, 1);

            Grid.SetColumn(FilterBorder, 0);
            Grid.SetRow(FilterBorder, 2);

            Grid.SetColumn(ErrorStatsExpander, 0);
            Grid.SetRow(ErrorStatsExpander, 3);

            Grid.SetColumn(SessionsScrollViewer, 0);
            Grid.SetRow(SessionsScrollViewer, 4);
            Grid.SetRowSpan(SessionsScrollViewer, 1);

            Grid.SetColumn(NoHistoryText, 0);
            Grid.SetRow(NoHistoryText, 4);
            Grid.SetRowSpan(NoHistoryText, 1);
        }
    }
}
