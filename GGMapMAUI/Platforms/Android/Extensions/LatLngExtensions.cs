using Android.Gms.Maps.Model;
using Maui.GoogleMaps;

namespace GGMapMAUI.Platforms.Android.Extensions
{
    public static class LatLngExtensions
    {
        public static Position ToPosition(this LatLng self)
        {
            return new Position(self.Latitude, self.Longitude);
        }
    }
}
