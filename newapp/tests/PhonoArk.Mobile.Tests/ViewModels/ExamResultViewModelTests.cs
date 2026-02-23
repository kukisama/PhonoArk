using PhonoArk.Mobile.Core.Models;
using PhonoArk.Mobile.Core.ViewModels;
using Xunit;

namespace PhonoArk.Mobile.Tests.ViewModels;

public class ExamResultViewModelTests
{
    private static ExamResult CreateTestResult(int total, int correct)
    {
        var records = new List<AnswerRecord>();
        for (int i = 0; i < total; i++)
        {
            records.Add(new AnswerRecord
            {
                Question = new ExamQuestion
                {
                    CorrectIndex = 0,
                    Options = new List<string> { "A", "B", "C", "D" },
                    Prompt = $"Question {i + 1}"
                },
                SelectedIndex = i < correct ? 0 : 1
            });
        }

        return new ExamResult
        {
            TotalQuestions = total,
            CorrectCount = correct,
            AnswerRecords = records
        };
    }

    [Fact]
    public void Properties_ReflectResult()
    {
        var result = CreateTestResult(10, 7);
        var vm = new ExamResultViewModel(result);

        Assert.Equal(10, vm.TotalQuestions);
        Assert.Equal(7, vm.CorrectCount);
        Assert.Equal(3, vm.WrongCount);
        Assert.Equal(70.0, vm.ScorePercentage);
    }

    [Fact]
    public void ScoreText_FormattedCorrectly()
    {
        var vm = new ExamResultViewModel(CreateTestResult(10, 8));
        Assert.Equal("8/10", vm.ScoreText);
    }

    [Theory]
    [InlineData(10, 10, "太棒了")]
    [InlineData(10, 9, "太棒了")]
    [InlineData(10, 7, "不错")]
    [InlineData(10, 5, "继续努力")]
    [InlineData(10, 3, "需要多加练习")]
    [InlineData(10, 0, "需要多加练习")]
    public void GradeMessage_MatchesScoreRange(int total, int correct, string expectedContains)
    {
        var vm = new ExamResultViewModel(CreateTestResult(total, correct));
        Assert.Contains(expectedContains, vm.GradeMessage);
    }

    [Fact]
    public void AnswerRecords_ReturnsAllRecords()
    {
        var result = CreateTestResult(5, 3);
        var vm = new ExamResultViewModel(result);

        Assert.Equal(5, vm.AnswerRecords.Count);
    }

    [Fact]
    public void PerfectScore_AllRecordsCorrect()
    {
        var vm = new ExamResultViewModel(CreateTestResult(5, 5));
        Assert.All(vm.AnswerRecords, r => Assert.True(r.IsCorrect));
    }

    [Fact]
    public void ZeroScore_AllRecordsWrong()
    {
        var vm = new ExamResultViewModel(CreateTestResult(5, 0));
        Assert.All(vm.AnswerRecords, r => Assert.False(r.IsCorrect));
    }
}
