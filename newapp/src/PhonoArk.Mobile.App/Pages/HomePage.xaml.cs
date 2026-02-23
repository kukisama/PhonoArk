namespace PhonoArk.Mobile.App.Pages;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
    }

    private async void OnLearningClicked(object? sender, EventArgs e)
    {
        try
        {
            await Shell.Current.GoToAsync("phonemeList");
        }
        catch (Exception ex)
        {
            await DisplayAlert("错误", ex.Message, "确定");
        }
    }

    private async void OnExamClicked(object? sender, EventArgs e)
    {
        try
        {
            await Shell.Current.GoToAsync("exam");
        }
        catch (Exception ex)
        {
            await DisplayAlert("错误", ex.Message, "确定");
        }
    }
}
