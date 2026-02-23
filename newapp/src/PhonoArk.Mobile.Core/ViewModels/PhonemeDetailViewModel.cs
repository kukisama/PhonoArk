using System;
using System.Collections.Generic;
using PhonoArk.Mobile.Core.Models;

namespace PhonoArk.Mobile.Core.ViewModels;

/// <summary>
/// 音标详情 ViewModel
/// </summary>
public class PhonemeDetailViewModel : BaseViewModel
{
    private Phoneme _phoneme;

    /// <summary>请求播放单词发音</summary>
    public event Action<string>? PlayWordRequested;

    public PhonemeDetailViewModel(Phoneme phoneme)
    {
        _phoneme = phoneme ?? throw new ArgumentNullException(nameof(phoneme));
    }

    /// <summary>音标符号</summary>
    public string Symbol => _phoneme.Symbol;

    /// <summary>音标描述</summary>
    public string Description => _phoneme.Description;

    /// <summary>音标类别</summary>
    public PhonemeCategory Category => _phoneme.Category;

    /// <summary>示例单词列表</summary>
    public IReadOnlyList<ExampleWord> ExampleWords => _phoneme.ExampleWords.AsReadOnly();

    /// <summary>播放单词发音</summary>
    public void PlayWord(string word)
    {
        if (!string.IsNullOrWhiteSpace(word))
            PlayWordRequested?.Invoke(word);
    }
}
