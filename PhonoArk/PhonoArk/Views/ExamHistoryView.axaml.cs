using Avalonia.Controls;

namespace PhonoArk.Views;

public partial class ExamHistoryView : UserControl
{
    public ExamHistoryView()
    {
        InitializeComponent();
        Loaded += async (s, e) =>
        {
            if (DataContext is ViewModels.ExamHistoryViewModel vm)
            {
                await vm.LoadHistoryAsync();
            }
        };
    }
}
