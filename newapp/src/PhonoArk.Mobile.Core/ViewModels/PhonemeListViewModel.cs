using System;
using System.Collections.Generic;
using System.Linq;
using PhonoArk.Mobile.Core.Models;
using PhonoArk.Mobile.Core.Services;

namespace PhonoArk.Mobile.Core.ViewModels;

/// <summary>
/// 音标列表 ViewModel
/// </summary>
public class PhonemeListViewModel : BaseViewModel
{
    private readonly IPhonemeService _phonemeService;

    private PhonemeCategory _selectedCategory;
    private IReadOnlyList<Phoneme> _phonemes = Array.Empty<Phoneme>();

    /// <summary>请求导航到音标详情页</summary>
    public event Action<Phoneme>? NavigateToDetail;

    public PhonemeListViewModel(IPhonemeService phonemeService)
    {
        _phonemeService = phonemeService ?? throw new ArgumentNullException(nameof(phonemeService));
        _selectedCategory = PhonemeCategory.Vowel;
        LoadPhonemes();
    }

    /// <summary>当前选中的类别</summary>
    public PhonemeCategory SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            if (SetProperty(ref _selectedCategory, value))
                LoadPhonemes();
        }
    }

    /// <summary>当前类别下的音标列表</summary>
    public IReadOnlyList<Phoneme> Phonemes
    {
        get => _phonemes;
        private set => SetProperty(ref _phonemes, value);
    }

    /// <summary>所有可用类别</summary>
    public IReadOnlyList<PhonemeCategory> Categories =>
        _phonemeService.GetCategories();

    /// <summary>切换到指定类别</summary>
    public void SelectCategory(PhonemeCategory category)
    {
        SelectedCategory = category;
    }

    /// <summary>选择一个音标进入详情</summary>
    public void SelectPhoneme(Phoneme phoneme)
    {
        NavigateToDetail?.Invoke(phoneme);
    }

    private void LoadPhonemes()
    {
        Phonemes = _phonemeService.GetPhonemesByCategory(_selectedCategory);
    }
}
