using PhonoArk.Mobile.Core.Models;
using PhonoArk.Mobile.Core.ViewModels;

namespace PhonoArk.Mobile.App.Pages;

[QueryProperty(nameof(PhonemeParam), "phoneme")]
public partial class PhonemeDetailPage : ContentPage
{
    private PhonemeDetailViewModel? _viewModel;

    public PhonemeDetailPage()
    {
        InitializeComponent();
    }

    public Phoneme PhonemeParam
    {
        set
        {
            _viewModel = new PhonemeDetailViewModel(value);
            _viewModel.PlayWordRequested += OnPlayWord;
            LblSymbol.Text = $"/{_viewModel.Symbol}/";
            LblDescription.Text = _viewModel.Description;
            WordList.ItemsSource = _viewModel.ExampleWords;
        }
    }

    private void OnWordClicked(object? sender, TappedEventArgs e)
    {
        if (sender is VisualElement element && element.BindingContext is ExampleWord word)
        {
            _viewModel?.PlayWord(word.Word);
        }
    }

    private async void OnPlayWord(string word)
    {
        try
        {
            // 使用系统 TTS 朗读单词
            await TextToSpeech.Default.SpeakAsync(word, new SpeechOptions
            {
                Locale = Locale.GetLocalesAsync().Result?.FirstOrDefault(l =>
                    l.Language == "en") ?? null
            });
        }
        catch
        {
            // TTS 不可用时静默处理
        }
    }
}
