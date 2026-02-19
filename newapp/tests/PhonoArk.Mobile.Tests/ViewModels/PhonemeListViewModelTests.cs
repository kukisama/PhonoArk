using PhonoArk.Mobile.Core.Models;
using PhonoArk.Mobile.Core.Services;
using PhonoArk.Mobile.Core.ViewModels;
using Xunit;

namespace PhonoArk.Mobile.Tests.ViewModels;

public class PhonemeListViewModelTests
{
    private readonly PhonemeService _phonemeService;

    public PhonemeListViewModelTests()
    {
        _phonemeService = new PhonemeService();
    }

    [Fact]
    public void Constructor_DefaultsToVowelCategory()
    {
        var vm = new PhonemeListViewModel(_phonemeService);
        Assert.Equal(PhonemeCategory.Vowel, vm.SelectedCategory);
    }

    [Fact]
    public void Constructor_LoadsVowelPhonemes()
    {
        var vm = new PhonemeListViewModel(_phonemeService);
        Assert.NotEmpty(vm.Phonemes);
        Assert.All(vm.Phonemes, p => Assert.Equal(PhonemeCategory.Vowel, p.Category));
    }

    [Fact]
    public void SelectCategory_ChangesToDiphthong_LoadsDiphthongs()
    {
        var vm = new PhonemeListViewModel(_phonemeService);
        vm.SelectCategory(PhonemeCategory.Diphthong);

        Assert.Equal(PhonemeCategory.Diphthong, vm.SelectedCategory);
        Assert.NotEmpty(vm.Phonemes);
        Assert.All(vm.Phonemes, p => Assert.Equal(PhonemeCategory.Diphthong, p.Category));
    }

    [Fact]
    public void SelectCategory_ChangesToConsonant_LoadsConsonants()
    {
        var vm = new PhonemeListViewModel(_phonemeService);
        vm.SelectCategory(PhonemeCategory.Consonant);

        Assert.Equal(PhonemeCategory.Consonant, vm.SelectedCategory);
        Assert.NotEmpty(vm.Phonemes);
        Assert.All(vm.Phonemes, p => Assert.Equal(PhonemeCategory.Consonant, p.Category));
    }

    [Fact]
    public void SelectCategory_NotifiesPropertyChanged()
    {
        var vm = new PhonemeListViewModel(_phonemeService);
        var changedProperties = new List<string>();
        vm.PropertyChanged += (_, e) => changedProperties.Add(e.PropertyName!);

        vm.SelectCategory(PhonemeCategory.Consonant);

        Assert.Contains("SelectedCategory", changedProperties);
        Assert.Contains("Phonemes", changedProperties);
    }

    [Fact]
    public void SelectPhoneme_RaisesNavigateToDetail()
    {
        var vm = new PhonemeListViewModel(_phonemeService);
        Phoneme? navigated = null;
        vm.NavigateToDetail += p => navigated = p;

        var phoneme = vm.Phonemes.First();
        vm.SelectPhoneme(phoneme);

        Assert.NotNull(navigated);
        Assert.Equal(phoneme.Symbol, navigated!.Symbol);
    }

    [Fact]
    public void Categories_ReturnsThreeCategories()
    {
        var vm = new PhonemeListViewModel(_phonemeService);
        Assert.Equal(3, vm.Categories.Count);
    }

    [Fact]
    public void Constructor_NullService_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new PhonemeListViewModel(null!));
    }
}
