using PhonoArk.Mobile.Core.Models;

namespace PhonoArk.Mobile.Core.ViewModels;

/// <summary>
/// 考试结果 ViewModel
/// </summary>
public class ExamResultViewModel : BaseViewModel
{
    private readonly ExamResult _result;

    public ExamResultViewModel(ExamResult result)
    {
        _result = result;
    }

    /// <summary>总题数</summary>
    public int TotalQuestions => _result.TotalQuestions;

    /// <summary>正确数</summary>
    public int CorrectCount => _result.CorrectCount;

    /// <summary>错误数</summary>
    public int WrongCount => _result.TotalQuestions - _result.CorrectCount;

    /// <summary>得分百分比</summary>
    public double ScorePercentage => _result.ScorePercentage;

    /// <summary>得分文本</summary>
    public string ScoreText => $"{_result.CorrectCount}/{_result.TotalQuestions}";

    /// <summary>评语</summary>
    public string GradeMessage => _result.ScorePercentage switch
    {
        >= 90 => "太棒了！🎉",
        >= 70 => "不错哦！👍",
        >= 50 => "继续努力！💪",
        _ => "需要多加练习 📚"
    };

    /// <summary>答题记录</summary>
    public System.Collections.Generic.IReadOnlyList<AnswerRecord> AnswerRecords =>
        _result.AnswerRecords.AsReadOnly();
}
