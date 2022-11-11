using GGMapMAUI.Platforms.iOS.Extensions;
using Google.Maps;
using Maui.GoogleMaps.Logics;
using Microsoft.Maui.Platform;
using NativeCircle = Google.Maps.Circle;

namespace GGMapMAUI.Platforms.iOS.Logics
{
    internal class CircleLogic : DefaultCircleLogic<NativeCircle, MapView>
    {
        protected override IList<Maui.GoogleMaps.Circle> GetItems(Maui.GoogleMaps.Map map) => map.Circles;

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

        protected override NativeCircle CreateNativeItem(Maui.GoogleMaps.Circle outerItem)
        {
            var nativeCircle = NativeCircle.FromPosition(
                outerItem.Center.ToCoord(), outerItem.Radius.Meters);
            nativeCircle.StrokeWidth = outerItem.StrokeWidth;
            nativeCircle.StrokeColor = outerItem.StrokeColor.ToPlatform();
            nativeCircle.FillColor = outerItem.FillColor.ToPlatform();
            nativeCircle.Tappable = outerItem.IsClickable;
            nativeCircle.ZIndex = outerItem.ZIndex;

            outerItem.NativeObject = nativeCircle;
            nativeCircle.Map = NativeMap;
            return nativeCircle;
        }

        protected override NativeCircle DeleteNativeItem(Maui.GoogleMaps.Circle outerItem)
        {
            var nativeCircle = outerItem.NativeObject as NativeCircle;
            nativeCircle.Map = null;
            return nativeCircle;
        }

        void OnOverlayTapped(object sender, GMSOverlayEventEventArgs e)
        {
            var targetOuterItem = GetItems(Map).FirstOrDefault(
                outerItem => object.ReferenceEquals(outerItem.NativeObject, e.Overlay));
            targetOuterItem?.SendTap();
        }

        protected override void OnUpdateStrokeWidth(Maui.GoogleMaps.Circle outerItem, NativeCircle nativeItem)
            => nativeItem.StrokeWidth = outerItem.StrokeWidth;

        protected override void OnUpdateStrokeColor(Maui.GoogleMaps.Circle outerItem, NativeCircle nativeItem)
            => nativeItem.StrokeColor = outerItem.StrokeColor.ToPlatform();

        protected override void OnUpdateFillColor(Maui.GoogleMaps.Circle outerItem, NativeCircle nativeItem)
            => nativeItem.FillColor = outerItem.FillColor.ToPlatform();

        protected override void OnUpdateCenter(Maui.GoogleMaps.Circle outerItem, NativeCircle nativeItem)
            => nativeItem.Position = outerItem.Center.ToCoord();

        protected override void OnUpdateRadius(Maui.GoogleMaps.Circle outerItem, NativeCircle nativeItem)
            => nativeItem.Radius = outerItem.Radius.Meters;

        protected override void OnUpdateIsClickable(Maui.GoogleMaps.Circle outerItem, NativeCircle nativeItem)
            => nativeItem.Tappable = outerItem.IsClickable;

        protected override void OnUpdateZIndex(Maui.GoogleMaps.Circle outerItem, NativeCircle nativeItem)
            => nativeItem.ZIndex = outerItem.ZIndex;
    }
}
