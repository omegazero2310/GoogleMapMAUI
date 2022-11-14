using Foundation;
using GGMapMAUI.Platforms.iOS;
using MAUIDotNet7.Platforms.iOS;
using Microsoft.Maui.Embedding;
using UIKit;

namespace MAUIDotNet7;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        var platformConfig = new PlatformConfig
        {
            ImageFactory = new CachingImageFactory()
        };
        var builder = MauiApp.CreateBuilder();

        //Add Maui Controls
        builder.UseMauiEmbedding<Application>();
        var mauiApp = builder.Build();
        //add context
        var mauiContext = new MauiContext(mauiApp.Services);
        GGMapMAUI.Platforms.iOS.FormsGoogleMaps.Init("AIzaSyA693e6AYFtx0zye2mFJm3aXNgEAfL_cNM", platformConfig, mauiContext);
        return base.FinishedLaunching(application, launchOptions);
    }
}
