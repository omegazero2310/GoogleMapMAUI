using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using GGMapMAUI.Platforms.Android;
using MAUIDotNet7.Platforms.Android;

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

        GGMapMAUI.Platforms.Android.FormsGoogleMaps.Init(this, savedInstanceState, platformConfig); // initialize for Xamarin.Forms.GoogleMaps
        base.OnCreate(savedInstanceState);       
    }
}
