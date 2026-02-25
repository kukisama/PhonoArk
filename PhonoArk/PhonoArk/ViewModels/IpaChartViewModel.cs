using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using PhonoArk.Models;
using PhonoArk.Services;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhonoArk.ViewModels;

public partial class IpaChartViewModel : ViewModelBase
{
    private readonly PhonemeDataService _phonemeDataService;
    private readonly AudioService _audioService;
    private readonly FavoriteService _favoriteService;
    private readonly LocalizationService _localizationService;
    private readonly bool _isMobile;

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

    /// <summary>当前元音是否全部已收藏</summary>
    [ObservableProperty]
    private bool _allVowelsFavorited;

    /// <summary>当前双元音是否全部已收藏</summary>
    [ObservableProperty]
    private bool _allDiphthongsFavorited;

    /// <summary>当前辅音是否全部已收藏</summary>
    [ObservableProperty]
    private bool _allConsonantsFavorited;

    public string CurrentAccentDisplay => CurrentAccent switch
    {
        Accent.USJenny => _localizationService.GetString("AccentUsJenny"),
        Accent.RP => _localizationService.GetString("AccentRp"),
        _ => _localizationService.GetString("AccentGenAm")
    };

    public IpaChartViewModel(
        PhonemeDataService phonemeDataService,
        AudioService audioService,
        FavoriteService favoriteService,
        LocalizationService localizationService)
    {
        _phonemeDataService = phonemeDataService;
        _audioService = audioService;
        _favoriteService = favoriteService;
        _localizationService = localizationService;
        _currentAccent = _audioService.CurrentAccent;
        _isMobile = Application.Current?.ApplicationLifetime is ISingleViewApplicationLifetime;

        _audioService.AccentChanged += OnAudioServiceAccentChanged;
        _localizationService.PropertyChanged += (_, _) => OnPropertyChanged(nameof(CurrentAccentDisplay));

        LoadPhonemes();
        InitializeFavoriteStatesAsync().SafeFireAndForget("InitFavoriteStates");
    }

    private void LoadPhonemes()
    {
        var vowels = _phonemeDataService.GetPhonemesByType(PhonemeType.Vowel).Select(ClonePhonemeForView).ToList();
        var diphthongs = _phonemeDataService.GetPhonemesByType(PhonemeType.Diphthong).Select(ClonePhonemeForView).ToList();
        var consonants = _phonemeDataService.GetPhonemesByType(PhonemeType.Consonant).Select(ClonePhonemeForView).ToList();

        Vowels = new ObservableCollection<Phoneme>(vowels);
        Diphthongs = new ObservableCollection<Phoneme>(diphthongs);
        Consonants = new ObservableCollection<Phoneme>(consonants);
    }

    [RelayCommand(AllowConcurrentExecutions = true)]
    private async Task SelectPhonemeAsync(Phoneme phoneme)
    {
        UpdateSelectedPhonemeState(phoneme);
        ClearPlayingWords();

        if (_isMobile && phoneme.ExampleWords.Count == 0)
        {
            phoneme.ExampleWords = _phonemeDataService.GetExampleWordsBySymbol(phoneme.Symbol);
        }

        SelectedPhoneme = phoneme;
        phoneme.IsFavorite = await _favoriteService.IsFavoriteAsync(phoneme.Symbol);
        IsFavorite = phoneme.IsFavorite;

        // 点击音标时自动播放发音
        await _audioService.PlayPhonemeAsync(phoneme);
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
            SelectedPhoneme.IsFavorite = false;
        }
        else
        {
            await _favoriteService.AddFavoriteAsync(SelectedPhoneme.Symbol);
            IsFavorite = true;
            SelectedPhoneme.IsFavorite = true;
        }

