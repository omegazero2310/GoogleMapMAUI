using GGMapMAUI.Platforms.iOS.Extensions;
using Google.Maps;
using Maui.GoogleMaps.Logics;
using Microsoft.Maui.Platform;
using NativePolygon = Google.Maps.Polygon;

namespace GGMapMAUI.Platforms.iOS.Logics
{
    public class PolygonLogic : DefaultPolygonLogic<NativePolygon, MapView>
    {
        protected override IList<Maui.GoogleMaps.Polygon> GetItems(Maui.GoogleMaps.Map map) => map.Polygons;

        public override void Register(MapView oldNativeMap, Maui.GoogleMaps.Map oldMap, MapView newNativeMap, Maui.GoogleMaps.Map newMap)
        {
            base.Register(oldNativeMap, oldMap, newNativeMap, newMap);

            if (newNativeMap != null)
            {
                newNativeMap.OverlayTapped += OnOverlayTapped;
            }
        }

        public override void Unregister(MapView nativeMap, Maui.GoogleMaps.Map map)
        {
            if (nativeMap != null)
            {
                nativeMap.OverlayTapped -= OnOverlayTapped;
            }

            base.Unregister(nativeMap, map);
        }

        protected override NativePolygon CreateNativeItem(Maui.GoogleMaps.Polygon outerItem)
        {
            var nativePolygon = NativePolygon.FromPath(outerItem.Positions.ToMutablePath());
            nativePolygon.StrokeWidth = outerItem.StrokeWidth;
            nativePolygon.StrokeColor = outerItem.StrokeColor.ToPlatform();
            nativePolygon.FillColor = outerItem.FillColor.ToPlatform();
            nativePolygon.Tappable = outerItem.IsClickable;
            nativePolygon.ZIndex = outerItem.ZIndex;

            nativePolygon.Holes = outerItem.Holes
                .Select(hole => hole.ToMutablePath())
                .ToArray();

            outerItem.NativeObject = nativePolygon;
            nativePolygon.Map = NativeMap;

            outerItem.SetOnPositionsChanged((polygon, e) =>
            {
                var native = polygon.NativeObject as NativePolygon;
                native.Path = polygon.Positions.ToMutablePath();
            });

            outerItem.SetOnHolesChanged((polygon, e) =>
            {
                var native = polygon.NativeObject as NativePolygon;
                native.Holes = outerItem.Holes
                    .Select(hole => hole.ToMutablePath())
                    .ToArray();
            });

            return nativePolygon;
        }

        protected override NativePolygon DeleteNativeItem(Maui.GoogleMaps.Polygon outerItem)
        {
            outerItem.SetOnHolesChanged(null);

            var nativePolygon = outerItem.NativeObject as NativePolygon;
            nativePolygon.Map = null;
            return nativePolygon;
        }

        void OnOverlayTapped(object sender, GMSOverlayEventEventArgs e)
        {
            var targetOuterItem = GetItems(Map).FirstOrDefault(
                outerItem => object.ReferenceEquals(outerItem.NativeObject, e.Overlay));
            targetOuterItem?.SendTap();
        }

        protected override void OnUpdateIsClickable(Maui.GoogleMaps.Polygon outerItem, NativePolygon nativeItem)
        {
            nativeItem.Tappable = outerItem.IsClickable;
        }

        protected override void OnUpdateStrokeColor(Maui.GoogleMaps.Polygon outerItem, NativePolygon nativeItem)
        {
            nativeItem.StrokeColor = outerItem.StrokeColor.ToPlatform();
        }

        protected override void OnUpdateStrokeWidth(Maui.GoogleMaps.Polygon outerItem, NativePolygon nativeItem)
        {
            nativeItem.StrokeWidth = outerItem.StrokeWidth;
        }

        protected override void OnUpdateFillColor(Maui.GoogleMaps.Polygon outerItem, NativePolygon nativeItem)
        {
            nativeItem.FillColor = outerItem.FillColor.ToPlatform();
        }

        protected override void OnUpdateZIndex(Maui.GoogleMaps.Polygon outerItem, NativePolygon nativeItem)
        {
            nativeItem.ZIndex = outerItem.ZIndex;
        }
    }
}
