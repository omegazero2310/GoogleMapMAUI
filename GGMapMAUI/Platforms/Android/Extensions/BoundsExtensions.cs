using Android.Gms.Maps.Model;
using Maui.GoogleMaps;

namespace GGMapMAUI.Platforms.Android.Extensions
{
    public static class BoundsExtensions
    {
        public static LatLngBounds ToLatLngBounds(this Bounds self)
        {

            return new LatLngBounds(self.SouthWest.ToLatLng(), self.NorthEast.ToLatLng());
        }
    }
}
