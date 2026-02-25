using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Avalonia;
using Avalonia.Android;
using PhonoArk.Android.Services;
using PhonoArk.ViewModels;
using System;

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
        ConfigureUriLauncher();

        // Android 端优先使用系统字体（Roboto/Noto Sans），避免缺字与图标私有区字体不匹配。
        return base.CustomizeAppBuilder(builder);
    }

    /// <summary>
    /// 注册 Android 原生 URI 打开回调，使用 Intent.ActionView 启动外部浏览器。
    /// 当 Avalonia TopLevel.Launcher 在安卓上不可用时，这是可靠的回退方案。
    /// </summary>
    private void ConfigureUriLauncher()
    {
        SettingsViewModel.PlatformOpenUri = uri =>
        {
            try
            {
                var intent = new Intent(Intent.ActionView, global::Android.Net.Uri.Parse(uri.AbsoluteUri));
                intent.AddFlags(ActivityFlags.NewTask);
                StartActivity(intent);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"[PhonoArk] Failed to open URI: {ex.Message}");
            }
        };
    }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SuppressAvaloniaAccessibility();
    }

    public override void OnAttachedToWindow()
    {
        base.OnAttachedToWindow();
        SuppressAvaloniaAccessibility();
    }

    /// <summary>
    /// 规避 Avalonia 11.3.x 已知 Bug（GitHub Issue #20453）：
    /// ToggleNodeInfoProvider.PopulateNodeInfo 中反射调用
    /// s_checkedProperty.SetValue(this, ...) 传入了错误的 target 对象，
    /// 当 Android 无障碍服务枚举到 CheckBox/ToggleSwitch 虚拟节点时
    /// 抛出 TargetException 导致 FATAL EXCEPTION。
    ///
    /// 修复方式：遍历 View 树，移除 AvaloniaAccessHelper（ExploreByTouchHelper）
    /// 在 AvaloniaView 上注册的 AccessibilityDelegate，
    /// 彻底阻止 AccessibilityNodeProvider 被无障碍框架查询。
    ///
    /// TODO: 升级到包含修复的 Avalonia 版本后移除此变通方案。
    /// </summary>
    private void SuppressAvaloniaAccessibility()
    {
        if (Window?.DecorView is ViewGroup decor)
        {
            WalkAndClearAccessibilityDelegate(decor);
        }
    }

    private static void WalkAndClearAccessibilityDelegate(ViewGroup parent)
    {
        for (var i = 0; i < parent.ChildCount; i++)
        {
            var child = parent.GetChildAt(i);
            if (child == null) continue;

            child.SetAccessibilityDelegate(null);
            child.ImportantForAccessibility = ImportantForAccessibility.NoHideDescendants;

            if (child is ViewGroup group)
            {
                WalkAndClearAccessibilityDelegate(group);
            }
        }
    }
}
