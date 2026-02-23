using PhonoArk.Mobile.Core.Models;
using PhonoArk.Mobile.Core.Services;
using PhonoArk.Mobile.Core.ViewModels;
using Xunit;

namespace PhonoArk.Mobile.Tests.ViewModels;

public class ExamViewModelTests
{
    private readonly ExamService _examService;
    private readonly ExamViewModel _viewModel;

    public ExamViewModelTests()
    {
        var phonemeService = new PhonemeService();
        _examService = new ExamService(phonemeService, new Random(42));
        _viewModel = new ExamViewModel(_examService);
    }

    [Fact]
    public void StartExam_LoadsFirstQuestion()
    {
        _viewModel.StartExam(5);

        Assert.NotNull(_viewModel.CurrentQuestion);
        Assert.Equal(1, _viewModel.CurrentNumber);
        Assert.Equal(5, _viewModel.TotalQuestions);
    }

    [Fact]
    public void StartExam_ResetsState()
    {
        _viewModel.StartExam(5);

        Assert.False(_viewModel.HasAnswered);
        Assert.Null(_viewModel.SelectedOptionIndex);
        Assert.Equal(string.Empty, _viewModel.FeedbackMessage);
    }

    [Fact]
    public void SubmitAnswer_SetsHasAnswered()
    {
        _viewModel.StartExam(5);
        _viewModel.SubmitAnswer(0);

        Assert.True(_viewModel.HasAnswered);
        Assert.NotNull(_viewModel.SelectedOptionIndex);
    }

    [Fact]
    public void SubmitAnswer_CorrectAnswer_ShowsSuccessMessage()
    {
        _viewModel.StartExam(5);
        var correctIndex = _viewModel.CurrentQuestion!.CorrectIndex;
        _viewModel.SubmitAnswer(correctIndex);

        Assert.Contains("正确", _viewModel.FeedbackMessage);
    }

    [Fact]
    public void SubmitAnswer_WrongAnswer_ShowsErrorMessage()
    {
        _viewModel.StartExam(5);
        var wrongIndex = (_viewModel.CurrentQuestion!.CorrectIndex + 1) % 4;
        _viewModel.SubmitAnswer(wrongIndex);

        Assert.Contains("错误", _viewModel.FeedbackMessage);
    }

    [Fact]
    public void SubmitAnswer_CannotSubmitTwice()
    {
        _viewModel.StartExam(5);
        _viewModel.SubmitAnswer(0);
        var firstFeedback = _viewModel.FeedbackMessage;

        _viewModel.SubmitAnswer(1); // 再次提交应该被忽略
        Assert.Equal(firstFeedback, _viewModel.FeedbackMessage);
    }

    [Fact]
    public void SubmitAnswer_InvalidIndex_IsIgnored()
    {
        _viewModel.StartExam(5);
        _viewModel.SubmitAnswer(-1);

        Assert.False(_viewModel.HasAnswered);
    }

    [Fact]
    public void SubmitAnswer_TooLargeIndex_IsIgnored()
    {
        _viewModel.StartExam(5);
        _viewModel.SubmitAnswer(99);

        Assert.False(_viewModel.HasAnswered);
    }

    [Fact]
    public void Next_BeforeAnswer_DoesNotAdvance()
    {
        _viewModel.StartExam(5);
        var firstQuestion = _viewModel.CurrentQuestion;

        _viewModel.Next();

        Assert.Same(firstQuestion, _viewModel.CurrentQuestion);
    }

    [Fact]
    public void Next_AfterAnswer_AdvancesToNextQuestion()
    {
        _viewModel.StartExam(5);
        var firstPrompt = _viewModel.CurrentQuestion!.Prompt;

        _viewModel.SubmitAnswer(0);
        _viewModel.Next();

        Assert.NotEqual(firstPrompt, _viewModel.CurrentQuestion!.Prompt);
        Assert.Equal(2, _viewModel.CurrentNumber);
    }

    [Fact]
    public void Next_AfterAnswer_ResetsState()
    {
        _viewModel.StartExam(5);
        _viewModel.SubmitAnswer(0);
        _viewModel.Next();

        Assert.False(_viewModel.HasAnswered);
        Assert.Null(_viewModel.SelectedOptionIndex);
        Assert.Equal(string.Empty, _viewModel.FeedbackMessage);
    }

    [Fact]
    public void Next_OnLastQuestion_RaisesExamCompleted()
    {
        _viewModel.StartExam(2);
        ExamResult? result = null;
        _viewModel.ExamCompleted += r => result = r;

        // 答第一题
        _viewModel.SubmitAnswer(0);
        _viewModel.Next();

        // 答第二题（最后一题）
        _viewModel.SubmitAnswer(0);
        _viewModel.Next();

        Assert.NotNull(result);
        Assert.Equal(2, result!.TotalQuestions);
    }

    [Fact]
    public void Progress_IncreasesAsTakingExam()
    {
        _viewModel.StartExam(2);
        Assert.Equal(0, _viewModel.Progress);

        _viewModel.SubmitAnswer(0);
        _viewModel.Next();
        Assert.Equal(50, _viewModel.Progress);
    }

    [Fact]
    public void IsLastQuestion_OnSecondOfTwo_ReturnsTrue()
    {
        _viewModel.StartExam(2);
        Assert.False(_viewModel.IsLastQuestion);

        _viewModel.SubmitAnswer(0);
        _viewModel.Next();
        Assert.True(_viewModel.IsLastQuestion);
    }

    [Fact]
    public void PropertyChanged_FiredForHasAnswered()
    {
        _viewModel.StartExam(5);
        var changed = new List<string>();
        _viewModel.PropertyChanged += (_, e) => changed.Add(e.PropertyName!);

        _viewModel.SubmitAnswer(0);

        Assert.Contains("HasAnswered", changed);
        Assert.Contains("SelectedOptionIndex", changed);
        Assert.Contains("FeedbackMessage", changed);
    }

    [Fact]
    public void Constructor_NullExamService_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => new ExamViewModel(null!));
    }

    [Fact]
    public void PlayCurrentWord_ForChoosePhonemeQuestion_RaisesEvent()
    {
        // Create a deterministic exam with a ChoosePhonemeForWord question
        var phonemeService = new PhonemeService();
        var examService = new ExamService(phonemeService, new Random(100));
        var vm = new ExamViewModel(examService);
        vm.StartExam(10);

        // Find a ChoosePhonemeForWord question
        while (vm.CurrentQuestion?.QuestionType != ExamQuestionType.ChoosePhonemeForWord)
        {
            vm.SubmitAnswer(0);
            vm.Next();
        }

        string? playedWord = null;
        vm.PlayWordRequested += w => playedWord = w;
        vm.PlayCurrentWord();

        Assert.NotNull(playedWord);
    }
}
