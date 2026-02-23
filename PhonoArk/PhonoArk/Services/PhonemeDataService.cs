using PhonoArk.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace PhonoArk.Services;

public class PhonemeDataService
{
    private readonly List<Phoneme> _phonemes = new();
    private readonly Random _random = new();

    public string? LoadError { get; private set; }

    public PhonemeDataService()
    {
        LoadFromExternalJson();
    }

    private sealed class WordBankModel
    {
        public int TargetWordsPerPhoneme { get; set; } = 30;
        public List<PhonemeItem> Phonemes { get; set; } = new();
        public List<WordItem> GlobalWords { get; set; } = new();
    }

    private sealed class PhonemeItem
    {
        public string Symbol { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<WordItem> Words { get; set; } = new();
    }

    private sealed class WordItem
    {
        public string Word { get; set; } = string.Empty;
        public string IpaTranscription { get; set; } = string.Empty;
    }

    private void LoadFromExternalJson()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "Data", "phoneme-word-bank.json");

        if (!File.Exists(filePath))
        {
            LoadError = $"词库文件未找到：{filePath}";
            return;
        }

        try
        {
            var json = File.ReadAllText(filePath);
            var model = JsonSerializer.Deserialize<WordBankModel>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (model == null || model.Phonemes.Count == 0)
            {
                LoadError = "词库文件格式错误：缺少 phonemes 节点。";
                return;
            }

            var targetCount = Math.Max(4, model.TargetWordsPerPhoneme);

            foreach (var item in model.Phonemes)
            {
                if (string.IsNullOrWhiteSpace(item.Symbol))
                {
                    continue;
                }

                var phoneme = new Phoneme
                {
                    Symbol = item.Symbol.Trim(),
                    Type = ParseType(item.Type),
                    Description = item.Description?.Trim() ?? string.Empty
                };

                var mergedWords = new List<ExampleWord>();
                mergedWords.AddRange((item.Words ?? new List<WordItem>()).Select(ToWord));

                var globalMatches = model.GlobalWords
                    .Where(word => ContainsPhoneme(word.IpaTranscription, phoneme.Symbol))
                    .Select(ToWord);

                mergedWords.AddRange(globalMatches);

                var distinctWords = mergedWords
                    .Where(word => !string.IsNullOrWhiteSpace(word.Word) && !string.IsNullOrWhiteSpace(word.IpaTranscription))
                    .GroupBy(word => word.Word.Trim().ToLowerInvariant())
                    .Select(group => group.First())
                    .ToList();

                phoneme.ExampleWords = distinctWords.Count > targetCount
                    ? distinctWords.OrderBy(_ => _random.Next()).Take(targetCount).ToList()
                    : distinctWords;

                _phonemes.Add(phoneme);
            }

            if (_phonemes.Count == 0)
            {
                LoadError = "词库文件读取成功，但未解析出有效音标数据。";
            }
        }
        catch (Exception ex)
        {
            LoadError = $"词库读取失败：{ex.Message}";
        }
    }

    private static PhonemeType ParseType(string type)
    {
        return type.Trim().ToLowerInvariant() switch
        {
            "vowel" => PhonemeType.Vowel,
            "diphthong" => PhonemeType.Diphthong,
            _ => PhonemeType.Consonant
        };
    }

    private static ExampleWord ToWord(WordItem source)
    {
        return new ExampleWord
        {
            Word = source.Word?.Trim() ?? string.Empty,
            IpaTranscription = source.IpaTranscription?.Trim() ?? string.Empty
        };
    }

    private static bool ContainsPhoneme(string ipa, string phonemeSymbol)
    {
        if (string.IsNullOrWhiteSpace(ipa) || string.IsNullOrWhiteSpace(phonemeSymbol))
        {
            return false;
        }

        return ipa.Contains(phonemeSymbol, StringComparison.Ordinal);
    }

    public List<Phoneme> GetAllPhonemes() => _phonemes;

    public List<Phoneme> GetPhonemesByType(PhonemeType type) =>
        _phonemes.Where(p => p.Type == type).ToList();

    public Phoneme? GetPhonemeBySymbol(string symbol) =>
        _phonemes.FirstOrDefault(p => p.Symbol == symbol);
}
