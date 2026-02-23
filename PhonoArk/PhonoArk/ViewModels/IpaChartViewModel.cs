using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PhonoArk.Models;
using PhonoArk.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace PhonoArk.ViewModels;

public partial class IpaChartViewModel : ViewModelBase
{
    private readonly PhonemeDataService _phonemeDataService;
    private readonly AudioService _audioService;
    private readonly FavoriteService _favoriteService;

    [ObservableProperty]
    private ObservableCollection<Phoneme> _vowels = new();

    [ObservableProperty]
    private ObservableCollection<Phoneme> _diphthongs = new();

    [ObservableProperty]
    private ObservableCollection<Phoneme> _consonants = new();

    [ObservableProperty]
    private Phoneme? _selectedPhoneme;

    [ObservableProperty]
    private bool _isFavorite;

    [ObservableProperty]
    private Accent _currentAccent;

    public IpaChartViewModel(
        PhonemeDataService phonemeDataService,
        AudioService audioService,
        FavoriteService favoriteService)
    {
        _phonemeDataService = phonemeDataService;
        _audioService = audioService;
        _favoriteService = favoriteService;
        _currentAccent = _audioService.CurrentAccent;

        LoadPhonemes();
    }

    private void LoadPhonemes()
    {
        var vowels = _phonemeDataService.GetPhonemesByType(PhonemeType.Vowel);
        var diphthongs = _phonemeDataService.GetPhonemesByType(PhonemeType.Diphthong);
        var consonants = _phonemeDataService.GetPhonemesByType(PhonemeType.Consonant);

        Vowels = new ObservableCollection<Phoneme>(vowels);
        Diphthongs = new ObservableCollection<Phoneme>(diphthongs);
        Consonants = new ObservableCollection<Phoneme>(consonants);
    }

    [RelayCommand]
    private async Task SelectPhonemeAsync(Phoneme phoneme)
    {
        ClearPlayingWords();
        SelectedPhoneme = phoneme;
        IsFavorite = await _favoriteService.IsFavoriteAsync(phoneme.Symbol);
    }

    [RelayCommand(AllowConcurrentExecutions = true)]
    private async Task PlayPhonemeAsync(Phoneme phoneme)
    {
        await _audioService.PlayPhonemeAsync(phoneme);
    }

    [RelayCommand(AllowConcurrentExecutions = true)]
    private async Task PlayWordAsync(ExampleWord word)
    {
        MarkPlayingWord(word);
        await _audioService.PlayWordAsync(word);
    }

    [RelayCommand]
    private async Task ToggleFavoriteAsync()
    {
        if (SelectedPhoneme == null)
            return;

        if (IsFavorite)
        {
            await _favoriteService.RemoveFavoriteAsync(SelectedPhoneme.Symbol);
            IsFavorite = false;
        }
        else
        {
            await _favoriteService.AddFavoriteAsync(SelectedPhoneme.Symbol);
            IsFavorite = true;
        }
    }

    [RelayCommand]
    private void SwitchAccent()
    {
        CurrentAccent = CurrentAccent == Accent.GenAm ? Accent.RP : Accent.GenAm;
        _audioService.CurrentAccent = CurrentAccent;
    }

    private void MarkPlayingWord(ExampleWord activeWord)
    {
        ClearPlayingWords();
        activeWord.IsPlaying = true;
    }

    private void ClearPlayingWords()
    {
        foreach (var phoneme in Vowels)
        {
            foreach (var word in phoneme.ExampleWords)
            {
                word.IsPlaying = false;
            }
        }

        foreach (var phoneme in Diphthongs)
        {
            foreach (var word in phoneme.ExampleWords)
            {
                word.IsPlaying = false;
            }
        }

        foreach (var phoneme in Consonants)
        {
            foreach (var word in phoneme.ExampleWords)
            {
                word.IsPlaying = false;
            }
        }
    }
}
