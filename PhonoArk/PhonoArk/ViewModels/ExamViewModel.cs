using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PhonoArk.Models;
using PhonoArk.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhonoArk.ViewModels;

public partial class ExamViewModel : ViewModelBase
{
    private readonly ExamService _examService;
    private readonly ExamHistoryService _examHistoryService;
    private readonly AudioService _audioService;
    private readonly LocalizationService _localizationService;

    [ObservableProperty]
    private ObservableCollection<ExamQuestion> _questions = new();

    [ObservableProperty]
    private ExamQuestion? _currentQuestion;

    [ObservableProperty]
    private int _currentQuestionIndex;

    public int CurrentQuestionNumber => CurrentQuestionIndex + 1;

    [ObservableProperty]
    private int _totalQuestions;

    [ObservableProperty]
    private bool _isExamActive;

    [ObservableProperty]
    private bool _showFeedback;

    [ObservableProperty]
    private string _feedbackMessage = string.Empty;

    [ObservableProperty]
    private int _correctAnswers;

    [ObservableProperty]
    private string _examScope = "All";

    [ObservableProperty]
    private int _selectedExamScopeIndex;

    [ObservableProperty]
    private int _questionCount = 10;

    [ObservableProperty]
    private bool _areOptionsInteractive;

    private DateTime _examStartTime;

    private const int AutoAdvanceDelayMilliseconds = 1200;

    public ExamViewModel(
        ExamService examService,
        ExamHistoryService examHistoryService,
        AudioService audioService,
        LocalizationService localizationService)
    {
        _examService = examService;
        _examHistoryService = examHistoryService;
        _audioService = audioService;
        _localizationService = localizationService;
        SelectedExamScopeIndex = 0;
    }

    [RelayCommand]
    private async Task StartExamAsync()
    {
        List<ExamQuestion> questions;
        try
        {
            questions = await _examService.GenerateExamAsync(QuestionCount, ExamScope);
        }
        catch (Exception ex)
        {
            FeedbackMessage = ex.Message;
            ShowFeedback = true;
            IsExamActive = false;
            return;
        }
        
        if (questions.Count == 0)
        {
            FeedbackMessage = _localizationService.GetString("FeedbackNoQuestions");
            ShowFeedback = true;
            return;
        }

        Questions = new ObservableCollection<ExamQuestion>(questions);
        TotalQuestions = questions.Count;
        CurrentQuestionIndex = 0;
        CorrectAnswers = 0;
        IsExamActive = true;
        ShowFeedback = false;
        AreOptionsInteractive = true;
        _examStartTime = DateTime.UtcNow;

        CurrentQuestion = Questions[0];
        await PlayCurrentQuestionAsync();
    }

    [RelayCommand]
    private async Task PlayCurrentQuestionAsync()
    {
        if (CurrentQuestion != null)
        {
            await _audioService.PlayPhonemeAsync(CurrentQuestion.Phoneme);
        }
    }

    [RelayCommand(AllowConcurrentExecutions = true)]
    private async Task SelectAnswerAsync(ExampleWord answer)
    {
        if (CurrentQuestion == null || CurrentQuestion.IsAnswered || !AreOptionsInteractive)
            return;

        AreOptionsInteractive = false;

        ResetOptionVisualState(CurrentQuestion.Options);

        _examService.AnswerQuestion(CurrentQuestion, answer);

        if (CurrentQuestion.IsCorrect)
        {
            CorrectAnswers++;
            answer.IsExamCorrect = true;
            FeedbackMessage = _localizationService.Format("FeedbackCorrectTemplate", answer.Word, answer.IpaTranscription);
        }
        else
        {
            answer.IsExamWrong = true;
            CurrentQuestion.CorrectAnswer.IsExamCorrect = true;
            FeedbackMessage = _localizationService.Format(
                "FeedbackIncorrectTemplate",
                CurrentQuestion.CorrectAnswer.Word,
                CurrentQuestion.CorrectAnswer.IpaTranscription);
        }

        ShowFeedback = true;
        
        // Auto advance to next question after a delay
        await Task.Delay(AutoAdvanceDelayMilliseconds);
        await NextQuestionCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    private async Task NextQuestionAsync()
    {
        ShowFeedback = false;

        if (CurrentQuestionIndex + 1 < Questions.Count)
        {
            CurrentQuestionIndex++;
            CurrentQuestion = Questions[CurrentQuestionIndex];
            AreOptionsInteractive = true;
            await PlayCurrentQuestionAsync();
        }
        else
        {
            await EndExamAsync();
        }
    }

    [RelayCommand]
    private async Task EndExamAsync()
    {
        IsExamActive = false;
        AreOptionsInteractive = false;

        var duration = DateTime.UtcNow - _examStartTime;
        var (correct, total) = _examService.CalculateResults(Questions.ToList());

        var result = new ExamResult
        {
            ExamDate = _examStartTime,
            TotalQuestions = total,
            CorrectAnswers = correct,
            ExamScope = ExamScope,
            Duration = duration
        };

        await _examHistoryService.SaveResultWithAttemptsAsync(result, Questions.ToList());

        FeedbackMessage = _localizationService.Format("FeedbackExamCompletedTemplate", correct, total, result.Score);
        ShowFeedback = true;

        ResetOptionVisualState(Questions.SelectMany(q => q.Options));
    }

    private static void ResetOptionVisualState(IEnumerable<ExampleWord> options)
    {
        foreach (var option in options)
        {
            option.IsExamCorrect = false;
            option.IsExamWrong = false;
        }
    }

    partial void OnCurrentQuestionIndexChanged(int value)
    {
        OnPropertyChanged(nameof(CurrentQuestionNumber));
    }

    partial void OnSelectedExamScopeIndexChanged(int value)
    {
        ExamScope = value == 1 ? "Favorites" : "All";
    }

    partial void OnExamScopeChanged(string value)
    {
        var targetIndex = value == "Favorites" ? 1 : 0;
        if (SelectedExamScopeIndex != targetIndex)
        {
            SelectedExamScopeIndex = targetIndex;
        }
    }
}
