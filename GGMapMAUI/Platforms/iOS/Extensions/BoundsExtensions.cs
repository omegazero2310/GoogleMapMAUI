using Google.Maps;
using Maui.GoogleMaps;

namespace GGMapMAUI.Platforms.iOS.Extensions
{
    internal static class BoundsExtensions
    {
        public static CoordinateBounds ToCoordinateBounds(this Bounds self)
        {
            return new CoordinateBounds(self.SouthWest.ToCoord(), self.NorthEast.ToCoord());
        }
    }
}
