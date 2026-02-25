using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PhonoArk.Models;
using PhonoArk.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace PhonoArk.ViewModels;

public partial class ExamHistoryViewModel : ViewModelBase
{
    private const int InitialSessionPageSize = 2;
    private const int SessionChunkSize = 3;

    public sealed partial class ExamAttemptDisplayItem : ObservableObject
    {
        public int AttemptId { get; init; }
        public int ExamResultId { get; init; }
        public int QuestionOrder { get; init; }
        public string PhonemeSymbol { get; init; } = string.Empty;

        [ObservableProperty]
        private string _userAnswer = "加载中...";

        [ObservableProperty]
        private string _correctAnswer = "加载中...";

        [ObservableProperty]
        private bool _isDetailLoaded;

        public bool IsCorrect { get; init; }
        public bool IsWrong => !IsCorrect;
        public string ResultSymbol => IsCorrect ? "✓" : "✗";
        public string ResultText { get; init; } = string.Empty;
        public string CardText => $"{QuestionOrder} {PhonemeSymbol} {ResultSymbol}";
        public string CardToolTip => $"题目 {QuestionOrder}\n音标: {PhonemeSymbol}\n结果: {ResultText}";
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
    private readonly SemaphoreSlim _loadGate = new(1, 1);
    private CancellationTokenSource? _loadCts;
    private int _nextResultSkip;
    private int _sessionIndexSeed;
    private bool _isInitialized;

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

    [ObservableProperty]
    private bool _isLoadingMoreSessions;

    [ObservableProperty]
    private bool _hasMoreSessions;

    public ExamHistoryViewModel(ExamHistoryService examHistoryService, LocalizationService localizationService)
    {
        _examHistoryService = examHistoryService;
        _localizationService = localizationService;
        _examHistoryService.HistoryChanged += OnHistoryChanged;
        _localizationService.PropertyChanged += (_, _) =>
            Dispatcher.UIThread.Post(() => _ = ReloadHistoryAsync(force: true));
    }

    public async Task EnsureHistoryLoadedAsync()
    {
        if (_isInitialized)
        {
            return;
        }

        await ReloadHistoryAsync(force: true);
    }

    public async Task LoadHistoryAsync()
    {
        await ReloadHistoryAsync(force: true);
    }

    public void OnViewDeactivated()
    {
        CancelCurrentLoading();
    }

    private async Task ReloadHistoryAsync(bool force)
    {
        await _loadGate.WaitAsync();
        try
        {
            if (_isInitialized && !force)
            {
                return;
            }

            CancelCurrentLoading();
            _loadCts = new CancellationTokenSource();
            var token = _loadCts.Token;

            _nextResultSkip = 0;
            _sessionIndexSeed = 0;
            Sessions = new ObservableCollection<ExamSessionDisplayItem>();
            HasAttemptItems = false;
            HasMoreSessions = false;

            await LoadMoreSessionsCoreAsync(InitialSessionPageSize, token);

            _isInitialized = true;

            _ = RefreshSummaryAsync(token);
        }
        catch (OperationCanceledException)
        {
            // ignore cancellation
        }
        finally
        {
            _loadGate.Release();
        }
    }

    [RelayCommand]
    private async Task ClearHistoryAsync()
    {
        await _examHistoryService.ClearAllResultsAsync();
        await ReloadHistoryAsync(force: true);
    }

    partial void OnShowWrongOnlyChanged(bool value)
    {
        _ = ReloadHistoryAsync(force: true);
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

    [RelayCommand]
    private async Task LoadMoreSessionsAsync()
    {
        if (IsLoadingMoreSessions || !HasMoreSessions)
        {
            return;
        }

        if (!await _loadGate.WaitAsync(0))
        {
            return;
        }

        try
        {
            var token = _loadCts?.Token ?? CancellationToken.None;
            await LoadMoreSessionsCoreAsync(SessionChunkSize, token);
        }
        finally
        {
            _loadGate.Release();
        }
    }

    private async Task LoadMoreSessionsCoreAsync(int pageSize, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return;
        }

        IsLoadingMoreSessions = true;
        try
        {
            var resultIds = await _examHistoryService.GetExamResultIdPageAsync(_nextResultSkip, pageSize);
            token.ThrowIfCancellationRequested();

            if (resultIds.Count == 0)
            {
                HasMoreSessions = false;
                return;
            }

            _nextResultSkip += resultIds.Count;
            HasMoreSessions = resultIds.Count == pageSize;

            var attempts = await _examHistoryService.GetQuestionAttemptsSummaryByResultIdsAsync(resultIds, ShowWrongOnly);
            token.ThrowIfCancellationRequested();

            var sessionsToAdd = BuildSessionsFromAttempts(resultIds, attempts);

            foreach (var session in sessionsToAdd)
            {
                Sessions.Add(session);
            }

            HasAttemptItems = Sessions.Count > 0;
            await Task.Yield();
        }
        catch (OperationCanceledException)
        {
            // ignore cancellation
        }
        finally
        {
            IsLoadingMoreSessions = false;
        }
    }

