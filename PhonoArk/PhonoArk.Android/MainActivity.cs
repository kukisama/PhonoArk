using Android.App;
using Android.Content.PM;
using Avalonia;
using Avalonia.Android;
using PhonoArk.Android.Services;

namespace PhonoArk.Android;

[Activity(
    Label = "音标方舟",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@mipmap/ic_launcher",
    MainLauncher = false,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity<App>
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        AndroidTtsBridge.Configure();

        // Android 端优先使用系统字体（Roboto/Noto Sans），避免缺字与图标私有区字体不匹配。
        return base.CustomizeAppBuilder(builder);
    }
}
