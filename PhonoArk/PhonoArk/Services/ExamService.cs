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
        if (!string.IsNullOrWhiteSpace(_phonemeDataService.LoadError))
        {
            throw new InvalidOperationException(_phonemeDataService.LoadError);
        }

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
        var usedWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var shuffledPhonemes = phonemePool.OrderBy(_ => _random.Next()).ToList();
        if (shuffledPhonemes.Count == 0)
        {
            return questions;
        }

        for (int i = 0; i < questionCount; i++)
        {
            var phoneme = shuffledPhonemes[i % shuffledPhonemes.Count];
            
            if (phoneme.ExampleWords.Count == 0)
                continue;

            var targetToken = NormalizePhonemeToken(phoneme.Symbol);

            var correctCandidates = phoneme.ExampleWords
                .Where(word => ContainsTargetPhonemeToken(word, targetToken))
                .ToList();

            if (correctCandidates.Count == 0)
            {
                correctCandidates = phoneme.ExampleWords;
            }

            var availableCorrectWords = correctCandidates
                .Where(word => !usedWords.Contains(word.Word))
                .ToList();

            var correctSource = availableCorrectWords.Count > 0
                ? availableCorrectWords[_random.Next(availableCorrectWords.Count)]
                : correctCandidates[_random.Next(correctCandidates.Count)];

            var correctAnswer = CloneExampleWord(correctSource);
            usedWords.Add(correctAnswer.Word);

            // Generate wrong options from other phonemes
            var options = new List<ExampleWord> { correctAnswer };
            
            var otherPhonemes = phonemePool.Where(p => p.Symbol != phoneme.Symbol)
                .OrderBy(_ => _random.Next())
                .ToList();

            var strictWrongPool = otherPhonemes
                .SelectMany(p => p.ExampleWords)
                .Where(word => !ContainsTargetPhonemeToken(word, targetToken))
                .Where(word => !usedWords.Contains(word.Word))
                .GroupBy(word => word.Word, StringComparer.OrdinalIgnoreCase)
                .Select(group => group.First())
                .OrderBy(_ => _random.Next())
                .ToList();

            while (options.Count < 4 && strictWrongPool.Count > 0)
            {
                var wrongSource = strictWrongPool[0];
                strictWrongPool.RemoveAt(0);

                if (options.Any(o => o.Word.Equals(wrongSource.Word, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                var wrongWord = CloneExampleWord(wrongSource);
                options.Add(wrongWord);
                usedWords.Add(wrongWord.Word);
            }

            while (options.Count < 4 && otherPhonemes.Count > 0)
            {
                var wrongPhoneme = otherPhonemes[_random.Next(otherPhonemes.Count)];
                otherPhonemes.Remove(wrongPhoneme);
                
                if (wrongPhoneme.ExampleWords.Count > 0)
                {
                    var wrongCandidates = wrongPhoneme.ExampleWords
                        .Where(word => !ContainsTargetPhonemeToken(word, targetToken))
                        .Where(word => !usedWords.Contains(word.Word))
                        .ToList();

                    if (wrongCandidates.Count == 0)
                    {
                        continue;
                    }

                    var wrongSource = wrongCandidates.Count > 0
                        ? wrongCandidates[_random.Next(wrongCandidates.Count)]
                        : wrongPhoneme.ExampleWords[_random.Next(wrongPhoneme.ExampleWords.Count)];

                    var wrongWord = CloneExampleWord(wrongSource);

                    if (!options.Any(o => o.Word == wrongWord.Word))
                    {
                        options.Add(wrongWord);
                        usedWords.Add(wrongWord.Word);
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

    private static bool ContainsTargetPhonemeToken(ExampleWord word, string targetToken)
    {
        if (string.IsNullOrWhiteSpace(targetToken))
        {
            return false;
        }

        var transcription = NormalizePhonemeToken(word.IpaTranscription);
        return transcription.Contains(targetToken, StringComparison.Ordinal);
    }

    private static string NormalizePhonemeToken(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        var chars = text.Where(c => !char.IsWhiteSpace(c) && c != '/' && c != '[' && c != ']');
        return new string(chars.ToArray());
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

    private static ExampleWord CloneExampleWord(ExampleWord source)
    {
        return new ExampleWord
        {
            Word = source.Word,
            IpaTranscription = source.IpaTranscription,
            GenAmAudioPath = source.GenAmAudioPath,
            RpAudioPath = source.RpAudioPath,
            VoiceAudioPaths = new Dictionary<string, string>(source.VoiceAudioPaths, StringComparer.OrdinalIgnoreCase)
        };
    }
}
