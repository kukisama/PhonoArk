using PhonoArk.Mobile.Core.Models;
using PhonoArk.Mobile.Core.ViewModels;
using Xunit;

namespace PhonoArk.Mobile.Tests.ViewModels;

public class PhonemeDetailViewModelTests
{
    private static Phoneme CreateTestPhoneme() => new()
    {
        Symbol = "iː",
        Category = PhonemeCategory.Vowel,
        Description = "Long vowel",
        ExampleWords = new List<ExampleWord>
        {
            new() { Word = "see", IpaTranscription = "/siː/" },
            new() { Word = "meet", IpaTranscription = "/miːt/" },
            new() { Word = "key", IpaTranscription = "/kiː/" },
            new() { Word = "beach", IpaTranscription = "/biːtʃ/" },
            new() { Word = "sheep", IpaTranscription = "/ʃiːp/" },
        }
    };

    [Fact]
    public void Constructor_SetsProperties()
    {
        var phoneme = CreateTestPhoneme();
        var vm = new PhonemeDetailViewModel(phoneme);

        Assert.Equal("iː", vm.Symbol);
        Assert.Equal("Long vowel", vm.Description);
        Assert.Equal(PhonemeCategory.Vowel, vm.Category);
    }

    [Fact]
    public void ExampleWords_ReturnsFiveWords()
    {
        var vm = new PhonemeDetailViewModel(CreateTestPhoneme());
        Assert.Equal(5, vm.ExampleWords.Count);
    }

    [Fact]
    public void ExampleWords_EachHasWordAndIpa()
    {
        var vm = new PhonemeDetailViewModel(CreateTestPhoneme());
        foreach (var word in vm.ExampleWords)
        {
            Assert.False(string.IsNullOrEmpty(word.Word));
            Assert.False(string.IsNullOrEmpty(word.IpaTranscription));
        }
    }

    [Fact]
    public void PlayWord_ValidWord_RaisesPlayWordRequested()
    {
        var vm = new PhonemeDetailViewModel(CreateTestPhoneme());
        string? playedWord = null;
        vm.PlayWordRequested += w => playedWord = w;

        vm.PlayWord("see");

        Assert.Equal("see", playedWord);
    }

    [Fact]
    public void PlayWord_EmptyWord_DoesNotRaiseEvent()
    {
        var vm = new PhonemeDetailViewModel(CreateTestPhoneme());
        bool fired = false;
        vm.PlayWordRequested += _ => fired = true;

        vm.PlayWord("");

        Assert.False(fired);
    }

    [Fact]
    public void PlayWord_WhitespaceWord_DoesNotRaiseEvent()
    {
        var vm = new PhonemeDetailViewModel(CreateTestPhoneme());
        bool fired = false;
        vm.PlayWordRequested += _ => fired = true;

        vm.PlayWord("   ");

        Assert.False(fired);
    }

    [Fact]
    public void Constructor_NullPhoneme_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new PhonemeDetailViewModel(null!));
    }
}
