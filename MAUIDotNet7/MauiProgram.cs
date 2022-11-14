#if ANDROID
using GGMapMAUI.Platforms.Android;
#endif
#if IOS
using GGMapMAUI.Platforms.iOS;
#endif
using Microsoft.Extensions.Logging;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Compatibility.Hosting;
using Microsoft.Maui.Embedding;
using Microsoft.Maui.Hosting;

namespace MAUIDotNet7;

public static class MauiProgram
{
	public static MauiContext MauiContext { get; private set; }
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
		builder.UseMauiCompatibility();
        builder.ConfigureMauiHandlers(handlers =>
        {
#if ANDROID
			handlers.AddCompatibilityRenderer(typeof(Maui.GoogleMaps.Map), typeof(MapRenderer));
#endif
#if IOS
			handlers.AddCompatibilityRenderer(typeof(Maui.GoogleMaps.Map), typeof(MapRenderer));
#endif
        });

#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
	}
}
