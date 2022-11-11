using Android.Gms.Maps.Model;
using Maui.GoogleMaps;

namespace GGMapMAUI.Platforms.Android.Extensions
{
    public static class VisibleRegionExtensions
    {
        public static MapRegion ToRegion(this VisibleRegion visibleRegion)
        {
            return new MapRegion(
                visibleRegion.NearLeft.ToPosition(),
                visibleRegion.NearRight.ToPosition(),
                visibleRegion.FarLeft.ToPosition(),
                visibleRegion.FarRight.ToPosition()
            );
        }
    }
}
