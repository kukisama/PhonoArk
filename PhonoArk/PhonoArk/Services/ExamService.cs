using PhonoArk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhonoArk.Services;

public class ExamService
{
    private readonly PhonemeDataService _phonemeDataService;
    private readonly FavoriteService _favoriteService;
    private readonly Random _random = new();

    public ExamService(PhonemeDataService phonemeDataService, FavoriteService favoriteService)
    {
        _phonemeDataService = phonemeDataService;
        _favoriteService = favoriteService;
    }

    public async Task<List<ExamQuestion>> GenerateExamAsync(int questionCount, string scope = "All")
    {
        List<Phoneme> phonemePool;

        if (scope == "Favorites")
        {
            var favorites = await _favoriteService.GetAllFavoritesAsync();
            var favoriteSymbols = favorites.Select(f => f.PhonemeSymbol).ToHashSet();
            phonemePool = _phonemeDataService.GetAllPhonemes()
                .Where(p => favoriteSymbols.Contains(p.Symbol))
                .ToList();
        }
        else
        {
            phonemePool = _phonemeDataService.GetAllPhonemes();
        }

        if (phonemePool.Count == 0)
            return new List<ExamQuestion>();

        var questions = new List<ExamQuestion>();

        for (int i = 0; i < questionCount; i++)
        {
            var phoneme = phonemePool[_random.Next(phonemePool.Count)];
            
            if (phoneme.ExampleWords.Count == 0)
                continue;

            var correctAnswer = phoneme.ExampleWords[_random.Next(phoneme.ExampleWords.Count)];

            // Generate wrong options from other phonemes
            var options = new List<ExampleWord> { correctAnswer };
            
            var otherPhonemes = phonemePool.Where(p => p.Symbol != phoneme.Symbol).ToList();
            while (options.Count < 4 && otherPhonemes.Count > 0)
            {
                var wrongPhoneme = otherPhonemes[_random.Next(otherPhonemes.Count)];
                otherPhonemes.Remove(wrongPhoneme);
                
                if (wrongPhoneme.ExampleWords.Count > 0)
                {
                    var wrongWord = wrongPhoneme.ExampleWords[_random.Next(wrongPhoneme.ExampleWords.Count)];
                    if (!options.Any(o => o.Word == wrongWord.Word))
                    {
                        options.Add(wrongWord);
                    }
                }
            }

            // Shuffle options
            options = options.OrderBy(_ => _random.Next()).ToList();

            questions.Add(new ExamQuestion
            {
                Phoneme = phoneme,
                Options = options,
                CorrectAnswer = correctAnswer
            });
        }

        return questions;
    }

    public ExamQuestion GetNextQuestion(List<ExamQuestion> questions)
    {
        return questions.FirstOrDefault(q => !q.IsAnswered) ?? questions.Last();
    }

    public void AnswerQuestion(ExamQuestion question, ExampleWord answer)
    {
        question.UserAnswer = answer;
        question.IsAnswered = true;
    }

    public (int correct, int total) CalculateResults(List<ExamQuestion> questions)
    {
        int correct = questions.Count(q => q.IsCorrect);
        int total = questions.Count(q => q.IsAnswered);
        return (correct, total);
    }
}
