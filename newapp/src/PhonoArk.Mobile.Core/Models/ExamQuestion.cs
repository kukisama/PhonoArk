using System.Collections.Generic;

namespace PhonoArk.Mobile.Core.Models;

/// <summary>
/// 考试题目
/// </summary>
public class ExamQuestion
{
    /// <summary>题目类型</summary>
    public ExamQuestionType QuestionType { get; set; }

    /// <summary>题干文本（单词或音标）</summary>
    public string Prompt { get; set; } = string.Empty;

    /// <summary>选项列表（4 个选项）</summary>
    public List<string> Options { get; set; } = new();

    /// <summary>正确答案的索引</summary>
    public int CorrectIndex { get; set; }

    /// <summary>关联的音标符号</summary>
    public string PhonemeSymbol { get; set; } = string.Empty;
}