    private List<ExamSessionDisplayItem> BuildSessionsFromAttempts(
        IReadOnlyList<int> orderedResultIds,
        List<ExamQuestionAttempt> attempts)
    {
        var attemptsByResult = attempts
            .GroupBy(a => a.ExamResultId)
            .ToDictionary(g => g.Key, g => g.OrderBy(a => a.QuestionOrder).ToList());

        var sessions = new List<ExamSessionDisplayItem>();

        foreach (var resultId in orderedResultIds)
        {
            if (!attemptsByResult.TryGetValue(resultId, out var resultAttempts) || resultAttempts.Count == 0)
            {
                continue;
            }

            var cards = resultAttempts
                .Select(a => new ExamAttemptDisplayItem
                {
                    AttemptId = a.Id,
                    ExamResultId = a.ExamResultId,
                    QuestionOrder = a.QuestionOrder,
                    PhonemeSymbol = a.PhonemeSymbol,
                    IsCorrect = a.IsCorrect,
                    ResultText = _localizationService.GetString(a.IsCorrect ? "ResultCorrect" : "ResultWrong")
                })
                .ToList();

            _sessionIndexSeed++;
            sessions.Add(new ExamSessionDisplayItem
            {
                SessionTitle = _localizationService.Format("ExamSessionTitleTemplate", _sessionIndexSeed),
                TotalCount = cards.Count,
                CorrectCount = cards.Count(item => item.IsCorrect),
                WrongCount = cards.Count(item => !item.IsCorrect),
                Cards = new ObservableCollection<ExamAttemptDisplayItem>(cards)
            });
        }

        return sessions;
    }

    private async Task RefreshSummaryAsync(CancellationToken token)
    {
        try
        {
            var averageScoreTask = _examHistoryService.GetAverageScoreAsync();
            var metricsTask = _examHistoryService.GetAttemptMetricsAsync();
            var errorStatsTask = _examHistoryService.GetPhonemeErrorStatsAsync();

            await Task.WhenAll(averageScoreTask, metricsTask, errorStatsTask);
            token.ThrowIfCancellationRequested();

            AverageScore = averageScoreTask.Result;
            TotalAttempts = metricsTask.Result.TotalAttempts;
            WrongAttempts = metricsTask.Result.WrongAttempts;
            WrongRate = TotalAttempts > 0 ? (double)WrongAttempts / TotalAttempts * 100 : 0;

            ErrorStats = new ObservableCollection<ExamHistoryService.PhonemeErrorStat>(errorStatsTask.Result);
            HasErrorStats = ErrorStats.Count > 0;

            if (!HasAttemptItems)
            {
                ShowAttemptDetails = false;
                SelectedAttempt = null;
            }
        }
        catch (OperationCanceledException)
        {
            // ignore cancellation
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"History summary refresh failed: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task LoadAttemptDetailAsync(ExamAttemptDisplayItem? item)
    {
        if (item == null || item.IsDetailLoaded)
        {
            return;
        }

        var detail = await _examHistoryService.GetQuestionAttemptDetailByIdAsync(item.AttemptId);
        if (detail == null)
        {
            item.UserAnswer = "-";
            item.CorrectAnswer = "-";
            item.IsDetailLoaded = true;
            return;
        }

        item.UserAnswer = string.IsNullOrWhiteSpace(detail.UserWord)
            ? "-"
            : $"{detail.UserWord} {detail.UserIpa}";
        item.CorrectAnswer = string.IsNullOrWhiteSpace(detail.CorrectWord)
            ? "-"
            : $"{detail.CorrectWord} {detail.CorrectIpa}";
        item.IsDetailLoaded = true;
    }

    private void OnHistoryChanged()
    {
        Dispatcher.UIThread.Post(() => _ = ReloadHistorySafeAsync());
    }

    private async Task ReloadHistorySafeAsync()
    {
        try
        {
            await ReloadHistoryAsync(force: true);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"History reload failed: {ex.Message}");
        }
    }

    private void CancelCurrentLoading()
    {
        try
        {
            _loadCts?.Cancel();
        }
        catch
        {
            // ignore cancellation race
        }
    }
}
