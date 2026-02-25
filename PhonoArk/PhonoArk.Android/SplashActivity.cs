using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using System;

namespace PhonoArk.Android;

[Activity(
    Theme = "@style/MyTheme.Launch",
    MainLauncher = true,
    NoHistory = true,
    Exported = true,
    LaunchMode = LaunchMode.SingleTask,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class SplashActivity : Activity
{
    private const int MinSplashDurationMs = 300;
    private const float PortraitCropFocusY = 0.5f;

    private Handler? _mainHandler;
    private bool _navigated;
    private Java.Lang.IRunnable? _navigateRunnable;
    private ImageView? _splashImage;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        if (!IsTaskRoot
            && Intent is { } launchIntent
            && string.Equals(launchIntent.Action, Intent.ActionMain, StringComparison.Ordinal)
            && launchIntent.HasCategory(Intent.CategoryLauncher))
        {
            Finish();
            return;
        }

        _mainHandler = new Handler(Looper.MainLooper!);

        _splashImage = new ImageView(this);
        _splashImage.SetScaleType(ImageView.ScaleType.Matrix);
        try
        {
            _splashImage.SetImageResource(Resource.Drawable.logo);
        }
        catch (Resources.NotFoundException)
        {
            _splashImage.SetBackgroundColor(Color.White);
        }
        _splashImage.Post(ApplySplashImageCrop);

        SetContentView(_splashImage);

        _navigateRunnable = new Java.Lang.Runnable(NavigateToMain);
        _mainHandler.PostDelayed(_navigateRunnable, MinSplashDurationMs);
    }

    public override void OnConfigurationChanged(global::Android.Content.Res.Configuration newConfig)
    {
        base.OnConfigurationChanged(newConfig);
        if (_splashImage is null)
            return;

        try
        {
            _splashImage.SetImageResource(Resource.Drawable.logo);
        }
        catch (Resources.NotFoundException)
        {
            _splashImage.SetBackgroundColor(Color.White);
        }
        _splashImage.Post(ApplySplashImageCrop);
    }

    protected override void OnDestroy()
    {
        if (_mainHandler is not null && _navigateRunnable is not null)
        {
            _mainHandler.RemoveCallbacks(_navigateRunnable);
            _navigateRunnable.Dispose();
            _navigateRunnable = null;
        }

        base.OnDestroy();
    }

    private void NavigateToMain()
    {
        if (_navigated || IsFinishing || IsDestroyed)
            return;

        _navigated = true;

        var intent = new Intent(this, typeof(MainActivity));
        intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
        StartActivity(intent);
        Finish();
    }

    private void ApplySplashImageCrop()
    {
        if (_splashImage is null)
            return;

        var drawable = _splashImage.Drawable;
        var viewWidth = _splashImage.Width;
        var viewHeight = _splashImage.Height;
        var drawableWidth = drawable?.IntrinsicWidth ?? 0;
        var drawableHeight = drawable?.IntrinsicHeight ?? 0;

        if (drawable is null || viewWidth <= 0 || viewHeight <= 0 || drawableWidth <= 0 || drawableHeight <= 0)
        {
            _splashImage.SetScaleType(ImageView.ScaleType.CenterCrop);
            return;
        }

        var scale = Math.Max(viewWidth / (float)drawableWidth, viewHeight / (float)drawableHeight);
        var scaledWidth = drawableWidth * scale;
        var scaledHeight = drawableHeight * scale;

        var dx = (viewWidth - scaledWidth) / 2f;
        var dy = (viewHeight - scaledHeight) * PortraitCropFocusY;

        var matrix = new Matrix();
        matrix.SetScale(scale, scale);
        matrix.PostTranslate(dx, dy);

        _splashImage.SetScaleType(ImageView.ScaleType.Matrix);
        _splashImage.ImageMatrix = matrix;
    }
}
