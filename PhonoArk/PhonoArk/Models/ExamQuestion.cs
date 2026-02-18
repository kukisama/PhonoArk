using System.Collections.Generic;

namespace PhonoArk.Models;

public class ExamQuestion
{
    public Phoneme Phoneme { get; set; } = new();
    public List<ExampleWord> Options { get; set; } = new();
    public ExampleWord CorrectAnswer { get; set; } = new();
    public ExampleWord? UserAnswer { get; set; }
    public bool IsAnswered { get; set; }
    public bool IsCorrect => IsAnswered && UserAnswer?.Word == CorrectAnswer.Word;
}

