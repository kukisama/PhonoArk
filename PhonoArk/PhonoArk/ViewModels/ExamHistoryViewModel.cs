using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PhonoArk.Models;
using PhonoArk.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace PhonoArk.ViewModels;

public partial class ExamHistoryViewModel : ViewModelBase
{
    public sealed class ExamAttemptDisplayItem
    {
        public int ExamResultId { get; init; }
        public int QuestionOrder { get; init; }
        public string PhonemeSymbol { get; init; } = string.Empty;
        public string UserAnswer { get; init; } = string.Empty;
        public string CorrectAnswer { get; init; } = string.Empty;
        public bool IsCorrect { get; init; }
        public bool IsWrong => !IsCorrect;
           public string ResultSymbol => IsCorrect ? "✓" : "✗";
           public string ResultText { get; init; } = string.Empty;
        public string CardText => $"{QuestionOrder} {PhonemeSymbol} {ResultSymbol}";
        public string CardToolTip => $"题目 {QuestionOrder}\n音标: {PhonemeSymbol}\n你的答案: {UserAnswer}\n正确答案: {CorrectAnswer}\n结果: {ResultText}";
        public string CardBackground => IsCorrect ? "#1D8A3A" : "#C62828";
        public string CardBorderBrush => IsCorrect ? "#1D8A3A" : "#C62828";
        public string CardForeground => "White";
    }

    public sealed class ExamSessionDisplayItem
    {
        public string SessionTitle { get; init; } = string.Empty;
        public int TotalCount { get; init; }
        public int CorrectCount { get; init; }
        public int WrongCount { get; init; }
        public ObservableCollection<ExamAttemptDisplayItem> Cards { get; init; } = new();
    }

    private readonly ExamHistoryService _examHistoryService;
        private readonly LocalizationService _localizationService;
    private readonly List<ExamQuestionAttempt> _allAttempts = new();

    [ObservableProperty]
    private ObservableCollection<ExamSessionDisplayItem> _sessions = new();

    [ObservableProperty]
    private ObservableCollection<ExamHistoryService.PhonemeErrorStat> _errorStats = new();

    [ObservableProperty]
    private double _averageScore;

    [ObservableProperty]
    private bool _showWrongOnly;

    [ObservableProperty]
    private int _totalAttempts;

    [ObservableProperty]
    private int _wrongAttempts;

    [ObservableProperty]
    private double _wrongRate;

    [ObservableProperty]
    private bool _hasErrorStats;

    [ObservableProperty]
    private bool _hasAttemptItems;

    [ObservableProperty]
    private bool _isErrorStatsExpanded;

    [ObservableProperty]
    private ExamAttemptDisplayItem? _selectedAttempt;

    [ObservableProperty]
    private bool _showAttemptDetails;

    public ExamHistoryViewModel(ExamHistoryService examHistoryService, LocalizationService localizationService)
    {
        _examHistoryService = examHistoryService;
        _localizationService = localizationService;
        _examHistoryService.HistoryChanged += OnHistoryChanged;
        _localizationService.PropertyChanged += (_, _) =>
        {
            BuildSessionCards();
        };
    }

    public async Task LoadHistoryAsync()
    {
        AverageScore = await _examHistoryService.GetAverageScoreAsync();

        var attempts = await _examHistoryService.GetAllQuestionAttemptsAsync();
        _allAttempts.Clear();
        _allAttempts.AddRange(attempts);

        ErrorStats = new ObservableCollection<ExamHistoryService.PhonemeErrorStat>(
            await _examHistoryService.GetPhonemeErrorStatsAsync());
        HasErrorStats = ErrorStats.Count > 0;

        UpdateAttemptMetrics();
        BuildSessionCards();
    }

    [RelayCommand]
    private async Task ClearHistoryAsync()
    {
        await _examHistoryService.ClearAllResultsAsync();
        await LoadHistoryAsync();
    }

    partial void OnShowWrongOnlyChanged(bool value)
    {
        BuildSessionCards();
    }

    private void UpdateAttemptMetrics()
    {
        TotalAttempts = _allAttempts.Count;
        WrongAttempts = _allAttempts.Count(a => !a.IsCorrect);
        WrongRate = TotalAttempts > 0 ? (double)WrongAttempts / TotalAttempts * 100 : 0;
    }

    [RelayCommand]
    private void OpenAttemptDetails(ExamAttemptDisplayItem? item)
    {
        if (item == null)
        {
            return;
        }

        SelectedAttempt = item;
        ShowAttemptDetails = true;
    }

    [RelayCommand]
    private void CloseAttemptDetails()
    {
        ShowAttemptDetails = false;
    }

    private void BuildSessionCards()
    {
        var filtered = ShowWrongOnly
            ? _allAttempts.Where(a => !a.IsCorrect)
            : _allAttempts.AsEnumerable();

        var attemptCards = filtered
            .Select(a => new ExamAttemptDisplayItem
            {
                ExamResultId = a.ExamResultId,
                QuestionOrder = a.QuestionOrder,
                PhonemeSymbol = a.PhonemeSymbol,
                UserAnswer = string.IsNullOrWhiteSpace(a.UserWord) ? "-" : $"{a.UserWord} {a.UserIpa}",
                CorrectAnswer = $"{a.CorrectWord} {a.CorrectIpa}",
                IsCorrect = a.IsCorrect,
                ResultText = _localizationService.GetString(a.IsCorrect ? "ResultCorrect" : "ResultWrong")
            })
            .ToList();

        var grouped = attemptCards
            .GroupBy(a => a.ExamResultId)
            .OrderByDescending(group => group.Key)
            .Select((group, index) => new ExamSessionDisplayItem
            {
                SessionTitle = _localizationService.Format("ExamSessionTitleTemplate", index + 1),
                TotalCount = group.Count(),
                CorrectCount = group.Count(item => item.IsCorrect),
                WrongCount = group.Count(item => !item.IsCorrect),
                Cards = new ObservableCollection<ExamAttemptDisplayItem>(group.OrderBy(item => item.QuestionOrder))
            });

        Sessions = new ObservableCollection<ExamSessionDisplayItem>(grouped);

        HasAttemptItems = attemptCards.Count > 0;

        if (!HasAttemptItems)
        {
            ShowAttemptDetails = false;
            SelectedAttempt = null;
        }
    }

    private void OnHistoryChanged()
    {
        _ = LoadHistoryAsync();
    }
}
