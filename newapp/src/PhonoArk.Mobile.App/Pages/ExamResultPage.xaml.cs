using PhonoArk.Mobile.Core.Models;
using PhonoArk.Mobile.Core.ViewModels;

namespace PhonoArk.Mobile.App.Pages;

[QueryProperty(nameof(ResultParam), "result")]
public partial class ExamResultPage : ContentPage
{
    public ExamResultPage()
    {
        InitializeComponent();
    }

    public ExamResult ResultParam
    {
        set
        {
            var vm = new ExamResultViewModel(value);
            LblGrade.Text = vm.GradeMessage;
            LblScore.Text = $"{vm.ScorePercentage:F0}%";
            LblScoreDetail.Text = $"答对 {vm.CorrectCount} / {vm.TotalQuestions} 题";

            var wrongRecords = vm.AnswerRecords
                .Where(r => !r.IsCorrect)
                .Select(r => new
                {
                    r.Question,
                    SelectedAnswer = r.Question.Options.ElementAtOrDefault(r.SelectedIndex) ?? "—",
                    CorrectAnswer = r.Question.Options.ElementAtOrDefault(r.Question.CorrectIndex) ?? "—"
                })
                .ToList();

            if (wrongRecords.Count == 0)
            {
                LblWrongTitle.Text = "全部答对，太棒了！🎉";
            }

            WrongList.ItemsSource = wrongRecords;
        }
    }

    private async void OnGoHomeClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//home");
    }
}
