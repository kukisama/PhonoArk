using Avalonia.Controls;
using System;

namespace PhonoArk.Views;

public partial class ExamHistoryView : UserControl
{
    public ExamHistoryView()
    {
        InitializeComponent();
        Loaded += OnLoadedAsync;
    }

    private async void OnLoadedAsync(object? s, EventArgs e)
    {
        try
        {
            if (DataContext is ViewModels.ExamHistoryViewModel vm)
            {
                await vm.LoadHistoryAsync();
            }
        }
        catch (Exception ex)
        {
            // Log error or show user-friendly message
            System.Diagnostics.Debug.WriteLine($"Error loading exam history: {ex.Message}");
        }
    }
}
