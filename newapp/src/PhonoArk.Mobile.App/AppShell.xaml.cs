namespace PhonoArk.Mobile.App;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // 注册导航路由
        Routing.RegisterRoute("phonemeList", typeof(Pages.PhonemeListPage));
        Routing.RegisterRoute("phonemeDetail", typeof(Pages.PhonemeDetailPage));
        Routing.RegisterRoute("exam", typeof(Pages.ExamPage));
        Routing.RegisterRoute("examResult", typeof(Pages.ExamResultPage));
    }
}
