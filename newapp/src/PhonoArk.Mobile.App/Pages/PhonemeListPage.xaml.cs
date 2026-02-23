using PhonoArk.Mobile.Core.Models;
using PhonoArk.Mobile.Core.ViewModels;
using PhonoArk.Mobile.Core.Services;

namespace PhonoArk.Mobile.App.Pages;

public partial class PhonemeListPage : ContentPage
{
    private readonly PhonemeListViewModel _viewModel;

    public PhonemeListPage(IPhonemeService phonemeService)
    {
        InitializeComponent();
        _viewModel = new PhonemeListViewModel(phonemeService);
        PhonemeGrid.ItemsSource = _viewModel.Phonemes;

        _viewModel.NavigateToDetail += async (phoneme) =>
        {
            var navParams = new Dictionary<string, object> { { "phoneme", phoneme } };
            await Shell.Current.GoToAsync("phonemeDetail", navParams);
        };
    }

    private void OnCategoryClicked(object? sender, EventArgs e)
    {
        if (sender is not Button btn) return;

        PhonemeCategory category;
        if (btn == BtnVowel) category = PhonemeCategory.Vowel;
        else if (btn == BtnDiphthong) category = PhonemeCategory.Diphthong;
        else category = PhonemeCategory.Consonant;

        _viewModel.SelectCategory(category);
        PhonemeGrid.ItemsSource = _viewModel.Phonemes;

        // 更新 Tab 样式
        var activeColor = (Color)Application.Current!.Resources["PrimaryColor"];
        BtnVowel.BackgroundColor = btn == BtnVowel ? activeColor : Color.FromArgb("#E0E0E0");
        BtnDiphthong.BackgroundColor = btn == BtnDiphthong ? activeColor : Color.FromArgb("#E0E0E0");
        BtnConsonant.BackgroundColor = btn == BtnConsonant ? activeColor : Color.FromArgb("#E0E0E0");

        BtnVowel.TextColor = btn == BtnVowel ? Colors.White : Color.FromArgb("#333333");
        BtnDiphthong.TextColor = btn == BtnDiphthong ? Colors.White : Color.FromArgb("#333333");
        BtnConsonant.TextColor = btn == BtnConsonant ? Colors.White : Color.FromArgb("#333333");
    }

    private void OnPhonemeClicked(object? sender, TappedEventArgs e)
    {
        if (sender is VisualElement element && element.BindingContext is Phoneme phoneme)
        {
            _viewModel.SelectPhoneme(phoneme);
        }
    }
}
