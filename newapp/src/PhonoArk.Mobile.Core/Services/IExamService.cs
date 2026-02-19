using System.Collections.Generic;
using PhonoArk.Mobile.Core.Models;

namespace PhonoArk.Mobile.Core.Services;

/// <summary>
/// 考试服务接口
/// </summary>
public interface IExamService
{
    /// <summary>生成一组考试题目</summary>
    /// <param name="questionCount">题目数量</param>
    List<ExamQuestion> GenerateExam(int questionCount = 10);

    /// <summary>判断答案是否正确</summary>
    bool CheckAnswer(ExamQuestion question, int selectedIndex);

    /// <summary>计算考试结果</summary>
    ExamResult CalculateResult(List<AnswerRecord> records);
}
