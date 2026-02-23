using System;
using System.Collections.Generic;
using System.Linq;
using PhonoArk.Mobile.Core.Models;

namespace PhonoArk.Mobile.Core.Services;

/// <summary>
/// 考试服务
/// </summary>
public class ExamService : IExamService
{
    private readonly IPhonemeService _phonemeService;
    private readonly Random _random;

    public ExamService(IPhonemeService phonemeService)
    {
        _phonemeService = phonemeService ?? throw new ArgumentNullException(nameof(phonemeService));
        _random = new Random();
    }

    // 支持注入 Random 以便测试确定性
    internal ExamService(IPhonemeService phonemeService, Random random)
    {
        _phonemeService = phonemeService ?? throw new ArgumentNullException(nameof(phonemeService));
        _random = random ?? throw new ArgumentNullException(nameof(random));
    }

    public List<ExamQuestion> GenerateExam(int questionCount = 10)
    {
        if (questionCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(questionCount), "题目数量必须大于 0");

        var allPhonemes = _phonemeService.GetAllPhonemes();
        if (allPhonemes.Count == 0)
            return new List<ExamQuestion>();

        var questions = new List<ExamQuestion>();

        for (int i = 0; i < questionCount; i++)
        {
            // 交替生成两种题型
            var questionType = i % 2 == 0
                ? ExamQuestionType.ChoosePhonemeForWord
                : ExamQuestionType.ChooseWordForPhoneme;

            var question = GenerateQuestion(allPhonemes, questionType);
            if (question != null)
                questions.Add(question);
        }

        return questions;
    }

    public bool CheckAnswer(ExamQuestion question, int selectedIndex)
    {
        if (question == null)
            throw new ArgumentNullException(nameof(question));
        if (selectedIndex < 0 || selectedIndex >= question.Options.Count)
            return false;

        return selectedIndex == question.CorrectIndex;
    }

    public ExamResult CalculateResult(List<AnswerRecord> records)
    {
        if (records == null)
            throw new ArgumentNullException(nameof(records));

        return new ExamResult
        {
            TotalQuestions = records.Count,
            CorrectCount = records.Count(r => r.IsCorrect),
            AnswerRecords = records
        };
    }

    private ExamQuestion? GenerateQuestion(IReadOnlyList<Phoneme> phonemes, ExamQuestionType questionType)
    {
        if (phonemes.Count < 4)
            return null;

        var correctPhoneme = phonemes[_random.Next(phonemes.Count)];

        if (correctPhoneme.ExampleWords.Count == 0)
            return null;

        switch (questionType)
        {
            case ExamQuestionType.ChoosePhonemeForWord:
                return GenerateChoosePhonemeQuestion(phonemes, correctPhoneme);
            case ExamQuestionType.ChooseWordForPhoneme:
                return GenerateChooseWordQuestion(phonemes, correctPhoneme);
            default:
                return null;
        }
    }

    private ExamQuestion GenerateChoosePhonemeQuestion(IReadOnlyList<Phoneme> phonemes, Phoneme correctPhoneme)
    {
        var word = correctPhoneme.ExampleWords[_random.Next(correctPhoneme.ExampleWords.Count)];

        // 生成 3 个干扰项（不同音标）
        var distractors = phonemes
            .Where(p => p.Symbol != correctPhoneme.Symbol)
            .OrderBy(_ => _random.Next())
            .Take(3)
            .Select(p => p.Symbol)
            .ToList();

        var options = new List<string>(distractors) { correctPhoneme.Symbol };
        var correctIndex = ShuffleAndGetCorrectIndex(options, correctPhoneme.Symbol);

        return new ExamQuestion
        {
            QuestionType = ExamQuestionType.ChoosePhonemeForWord,
            Prompt = $"单词 \"{word.Word}\" 包含哪个音标？",
            PhonemeSymbol = correctPhoneme.Symbol,
            Options = options,
            CorrectIndex = correctIndex
        };
    }

    private ExamQuestion GenerateChooseWordQuestion(IReadOnlyList<Phoneme> phonemes, Phoneme correctPhoneme)
    {
        var correctWord = correctPhoneme.ExampleWords[_random.Next(correctPhoneme.ExampleWords.Count)];

        // 从其他音标中选取干扰单词
        var distractorWords = phonemes
            .Where(p => p.Symbol != correctPhoneme.Symbol)
            .SelectMany(p => p.ExampleWords)
            .Where(w => w.Word != correctWord.Word)
            .OrderBy(_ => _random.Next())
            .Take(3)
            .Select(w => w.Word)
            .ToList();

        var options = new List<string>(distractorWords) { correctWord.Word };
        var correctIndex = ShuffleAndGetCorrectIndex(options, correctWord.Word);

        return new ExamQuestion
        {
            QuestionType = ExamQuestionType.ChooseWordForPhoneme,
            Prompt = $"音标 /{correctPhoneme.Symbol}/ 对应哪个单词？",
            PhonemeSymbol = correctPhoneme.Symbol,
            Options = options,
            CorrectIndex = correctIndex
        };
    }

    private int ShuffleAndGetCorrectIndex(List<string> options, string correctOption)
    {
        // Fisher-Yates 洗牌
        for (int i = options.Count - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            (options[i], options[j]) = (options[j], options[i]);
        }

        return options.IndexOf(correctOption);
    }
}
