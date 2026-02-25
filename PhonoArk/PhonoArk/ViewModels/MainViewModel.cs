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

    public void NavigateToIpaChart()
    {
        IpaChartViewModel.PrepareForDisplay();
        SwitchTo(IpaChartViewModel);
    }
    public void NavigateToExam() => SwitchTo(ExamViewModel);
    public void NavigateToHistory() => SwitchTo(ExamHistoryViewModel);
    public void NavigateToFavorites() => SwitchTo(FavoritesViewModel);
    public void NavigateToSettings() => SwitchTo(SettingsViewModel);

    private void SwitchTo(ViewModelBase target)
    {
        if (ReferenceEquals(CurrentView, target))
        {
            return;
        }

        if (ReferenceEquals(CurrentView, ExamHistoryViewModel))
        {
            ExamHistoryViewModel.OnViewDeactivated();
        }

        CurrentView = target;
    }

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
