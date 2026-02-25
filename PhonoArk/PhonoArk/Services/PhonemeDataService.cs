using PhonoArk.Models;
using Avalonia.Platform;
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
    private readonly LocalizationService? _localizationService;
    private readonly Dictionary<string, string> _rawDescriptionsBySymbol = new(StringComparer.Ordinal);

    public string? LoadError { get; private set; }

    public PhonemeDataService(LocalizationService? localizationService = null)
    {
        _localizationService = localizationService;
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
        try
        {
            var json = TryReadWordBankJson(out var sourceDescription);
            if (string.IsNullOrWhiteSpace(json))
            {
                LoadError = $"词库文件未找到：{sourceDescription}";
                return;
            }

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
            var phonemeIndex = 0;

            foreach (var item in model.Phonemes)
            {
                phonemeIndex++;

                if (string.IsNullOrWhiteSpace(item.Symbol))
                {
                    continue;
                }

                var phoneme = new Phoneme
                {
                    Symbol = item.Symbol.Trim(),
                    Type = ParseType(item.Type),
                    Description = LocalizeDescription(item.Description?.Trim() ?? string.Empty)
                };
                phoneme.VoiceAudioPaths[Accent.USJenny.ToString()] = BuildUsJennyPhonemeAudioPath(phonemeIndex);

                _rawDescriptionsBySymbol[phoneme.Symbol] = item.Description?.Trim() ?? string.Empty;

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

    private static string? TryReadWordBankJson(out string sourceDescription)
    {
        var diskPath = Path.Combine(AppContext.BaseDirectory, "Data", "phoneme-word-bank.json");
        if (File.Exists(diskPath))
        {
            sourceDescription = diskPath;
            return File.ReadAllText(diskPath);
        }

        try
        {
            var uri = new Uri("avares://PhonoArk/Data/phoneme-word-bank.json");
            if (AssetLoader.Exists(uri))
            {
                using var stream = AssetLoader.Open(uri);
                using var reader = new StreamReader(stream);
                sourceDescription = uri.ToString();
                return reader.ReadToEnd();
            }
        }
        catch
        {
            // ignore and fall through to not found
        }

        sourceDescription = $"{diskPath} | avares://PhonoArk/Data/phoneme-word-bank.json";
        return null;
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

    private ExampleWord ToWord(WordItem source)
    {
        var normalizedWord = source.Word?.Trim() ?? string.Empty;

        var voiceAudioPaths = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (!string.IsNullOrWhiteSpace(normalizedWord))
        {
            voiceAudioPaths[Accent.USJenny.ToString()] = BuildUsJennyWordAudioPath(normalizedWord);
        }

        return new ExampleWord
        {
            Word = normalizedWord,
            IpaTranscription = source.IpaTranscription?.Trim() ?? string.Empty
            ,
            VoiceAudioPaths = voiceAudioPaths
        };
    }

    private static string BuildUsJennyPhonemeAudioPath(int phonemeIndex)
    {
        return $"Data/Exportfile/US-Jenny/phonemes/phonemes{phonemeIndex:00}.wav";
    }

    private static string BuildUsJennyWordAudioPath(string word)
    {
        var normalized = word.Trim().ToLowerInvariant();
        return $"Data/Exportfile/US-Jenny/words/{normalized}.wav";
    }

    private static bool ContainsPhoneme(string ipa, string phonemeSymbol)
    {
        if (string.IsNullOrWhiteSpace(ipa) || string.IsNullOrWhiteSpace(phonemeSymbol))
        {
            return false;
        }

        return ipa.Contains(phonemeSymbol, StringComparison.Ordinal);
    }

    private string LocalizeDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return string.Empty;
        }

        var isChinese = _localizationService?.CurrentCultureName.StartsWith("zh", StringComparison.OrdinalIgnoreCase) == true;
        if (!isChinese)
        {
            return description;
        }

        return description.Trim() switch
        {
            "Short vowel" => "短元音",
            "Long vowel" => "长元音",
            "Vowel" => "元音",
            "Diphthong" => "双元音",
            "Consonant" => "辅音",
            "Schwa" => "央元音",
            _ => description
        };
    }

    public List<Phoneme> GetAllPhonemes() => _phonemes;

    public List<Phoneme> GetPhonemesByType(PhonemeType type) =>
        _phonemes.Where(p => p.Type == type).ToList();

    public Phoneme? GetPhonemeBySymbol(string symbol) =>
        _phonemes.FirstOrDefault(p => p.Symbol == symbol);

    public List<ExampleWord> GetExampleWordsBySymbol(string symbol)
    {
        var source = GetPhonemeBySymbol(symbol);
        if (source == null)
        {
            return new List<ExampleWord>();
        }

        return source.ExampleWords
            .Select(word => new ExampleWord
            {
                Word = word.Word,
                IpaTranscription = word.IpaTranscription,
                GenAmAudioPath = word.GenAmAudioPath,
                RpAudioPath = word.RpAudioPath,
                VoiceAudioPaths = new Dictionary<string, string>(word.VoiceAudioPaths, StringComparer.OrdinalIgnoreCase)
            })
            .ToList();
    }

    public void RefreshLocalizedDescriptions()
    {
        if (_phonemes.Count == 0)
        {
            return;
        }

        foreach (var phoneme in _phonemes)
        {
            if (_rawDescriptionsBySymbol.TryGetValue(phoneme.Symbol, out var raw))
            {
                phoneme.Description = LocalizeDescription(raw);
            }
        }
    }
}
