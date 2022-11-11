#if ANDROID
using GGMapMAUI.Platforms.Android;
#endif
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Compatibility.Hosting;

namespace MAUIDotNet7;

public static class MauiProgram
{
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
		});

#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
