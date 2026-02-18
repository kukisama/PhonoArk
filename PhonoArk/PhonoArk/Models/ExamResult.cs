using System;

namespace PhonoArk.Models;

public class ExamResult
{
    public int Id { get; set; }
    public DateTime ExamDate { get; set; } = DateTime.UtcNow;
    public int TotalQuestions { get; set; }
    public int CorrectAnswers { get; set; }
    public string ExamScope { get; set; } = "All"; // "All", "Favorites", etc.
    public TimeSpan Duration { get; set; }
    
    public double Score => TotalQuestions > 0 ? (double)CorrectAnswers / TotalQuestions * 100 : 0;
}

