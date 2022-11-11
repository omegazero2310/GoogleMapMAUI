using Foundation;
using GGMapMAUI.Platforms.iOS;
using MAUIDotNet7.Platforms.iOS;
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

        GGMapMAUI.Platforms.iOS.FormsGoogleMaps.Init("you google map api key here", platformConfig);
        return base.FinishedLaunching(application, launchOptions);
    }
}
