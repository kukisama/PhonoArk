using PhonoArk.Mobile.Core.Models;
using PhonoArk.Mobile.Core.Services;
using Xunit;

namespace PhonoArk.Mobile.Tests.Services;

public class PhonemeServiceTests
{
    private readonly PhonemeService _service;

    public PhonemeServiceTests()
    {
        _service = new PhonemeService();
    }

    [Fact]
    public void GetAllPhonemes_ReturnsNonEmptyList()
    {
        var phonemes = _service.GetAllPhonemes();
        Assert.NotEmpty(phonemes);
    }

    [Fact]
    public void GetAllPhonemes_ContainsAllThreeCategories()
    {
        var phonemes = _service.GetAllPhonemes();
        Assert.Contains(phonemes, p => p.Category == PhonemeCategory.Vowel);
        Assert.Contains(phonemes, p => p.Category == PhonemeCategory.Diphthong);
        Assert.Contains(phonemes, p => p.Category == PhonemeCategory.Consonant);
    }

    [Fact]
    public void GetAllPhonemes_EachPhonemeHasAtLeastFiveExampleWords()
    {
        var phonemes = _service.GetAllPhonemes();
        foreach (var phoneme in phonemes)
        {
            Assert.True(phoneme.ExampleWords.Count >= 5,
                $"音标 {phoneme.Symbol} 的示例单词少于 5 个，实际为 {phoneme.ExampleWords.Count}");
        }
    }

    [Fact]
    public void GetAllPhonemes_EachPhonemeHasNonEmptySymbol()
    {
        var phonemes = _service.GetAllPhonemes();
        foreach (var phoneme in phonemes)
        {
            Assert.False(string.IsNullOrWhiteSpace(phoneme.Symbol),
                "存在空白音标符号");
        }
    }

    [Fact]
    public void GetAllPhonemes_EachPhonemeHasDescription()
    {
        var phonemes = _service.GetAllPhonemes();
        foreach (var phoneme in phonemes)
        {
            Assert.False(string.IsNullOrWhiteSpace(phoneme.Description),
                $"音标 {phoneme.Symbol} 缺少描述");
        }
    }

    [Fact]
    public void GetAllPhonemes_ExampleWordsHaveIpaTranscription()
    {
        var phonemes = _service.GetAllPhonemes();
        foreach (var phoneme in phonemes)
        {
            foreach (var word in phoneme.ExampleWords)
            {
                Assert.False(string.IsNullOrWhiteSpace(word.IpaTranscription),
                    $"音标 {phoneme.Symbol} 的单词 {word.Word} 缺少音标标注");
            }
        }
    }

    [Fact]
    public void GetAllPhonemes_NoDuplicateSymbols()
    {
        var phonemes = _service.GetAllPhonemes();
        var symbols = phonemes.Select(p => p.Symbol).ToList();
        Assert.Equal(symbols.Count, symbols.Distinct().Count());
    }

    [Theory]
    [InlineData(PhonemeCategory.Vowel)]
    [InlineData(PhonemeCategory.Diphthong)]
    [InlineData(PhonemeCategory.Consonant)]
    public void GetPhonemesByCategory_ReturnsOnlyMatchingCategory(PhonemeCategory category)
    {
        var phonemes = _service.GetPhonemesByCategory(category);
        Assert.NotEmpty(phonemes);
        Assert.All(phonemes, p => Assert.Equal(category, p.Category));
    }

    [Fact]
    public void GetPhonemesByCategory_Vowels_Returns12Phonemes()
    {
        var vowels = _service.GetPhonemesByCategory(PhonemeCategory.Vowel);
        Assert.Equal(12, vowels.Count);
    }

    [Fact]
    public void GetPhonemesByCategory_Diphthongs_Returns8Phonemes()
    {
        var diphthongs = _service.GetPhonemesByCategory(PhonemeCategory.Diphthong);
        Assert.Equal(8, diphthongs.Count);
    }

    [Fact]
    public void GetPhonemesByCategory_Consonants_HasMultiplePhonemes()
    {
        var consonants = _service.GetPhonemesByCategory(PhonemeCategory.Consonant);
        Assert.True(consonants.Count >= 20,
            $"辅音应至少有 20 个，实际为 {consonants.Count}");
    }

    [Theory]
    [InlineData("iː")]
    [InlineData("æ")]
    [InlineData("θ")]
    [InlineData("eɪ")]
    public void GetPhonemeBySymbol_ReturnsCorrectPhoneme(string symbol)
    {
        var phoneme = _service.GetPhonemeBySymbol(symbol);
        Assert.NotNull(phoneme);
        Assert.Equal(symbol, phoneme.Symbol);
    }

    [Fact]
    public void GetPhonemeBySymbol_NonExistentSymbol_ReturnsNull()
    {
        var phoneme = _service.GetPhonemeBySymbol("xyz");
        Assert.Null(phoneme);
    }

    [Fact]
    public void GetCategories_ReturnsAllThreeCategories()
    {
        var categories = _service.GetCategories();
        Assert.Equal(3, categories.Count);
        Assert.Contains(PhonemeCategory.Vowel, categories);
        Assert.Contains(PhonemeCategory.Diphthong, categories);
        Assert.Contains(PhonemeCategory.Consonant, categories);
    }

    [Fact]
    public void GetPhonemesByCategory_SumMatchesTotalCount()
    {
        var all = _service.GetAllPhonemes();
        var vowels = _service.GetPhonemesByCategory(PhonemeCategory.Vowel);
        var diphthongs = _service.GetPhonemesByCategory(PhonemeCategory.Diphthong);
        var consonants = _service.GetPhonemesByCategory(PhonemeCategory.Consonant);

        Assert.Equal(all.Count, vowels.Count + diphthongs.Count + consonants.Count);
    }
}
