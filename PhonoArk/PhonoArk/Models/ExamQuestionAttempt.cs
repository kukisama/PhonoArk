using System;

namespace PhonoArk.Models;

public class ExamQuestionAttempt
{
    public int Id { get; set; }
    public int ExamResultId { get; set; }
    public int QuestionOrder { get; set; }
    public DateTime ExamDate { get; set; }

    public string PhonemeSymbol { get; set; } = string.Empty;
    public string CorrectWord { get; set; } = string.Empty;
    public string CorrectIpa { get; set; } = string.Empty;
    public string UserWord { get; set; } = string.Empty;
    public string UserIpa { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}
