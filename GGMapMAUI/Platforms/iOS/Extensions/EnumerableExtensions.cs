﻿using Google.Maps;
using Maui.GoogleMaps;

namespace GGMapMAUI.Platforms.iOS.Extensions
{
    internal static class EnumerableExtensions
    {
        public static MutablePath ToMutablePath(this IEnumerable<Position> self)
        {
            var path = new MutablePath();
            foreach (var p in self)
            {
                path.AddLatLon(p.Latitude, p.Longitude);
            }
            return path;
        }
    }
}
