using CommunityToolkit.Mvvm.ComponentModel;

namespace PhonoArk.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase _currentView;

    [ObservableProperty]
    private bool _isIpaChartSelected;

    [ObservableProperty]
    private bool _isExamSelected;

    [ObservableProperty]
    private bool _isFavoritesSelected;

    [ObservableProperty]
    private bool _isHistorySelected;

    [ObservableProperty]
    private bool _isSettingsSelected;

    public IpaChartViewModel IpaChartViewModel { get; }
    public ExamViewModel ExamViewModel { get; }
    public ExamHistoryViewModel ExamHistoryViewModel { get; }
    public FavoritesViewModel FavoritesViewModel { get; }
    public SettingsViewModel SettingsViewModel { get; }

    public MainViewModel(
        IpaChartViewModel ipaChartViewModel,
        ExamViewModel examViewModel,
        ExamHistoryViewModel examHistoryViewModel,
        FavoritesViewModel favoritesViewModel,
        SettingsViewModel settingsViewModel)
    {
        IpaChartViewModel = ipaChartViewModel;
        ExamViewModel = examViewModel;
        ExamHistoryViewModel = examHistoryViewModel;
        FavoritesViewModel = favoritesViewModel;
        SettingsViewModel = settingsViewModel;

        // Default to IPA Chart view
        _currentView = IpaChartViewModel;
        UpdateNavigationState();
    }

    public void NavigateToIpaChart() => CurrentView = IpaChartViewModel;
    public void NavigateToExam() => CurrentView = ExamViewModel;
    public void NavigateToHistory() => CurrentView = ExamHistoryViewModel;
    public void NavigateToFavorites() => CurrentView = FavoritesViewModel;
    public void NavigateToSettings() => CurrentView = SettingsViewModel;

    partial void OnCurrentViewChanged(ViewModelBase value)
    {
        UpdateNavigationState();
    }

    private void UpdateNavigationState()
    {
        IsIpaChartSelected = ReferenceEquals(CurrentView, IpaChartViewModel);
        IsExamSelected = ReferenceEquals(CurrentView, ExamViewModel);
        IsFavoritesSelected = ReferenceEquals(CurrentView, FavoritesViewModel);
        IsHistorySelected = ReferenceEquals(CurrentView, ExamHistoryViewModel);
        IsSettingsSelected = ReferenceEquals(CurrentView, SettingsViewModel);
    }
}
