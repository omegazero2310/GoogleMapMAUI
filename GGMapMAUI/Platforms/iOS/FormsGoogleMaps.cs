using Google.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GGMapMAUI.Platforms.iOS
{
    public static class FormsGoogleMaps
    {
        public static bool IsInitialized { get; private set; }

        public static void Init(string apiKey, PlatformConfig config = null, MauiContext context = null)
        {
            MapServices.ProvideApiKey(apiKey);
            GeocoderBackend.Register();
            MapRenderer.Config = config ?? new PlatformConfig();
            MapRenderer.MauiContext = context;
            IsInitialized = true;
        }
    }
}
