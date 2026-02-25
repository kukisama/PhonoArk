using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia;
using Avalonia.Layout;
using PhonoArk.ViewModels;
using System.ComponentModel;

namespace PhonoArk.Views;

public partial class IpaChartView : UserControl
{
    private const double TabletBreakpoint = 900;
    private IpaChartViewModel? _viewModel;

    public IpaChartView()
    {
        InitializeComponent();

        if (Application.Current?.ApplicationLifetime is ISingleViewApplicationLifetime)
        {
            Classes.Add("mobile");
            ApplyMobileLayout();
        }

        DataContextChanged += OnDataContextChanged;
        SizeChanged += (_, _) =>
        {
            if (Classes.Contains("mobile"))
            {
                UpdateMobilePanels();
            }
        };
    }

    private void ApplyMobileLayout()
    {
        IpaMainContentGrid.ColumnDefinitions = new ColumnDefinitions("*");
        IpaMainContentGrid.RowDefinitions = new RowDefinitions("*");
        IpaMainContentGrid.ColumnSpacing = 0;
        IpaMainContentGrid.RowSpacing = 0;

        Grid.SetColumn(PhonemeListScrollViewer, 0);
        Grid.SetRow(PhonemeListScrollViewer, 0);
        PhonemeListScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
        PhonemeListScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

        Grid.SetColumn(DetailPanelBorder, 0);
        Grid.SetRow(DetailPanelBorder, 1);

        UpdateMobilePanels();
    }

    private void OnDataContextChanged(object? sender, System.EventArgs e)
    {
        if (_viewModel != null)
        {
            _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }

        _viewModel = DataContext as IpaChartViewModel;
        if (_viewModel != null)
        {
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        if (Classes.Contains("mobile"))
        {
            UpdateMobilePanels();
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (!Classes.Contains("mobile") || e.PropertyName != nameof(IpaChartViewModel.SelectedPhoneme))
        {
            return;
        }

        UpdateMobilePanels();
    }

    private void UpdateMobilePanels()
    {
        if (!Classes.Contains("mobile"))
        {
            return;
        }

        var hasSelection = _viewModel?.SelectedPhoneme != null;
        var isPhoneLandscape = Bounds.Width > Bounds.Height && Bounds.Width < TabletBreakpoint;

        if (hasSelection && isPhoneLandscape)
        {
            IpaMainContentGrid.ColumnDefinitions = new ColumnDefinitions("1.15*,0.85*");
            IpaMainContentGrid.RowDefinitions = new RowDefinitions("*");
            IpaMainContentGrid.ColumnSpacing = 10;
            IpaMainContentGrid.RowSpacing = 0;

            Grid.SetColumn(PhonemeListScrollViewer, 0);
            Grid.SetRow(PhonemeListScrollViewer, 0);
            Grid.SetColumn(DetailPanelBorder, 1);
            Grid.SetRow(DetailPanelBorder, 0);
            DetailPanelBorder.IsVisible = true;
        }
        else if (hasSelection)
        {
            IpaMainContentGrid.ColumnDefinitions = new ColumnDefinitions("*");
            IpaMainContentGrid.RowDefinitions = new RowDefinitions("*,*");
            IpaMainContentGrid.ColumnSpacing = 0;
            IpaMainContentGrid.RowSpacing = 10;
            Grid.SetColumn(PhonemeListScrollViewer, 0);
            Grid.SetRow(PhonemeListScrollViewer, 0);
            Grid.SetColumn(DetailPanelBorder, 0);
            Grid.SetRow(DetailPanelBorder, 1);
            DetailPanelBorder.IsVisible = true;
        }
        else
        {
            IpaMainContentGrid.ColumnDefinitions = new ColumnDefinitions("*");
            IpaMainContentGrid.RowDefinitions = new RowDefinitions("*");
            IpaMainContentGrid.ColumnSpacing = 0;
            IpaMainContentGrid.RowSpacing = 0;
            Grid.SetColumn(PhonemeListScrollViewer, 0);
            Grid.SetRow(PhonemeListScrollViewer, 0);
            Grid.SetColumn(DetailPanelBorder, 0);
            Grid.SetRow(DetailPanelBorder, 0);
            DetailPanelBorder.IsVisible = false;
        }
    }
}
