using System.Collections.Generic;

namespace PhonoArk.Mobile.Core.Models;

/// <summary>
/// 考试结果
/// </summary>
public class ExamResult
{
    /// <summary>总题数</summary>
    public int TotalQuestions { get; set; }

    /// <summary>正确数</summary>
    public int CorrectCount { get; set; }

    /// <summary>得分百分比</summary>
    public double ScorePercentage =>
        TotalQuestions > 0 ? (double)CorrectCount / TotalQuestions * 100 : 0;

    /// <summary>答题记录</summary>
    public List<AnswerRecord> AnswerRecords { get; set; } = new();
}

/// <summary>
/// 单题答题记录
/// </summary>
public class AnswerRecord
{
    /// <summary>题目</summary>
    public ExamQuestion Question { get; set; } = null!;

    /// <summary>用户选择的索引</summary>
    public int SelectedIndex { get; set; }

    /// <summary>是否正确</summary>
    public bool IsCorrect => SelectedIndex == Question.CorrectIndex;
}
