using Microsoft.Maui.Hosting;
using PhonoArk.Mobile.Core.Services;

namespace PhonoArk.Mobile.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>();

        // 注册服务
        builder.Services.AddSingleton<IPhonemeService, PhonemeService>();
        builder.Services.AddSingleton<IExamService, ExamService>();

        // 注册页面
        builder.Services.AddTransient<Pages.HomePage>();
        builder.Services.AddTransient<Pages.PhonemeListPage>();
        builder.Services.AddTransient<Pages.PhonemeDetailPage>();
        builder.Services.AddTransient<Pages.ExamPage>();
        builder.Services.AddTransient<Pages.ExamResultPage>();

        return builder.Build();
    }
}