        RefreshBatchFavoriteStates();
    }

    [RelayCommand]
    private void SwitchAccent()
    {
        CurrentAccent = CurrentAccent switch
        {
            Accent.USJenny => Accent.GenAm,
            Accent.GenAm => Accent.RP,
            _ => Accent.USJenny
        };

        _audioService.CurrentAccent = CurrentAccent;
    }

    partial void OnCurrentAccentChanged(Accent value)
    {
        OnPropertyChanged(nameof(CurrentAccentDisplay));
    }

    private void OnAudioServiceAccentChanged(object? sender, Accent accent)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (CurrentAccent != accent)
            {
                CurrentAccent = accent;
            }
        });
    }

    public void PrepareForDisplay()
    {
        // 保持音标区与已加载单词区常驻内存：
        // 切回页面时仅清理播放态，不清空选中项与词表缓存。
        ClearPlayingWords();
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

    private void UpdateSelectedPhonemeState(Phoneme selected)
    {
        foreach (var phoneme in Vowels)
        {
            phoneme.IsSelected = ReferenceEquals(phoneme, selected);
        }

        foreach (var phoneme in Diphthongs)
        {
            phoneme.IsSelected = ReferenceEquals(phoneme, selected);
        }

        foreach (var phoneme in Consonants)
        {
            phoneme.IsSelected = ReferenceEquals(phoneme, selected);
        }
    }

    private async Task InitializeFavoriteStatesAsync()
    {
        foreach (var phoneme in Vowels)
        {
            phoneme.IsFavorite = await _favoriteService.IsFavoriteAsync(phoneme.Symbol);
        }

        foreach (var phoneme in Diphthongs)
        {
            phoneme.IsFavorite = await _favoriteService.IsFavoriteAsync(phoneme.Symbol);
        }

        foreach (var phoneme in Consonants)
        {
            phoneme.IsFavorite = await _favoriteService.IsFavoriteAsync(phoneme.Symbol);
        }

        RefreshBatchFavoriteStates();
    }

    /// <summary>
    /// 批量切换某类音标的收藏状态：若全部已收藏则全部取消，否则全部收藏。
    /// </summary>
    private async Task ToggleFavoriteBatchAsync(ObservableCollection<Phoneme> phonemes)
    {
        var allFavorited = phonemes.All(p => p.IsFavorite);

        foreach (var phoneme in phonemes)
        {
            if (allFavorited)
            {
                await _favoriteService.RemoveFavoriteAsync(phoneme.Symbol);
                phoneme.IsFavorite = false;
            }
            else if (!phoneme.IsFavorite)
            {
                await _favoriteService.AddFavoriteAsync(phoneme.Symbol);
                phoneme.IsFavorite = true;
            }
        }

        // 同步当前选中音标的收藏状态
        if (SelectedPhoneme != null)
        {
            IsFavorite = SelectedPhoneme.IsFavorite;
        }

        RefreshBatchFavoriteStates();
    }

    [RelayCommand]
    private async Task ToggleFavoriteVowelsAsync()
    {
        await ToggleFavoriteBatchAsync(Vowels);
    }

    [RelayCommand]
    private async Task ToggleFavoriteDiphthongsAsync()
    {
        await ToggleFavoriteBatchAsync(Diphthongs);
    }

    [RelayCommand]
    private async Task ToggleFavoriteConsonantsAsync()
    {
        await ToggleFavoriteBatchAsync(Consonants);
    }

    [RelayCommand]
    private async Task ClearAllFavoritesAsync()
    {
        await _favoriteService.ClearAllFavoritesAsync();

        foreach (var phoneme in Vowels) phoneme.IsFavorite = false;
        foreach (var phoneme in Diphthongs) phoneme.IsFavorite = false;
        foreach (var phoneme in Consonants) phoneme.IsFavorite = false;

        if (SelectedPhoneme != null)
        {
            IsFavorite = false;
        }

        RefreshBatchFavoriteStates();
    }

    private void RefreshBatchFavoriteStates()
    {
        AllVowelsFavorited = Vowels.Count > 0 && Vowels.All(p => p.IsFavorite);
        AllDiphthongsFavorited = Diphthongs.Count > 0 && Diphthongs.All(p => p.IsFavorite);
        AllConsonantsFavorited = Consonants.Count > 0 && Consonants.All(p => p.IsFavorite);
    }

    private Phoneme ClonePhonemeForView(Phoneme source)
    {
        var phoneme = new Phoneme
        {
            Symbol = source.Symbol,
            Type = source.Type,
            Description = source.Description,
            GenAmAudioPath = source.GenAmAudioPath,
            RpAudioPath = source.RpAudioPath,
            VoiceAudioPaths = new Dictionary<string, string>(source.VoiceAudioPaths, System.StringComparer.OrdinalIgnoreCase),
            ExampleWords = _isMobile
                ? new List<ExampleWord>()
                : source.ExampleWords.Select(word => new ExampleWord
                {
                    Word = word.Word,
                    IpaTranscription = word.IpaTranscription,
                    GenAmAudioPath = word.GenAmAudioPath,
                    RpAudioPath = word.RpAudioPath,
                    VoiceAudioPaths = new Dictionary<string, string>(word.VoiceAudioPaths, System.StringComparer.OrdinalIgnoreCase)
                }).ToList()
        };

        return phoneme;
    }
}
