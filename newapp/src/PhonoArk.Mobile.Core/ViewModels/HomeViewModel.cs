using System;

namespace PhonoArk.Mobile.Core.ViewModels;

/// <summary>
/// 首页 ViewModel
/// </summary>
public class HomeViewModel : BaseViewModel
{
    /// <summary>请求导航到音标学习页</summary>
    public event Action? NavigateToLearning;

    /// <summary>请求导航到音标考试页</summary>
    public event Action? NavigateToExam;

    public void GoToLearning()
    {
        NavigateToLearning?.Invoke();
    }

    public void GoToExam()
    {
        NavigateToExam?.Invoke();
    }
}
