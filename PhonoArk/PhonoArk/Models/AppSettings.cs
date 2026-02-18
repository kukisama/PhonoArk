namespace PhonoArk.Models;

public class AppSettings
{
    public int Id { get; set; }
    public Accent DefaultAccent { get; set; } = Accent.GenAm;
    public double Volume { get; set; } = 0.8;
    public int ExamQuestionCount { get; set; } = 10;
    public bool DarkMode { get; set; } = false;
    public bool RemindersEnabled { get; set; } = false;
}
