using CommunityToolkit.Mvvm.ComponentModel;

namespace PhonoArk.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase _currentView;

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
    }

    public void NavigateToIpaChart() => CurrentView = IpaChartViewModel;
    public void NavigateToExam() => CurrentView = ExamViewModel;
    public void NavigateToHistory() => CurrentView = ExamHistoryViewModel;
    public void NavigateToFavorites() => CurrentView = FavoritesViewModel;
    public void NavigateToSettings() => CurrentView = SettingsViewModel;
}
