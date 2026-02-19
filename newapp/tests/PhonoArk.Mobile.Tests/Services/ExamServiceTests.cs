using PhonoArk.Mobile.Core.Models;
using PhonoArk.Mobile.Core.Services;
using Xunit;

namespace PhonoArk.Mobile.Tests.Services;

public class ExamServiceTests
{
    private readonly PhonemeService _phonemeService;
    private readonly ExamService _examService;

    public ExamServiceTests()
    {
        _phonemeService = new PhonemeService();
        _examService = new ExamService(_phonemeService);
    }

    [Fact]
    public void GenerateExam_DefaultCount_Returns10Questions()
    {
        var questions = _examService.GenerateExam();
        Assert.Equal(10, questions.Count);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(20)]
    public void GenerateExam_SpecificCount_ReturnsCorrectNumber(int count)
    {
        var questions = _examService.GenerateExam(count);
        Assert.Equal(count, questions.Count);
    }

    [Fact]
    public void GenerateExam_ZeroCount_ThrowsException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _examService.GenerateExam(0));
    }

    [Fact]
    public void GenerateExam_NegativeCount_ThrowsException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _examService.GenerateExam(-1));
    }

    [Fact]
    public void GenerateExam_QuestionsHaveFourOptions()
    {
        var questions = _examService.GenerateExam(10);
        foreach (var q in questions)
        {
            Assert.Equal(4, q.Options.Count);
        }
    }

    [Fact]
    public void GenerateExam_CorrectIndexIsValid()
    {
        var questions = _examService.GenerateExam(10);
        foreach (var q in questions)
        {
            Assert.InRange(q.CorrectIndex, 0, q.Options.Count - 1);
        }
    }

    [Fact]
    public void GenerateExam_QuestionsHavePrompt()
    {
        var questions = _examService.GenerateExam(10);
        foreach (var q in questions)
        {
            Assert.False(string.IsNullOrWhiteSpace(q.Prompt),
                "题目缺少题干");
        }
    }

    [Fact]
    public void GenerateExam_QuestionsHavePhonemeSymbol()
    {
        var questions = _examService.GenerateExam(10);
        foreach (var q in questions)
        {
            Assert.False(string.IsNullOrWhiteSpace(q.PhonemeSymbol),
                "题目缺少关联音标");
        }
    }

    [Fact]
    public void GenerateExam_HasBothQuestionTypes()
    {
        var questions = _examService.GenerateExam(10);
        Assert.Contains(questions, q => q.QuestionType == ExamQuestionType.ChoosePhonemeForWord);
        Assert.Contains(questions, q => q.QuestionType == ExamQuestionType.ChooseWordForPhoneme);
    }

    [Fact]
    public void GenerateExam_OptionsAreDistinct()
    {
        var questions = _examService.GenerateExam(10);
        foreach (var q in questions)
        {
            Assert.Equal(q.Options.Count, q.Options.Distinct().Count());
        }
    }

    [Fact]
    public void CheckAnswer_CorrectAnswer_ReturnsTrue()
    {
        var question = new ExamQuestion
        {
            Prompt = "Test",
            Options = new List<string> { "A", "B", "C", "D" },
            CorrectIndex = 2
        };

        Assert.True(_examService.CheckAnswer(question, 2));
    }

    [Fact]
    public void CheckAnswer_WrongAnswer_ReturnsFalse()
    {
        var question = new ExamQuestion
        {
            Prompt = "Test",
            Options = new List<string> { "A", "B", "C", "D" },
            CorrectIndex = 2
        };

        Assert.False(_examService.CheckAnswer(question, 0));
        Assert.False(_examService.CheckAnswer(question, 1));
        Assert.False(_examService.CheckAnswer(question, 3));
    }

    [Fact]
    public void CheckAnswer_OutOfRangeIndex_ReturnsFalse()
    {
        var question = new ExamQuestion
        {
            Prompt = "Test",
            Options = new List<string> { "A", "B", "C", "D" },
            CorrectIndex = 0
        };

        Assert.False(_examService.CheckAnswer(question, -1));
        Assert.False(_examService.CheckAnswer(question, 4));
    }

    [Fact]
    public void CheckAnswer_NullQuestion_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => _examService.CheckAnswer(null!, 0));
    }

    [Fact]
    public void CalculateResult_AllCorrect_Returns100Percent()
    {
        var records = Enumerable.Range(0, 5).Select(i => new AnswerRecord
        {
            Question = new ExamQuestion
            {
                CorrectIndex = 1,
                Options = new List<string> { "A", "B", "C", "D" }
            },
            SelectedIndex = 1
        }).ToList();

        var result = _examService.CalculateResult(records);
        Assert.Equal(5, result.TotalQuestions);
        Assert.Equal(5, result.CorrectCount);
        Assert.Equal(100.0, result.ScorePercentage);
    }

    [Fact]
    public void CalculateResult_AllWrong_Returns0Percent()
    {
        var records = Enumerable.Range(0, 5).Select(i => new AnswerRecord
        {
            Question = new ExamQuestion
            {
                CorrectIndex = 1,
                Options = new List<string> { "A", "B", "C", "D" }
            },
            SelectedIndex = 0
        }).ToList();

        var result = _examService.CalculateResult(records);
        Assert.Equal(5, result.TotalQuestions);
        Assert.Equal(0, result.CorrectCount);
        Assert.Equal(0.0, result.ScorePercentage);
    }

    [Fact]
    public void CalculateResult_MixedResults_CalculatesCorrectly()
    {
        var records = new List<AnswerRecord>
        {
            new() { Question = new ExamQuestion { CorrectIndex = 0, Options = new() { "A", "B", "C", "D" } }, SelectedIndex = 0 }, // correct
            new() { Question = new ExamQuestion { CorrectIndex = 1, Options = new() { "A", "B", "C", "D" } }, SelectedIndex = 0 }, // wrong
            new() { Question = new ExamQuestion { CorrectIndex = 2, Options = new() { "A", "B", "C", "D" } }, SelectedIndex = 2 }, // correct
            new() { Question = new ExamQuestion { CorrectIndex = 3, Options = new() { "A", "B", "C", "D" } }, SelectedIndex = 0 }, // wrong
        };

        var result = _examService.CalculateResult(records);
        Assert.Equal(4, result.TotalQuestions);
        Assert.Equal(2, result.CorrectCount);
        Assert.Equal(50.0, result.ScorePercentage);
    }

    [Fact]
    public void CalculateResult_EmptyRecords_ReturnsZero()
    {
        var result = _examService.CalculateResult(new List<AnswerRecord>());
        Assert.Equal(0, result.TotalQuestions);
        Assert.Equal(0, result.CorrectCount);
        Assert.Equal(0.0, result.ScorePercentage);
    }

    [Fact]
    public void CalculateResult_NullRecords_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => _examService.CalculateResult(null!));
    }

    [Fact]
    public void GenerateExam_WithDeterministicRandom_IsReproducible()
    {
        var service1 = new ExamService(_phonemeService, new Random(42));
        var service2 = new ExamService(_phonemeService, new Random(42));

        var exam1 = service1.GenerateExam(5);
        var exam2 = service2.GenerateExam(5);

        Assert.Equal(exam1.Count, exam2.Count);
        for (int i = 0; i < exam1.Count; i++)
        {
            Assert.Equal(exam1[i].Prompt, exam2[i].Prompt);
            Assert.Equal(exam1[i].CorrectIndex, exam2[i].CorrectIndex);
        }
    }
}
