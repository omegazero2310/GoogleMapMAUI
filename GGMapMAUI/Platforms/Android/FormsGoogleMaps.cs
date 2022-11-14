using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.Gms.Maps;
using Android.OS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GGMapMAUI.Platforms.Android
{
    public static class FormsGoogleMaps
    {
        public static bool IsInitialized { get; private set; }

        public static Context Context { get; private set; }

        public static void Init(Activity activity, Bundle bundle, PlatformConfig config = null, MauiContext context = null)
        {
            if (IsInitialized)
                return;

            Context = activity;

            MapRenderer.Bundle = bundle;
            MapRenderer.MauiContext = context;
            MapRenderer.Config = config ?? new PlatformConfig();

#pragma warning disable 618
            if (GooglePlayServicesUtil.IsGooglePlayServicesAvailable(Context) == ConnectionResult.Success)
#pragma warning restore 618
            {
                try
                {
                    MapsInitializer.Initialize(Context);
                    IsInitialized = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Google Play Services Not Found");
                    Console.WriteLine("Exception: {0}", e);
                }
            }

            GeocoderBackend.Register(Context);
        }
    }
}
