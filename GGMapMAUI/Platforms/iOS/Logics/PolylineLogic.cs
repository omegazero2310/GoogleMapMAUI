using GGMapMAUI.Platforms.iOS.Extensions;
using Google.Maps;
using Maui.GoogleMaps.Logics;
using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NativePolyline = Google.Maps.Polyline;

namespace GGMapMAUI.Platforms.iOS.Logics
{
    public class PolylineLogic : DefaultPolylineLogic<NativePolyline, MapView>
    {
        protected override IList<Maui.GoogleMaps.Polyline> GetItems(Maui.GoogleMaps.Map map) => map.Polylines;

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

        protected override NativePolyline CreateNativeItem(Maui.GoogleMaps.Polyline outerItem)
        {
            var path = new MutablePath();
            foreach (var p in outerItem.Positions)
                path.AddLatLon(p.Latitude, p.Longitude);

            var nativePolyline = NativePolyline.FromPath(path);
            nativePolyline.StrokeWidth = outerItem.StrokeWidth;
            nativePolyline.StrokeColor = outerItem.StrokeColor.ToPlatform();
            nativePolyline.Tappable = outerItem.IsClickable;
            nativePolyline.ZIndex = outerItem.ZIndex;

            outerItem.NativeObject = nativePolyline;
            nativePolyline.Map = NativeMap;

            outerItem.SetOnPositionsChanged((polyline, e) =>
            {
                var native = polyline.NativeObject as NativePolyline;
                native.Path = polyline.Positions.ToMutablePath();
            });

            return nativePolyline;
        }

        protected override NativePolyline DeleteNativeItem(Maui.GoogleMaps.Polyline outerItem)
        {
            var nativePolyline = outerItem.NativeObject as NativePolyline;
            nativePolyline.Map = null;
            return nativePolyline;
        }

        void OnOverlayTapped(object sender, GMSOverlayEventEventArgs e)
        {
            var targetOuterItem = GetItems(Map).FirstOrDefault(
                outerItem => object.ReferenceEquals(outerItem.NativeObject, e.Overlay));
            targetOuterItem?.SendTap();
        }

        protected override void OnUpdateIsClickable(Maui.GoogleMaps.Polyline outerItem, NativePolyline nativeItem)
        {
            nativeItem.Tappable = outerItem.IsClickable;
        }

        protected override void OnUpdateStrokeColor(Maui.GoogleMaps.Polyline outerItem, NativePolyline nativeItem)
        {
            nativeItem.StrokeColor = outerItem.StrokeColor.ToPlatform();
        }

        protected override void OnUpdateStrokeWidth(Maui.GoogleMaps.Polyline outerItem, NativePolyline nativeItem)
        {
            nativeItem.StrokeWidth = outerItem.StrokeWidth;
        }

        protected override void OnUpdateZIndex(Maui.GoogleMaps.Polyline outerItem, NativePolyline nativeItem)
        {
            nativeItem.ZIndex = outerItem.ZIndex;
        }
    }
}
