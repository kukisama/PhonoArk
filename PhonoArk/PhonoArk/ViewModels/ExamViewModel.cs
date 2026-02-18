using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PhonoArk.Models;
using PhonoArk.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
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
    private int _questionCount = 10;

    private DateTime _examStartTime;

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
    }

    [RelayCommand]
    private async Task StartExamAsync()
    {
        var questions = await _examService.GenerateExamAsync(QuestionCount, ExamScope);
        
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

    [RelayCommand]
    private async Task SelectAnswerAsync(ExampleWord answer)
    {
        if (CurrentQuestion == null || CurrentQuestion.IsAnswered)
            return;

        _examService.AnswerQuestion(CurrentQuestion, answer);

        if (CurrentQuestion.IsCorrect)
        {
            CorrectAnswers++;
            FeedbackMessage = _localizationService.Format("FeedbackCorrectTemplate", answer.Word, answer.IpaTranscription);
        }
        else
        {
            FeedbackMessage = _localizationService.Format(
                "FeedbackIncorrectTemplate",
                CurrentQuestion.CorrectAnswer.Word,
                CurrentQuestion.CorrectAnswer.IpaTranscription);
        }

        ShowFeedback = true;
        
        // Auto advance to next question after a delay
        await Task.Delay(2000);
        await NextQuestionCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    private async Task NextQuestionAsync()
    {
        ShowFeedback = false;

        if (CurrentQuestionIndex < Questions.Count - 1)
        {
            CurrentQuestionIndex++;
            CurrentQuestion = Questions[CurrentQuestionIndex];
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

        await _examHistoryService.SaveResultAsync(result);

        FeedbackMessage = _localizationService.Format("FeedbackExamCompletedTemplate", correct, total, result.Score);
        ShowFeedback = true;
    }
}
