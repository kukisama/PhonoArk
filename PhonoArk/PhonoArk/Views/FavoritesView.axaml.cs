using Avalonia.Controls;

namespace PhonoArk.Views;

public partial class FavoritesView : UserControl
{
    public FavoritesView()
    {
        InitializeComponent();
        Loaded += async (s, e) =>
        {
            if (DataContext is ViewModels.FavoritesViewModel vm)
            {
                await vm.LoadFavoritesAsync();
            }
        };
    }
}
