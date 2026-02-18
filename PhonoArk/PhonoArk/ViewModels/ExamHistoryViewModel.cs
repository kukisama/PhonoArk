using CommunityToolkit.Mvvm.ComponentModel;
using PhonoArk.Models;
using PhonoArk.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace PhonoArk.ViewModels;

public partial class ExamHistoryViewModel : ViewModelBase
{
    private readonly ExamHistoryService _examHistoryService;

    [ObservableProperty]
    private ObservableCollection<ExamResult> _examResults = new();

    [ObservableProperty]
    private double _averageScore;

    public ExamHistoryViewModel(ExamHistoryService examHistoryService)
    {
        _examHistoryService = examHistoryService;
    }

    public async Task LoadHistoryAsync()
    {
        var results = await _examHistoryService.GetAllResultsAsync();
        ExamResults = new ObservableCollection<ExamResult>(results);
        AverageScore = await _examHistoryService.GetAverageScoreAsync();
    }
}
