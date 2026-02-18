using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PhonoArk.Models;
using PhonoArk.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace PhonoArk.ViewModels;

public partial class FavoritesViewModel : ViewModelBase
{
    private readonly FavoriteService _favoriteService;
    private readonly PhonemeDataService _phonemeDataService;

    [ObservableProperty]
    private ObservableCollection<FavoritePhoneme> _favorites = new();

    [ObservableProperty]
    private ObservableCollection<string> _groups = new();

    [ObservableProperty]
    private string _selectedGroup = "All";

    public FavoritesViewModel(FavoriteService favoriteService, PhonemeDataService phonemeDataService)
    {
        _favoriteService = favoriteService;
        _phonemeDataService = phonemeDataService;
    }

    public async Task LoadFavoritesAsync()
    {
        if (SelectedGroup == "All")
        {
            var allFavorites = await _favoriteService.GetAllFavoritesAsync();
            Favorites = new ObservableCollection<FavoritePhoneme>(allFavorites);
        }
        else
        {
            var groupFavorites = await _favoriteService.GetFavoritesByGroupAsync(SelectedGroup);
            Favorites = new ObservableCollection<FavoritePhoneme>(groupFavorites);
        }

        var groups = await _favoriteService.GetAllGroupsAsync();
        Groups = new ObservableCollection<string>(new[] { "All" }.Concat(groups));
    }

    [RelayCommand]
    private async Task RemoveFavoriteAsync(FavoritePhoneme favorite)
    {
        await _favoriteService.RemoveFavoriteAsync(favorite.PhonemeSymbol);
        await LoadFavoritesAsync();
    }

    [RelayCommand]
    private async Task ClearAllAsync()
    {
        await _favoriteService.ClearAllFavoritesAsync();
        await LoadFavoritesAsync();
    }

    partial void OnSelectedGroupChanged(string value)
    {
        _ = LoadFavoritesAsync();
    }
}
