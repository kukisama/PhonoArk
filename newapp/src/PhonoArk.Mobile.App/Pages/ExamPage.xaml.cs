using PhonoArk.Mobile.Core.Models;
using PhonoArk.Mobile.Core.ViewModels;
using PhonoArk.Mobile.Core.Services;

namespace PhonoArk.Mobile.App.Pages;

public partial class ExamPage : ContentPage
{
    private readonly ExamViewModel _viewModel;
    private readonly Button[] _optionButtons;

    public ExamPage(IExamService examService)
    {
        InitializeComponent();

        _viewModel = new ExamViewModel(examService);
        _optionButtons = new[] { BtnOption0, BtnOption1, BtnOption2, BtnOption3 };

        _viewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(ExamViewModel.HasAnswered))
            {
                BtnNext.IsVisible = _viewModel.HasAnswered;
                BtnNext.Text = _viewModel.IsLastQuestion ? "查看结果" : "下一题";
            }
            if (e.PropertyName == nameof(ExamViewModel.FeedbackMessage))
            {
                LblFeedback.Text = _viewModel.FeedbackMessage;
                LblFeedback.IsVisible = !string.IsNullOrEmpty(_viewModel.FeedbackMessage);
                LblFeedback.TextColor = _viewModel.FeedbackMessage.StartsWith("✓")
                    ? (Color)Application.Current!.Resources["SuccessColor"]
                    : (Color)Application.Current!.Resources["ErrorColor"];
            }
        };

        _viewModel.ExamCompleted += async (result) =>
        {
            var navParams = new Dictionary<string, object> { { "result", result } };
            await Shell.Current.GoToAsync("examResult", navParams);
        };

        _viewModel.PlayWordRequested += async (word) =>
        {
            try
            {
                await TextToSpeech.Default.SpeakAsync(word);
            }
            catch { }
        };

        // 开始考试
        _viewModel.StartExam(10);
        UpdateUI();
    }

    private void OnOptionClicked(object? sender, EventArgs e)
    {
        if (sender is not Button btn) return;

        int index = Array.IndexOf(_optionButtons, btn);
        if (index < 0) return;

        _viewModel.SubmitAnswer(index);
        HighlightOptions();
    }

    private void OnNextClicked(object? sender, EventArgs e)
    {
        _viewModel.Next();
        UpdateUI();
    }

    private void UpdateUI()
    {
        var q = _viewModel.CurrentQuestion;
        if (q == null) return;

        LblPrompt.Text = q.Prompt;
        LblProgress.Text = $"第 {_viewModel.CurrentNumber} 题 / 共 {_viewModel.TotalQuestions} 题";
        ProgressBar.Progress = _viewModel.Progress / 100.0;

        for (int i = 0; i < _optionButtons.Length && i < q.Options.Count; i++)
        {
            _optionButtons[i].Text = q.Options[i];
            _optionButtons[i].BackgroundColor = (Color)Application.Current!.Resources["CardColor"];
            _optionButtons[i].TextColor = (Color)Application.Current!.Resources["TextPrimaryColor"];
            _optionButtons[i].IsEnabled = true;
        }

        LblFeedback.IsVisible = false;
        BtnNext.IsVisible = false;
    }

    private void HighlightOptions()
    {
        var q = _viewModel.CurrentQuestion;
        if (q == null) return;

        for (int i = 0; i < _optionButtons.Length; i++)
        {
            _optionButtons[i].IsEnabled = false;

            if (i == q.CorrectIndex)
            {
                _optionButtons[i].BackgroundColor = (Color)Application.Current!.Resources["SuccessColor"];
                _optionButtons[i].TextColor = Colors.White;
            }
            else if (i == _viewModel.SelectedOptionIndex)
            {
                _optionButtons[i].BackgroundColor = (Color)Application.Current!.Resources["ErrorColor"];
                _optionButtons[i].TextColor = Colors.White;
            }
        }
    }
}
