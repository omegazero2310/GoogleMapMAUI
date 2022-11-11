using Android.Gms.Maps.Model;
using Maui.GoogleMaps;

namespace GGMapMAUI.Platforms.Android.Extensions
{
    public static class PositionExtensions
    {
        public static LatLng ToLatLng(this Position self)
        {
            return new LatLng(self.Latitude, self.Longitude);
        }

        public static IList<LatLng> ToLatLngs(this IList<Position> self)
        {
            return self.Select(ToLatLng).ToList();
        }
    }
}
