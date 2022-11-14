using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using GGMapMAUI.Platforms.Android;
using Maui.GoogleMaps.Logics;
using MAUIDotNet7.Platforms.Android;
using Microsoft.Maui;
using Microsoft.Maui.Embedding;
using Microsoft.Maui.Platform;

namespace MAUIDotNet7;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        // Override default BitmapDescriptorFactory by your implementation. 
        var platformConfig = new PlatformConfig
        {
            BitmapDescriptorFactory = new CachingNativeBitmapDescriptorFactory()
        };
        //Setup MauiBits
        var mauiEmbeddedApp = MauiApp
                            .CreateBuilder()
                            .UseMauiEmbedding<Microsoft.Maui.Controls.Application>()
                            .Build();
        var mauiContext = new MauiContext(mauiEmbeddedApp.Services, this);
        GGMapMAUI.Platforms.Android.FormsGoogleMaps.Init(this, savedInstanceState, platformConfig, mauiContext); // initialize for Xamarin.Forms.GoogleMaps
        base.OnCreate(savedInstanceState);

    }
}
