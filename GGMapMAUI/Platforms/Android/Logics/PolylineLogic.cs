
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using GGMapMAUI.Platforms.Android.Extensions;
using Maui.GoogleMaps;
using Maui.GoogleMaps.Logics;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using NativePolyline = Android.Gms.Maps.Model.Polyline;

namespace GGMapMAUI.Platforms.Android.Logics
{
    public class PolylineLogic : DefaultPolylineLogic<NativePolyline, GoogleMap>
    {
        protected override IList<Maui.GoogleMaps.Polyline> GetItems(Maui.GoogleMaps.Map map) => map.Polylines;

        public override void Register(GoogleMap oldNativeMap, Maui.GoogleMaps.Map oldMap, GoogleMap newNativeMap, Maui.GoogleMaps.Map newMap)
        {
            base.Register(oldNativeMap, oldMap, newNativeMap, newMap);

            if (newNativeMap != null)
            {
                newNativeMap.PolylineClick += OnPolylineClick;
            }
        }

        public override void Unregister(GoogleMap nativeMap, Maui.GoogleMaps.Map map)
        {
            if (nativeMap != null)
            {
                nativeMap.PolylineClick -= OnPolylineClick;
            }

            base.Unregister(nativeMap, map);
        }
        protected override NativePolyline CreateNativeItem(Maui.GoogleMaps.Polyline outerItem)
        {
            var opts = new PolylineOptions();

            foreach (var p in outerItem.Positions)
                opts.Add(new LatLng(p.Latitude, p.Longitude));

            opts.InvokeWidth(outerItem.StrokeWidth * this.ScaledDensity); // TODO: convert from px to pt. Is this collect? (looks like same iOS Maps)
            opts.InvokeColor(outerItem.StrokeColor.ToAndroid());
            opts.Clickable(outerItem.IsClickable);
            opts.InvokeZIndex(outerItem.ZIndex);

            var nativePolyline = NativeMap.AddPolyline(opts);

            // associate pin with marker for later lookup in event handlers
            outerItem.NativeObject = nativePolyline;
            outerItem.SetOnPositionsChanged((polyline, e) =>
            {
                var native = polyline.NativeObject as NativePolyline;
                native.Points = polyline.Positions.ToLatLngs();
            });

            return nativePolyline;
        }
        protected override NativePolyline DeleteNativeItem(Maui.GoogleMaps.Polyline outerItem)
        {
            outerItem.SetOnPositionsChanged(null);

            var nativeShape = outerItem.NativeObject as NativePolyline;
            if (nativeShape == null)
                return null;

            nativeShape.Remove();
            outerItem.NativeObject = null;
            return nativeShape;
        }

        void OnPolylineClick(object sender, GoogleMap.PolylineClickEventArgs e)
        {
            // clicked polyline
            var nativeItem = e.Polyline;

            // lookup pin
            var targetOuterItem = GetItems(Map).FirstOrDefault(
                outerItem => ((NativePolyline)outerItem.NativeObject).Id == nativeItem.Id);

            // only consider event handled if a handler is present.
            // Else allow default behavior of displaying an info window.
            targetOuterItem?.SendTap();
        }

        protected override void OnUpdateIsClickable(Maui.GoogleMaps.Polyline outerItem, NativePolyline nativeItem)
        {
            nativeItem.Clickable = outerItem.IsClickable;
        }

        protected override void OnUpdateStrokeColor(Maui.GoogleMaps.Polyline outerItem, NativePolyline nativeItem)
        {
            nativeItem.Color = outerItem.StrokeColor.ToAndroid();
        }

        protected override void OnUpdateStrokeWidth(Maui.GoogleMaps.Polyline outerItem, NativePolyline nativeItem)
        {
            nativeItem.Width = outerItem.StrokeWidth;
        }

        protected override void OnUpdateZIndex(Maui.GoogleMaps.Polyline outerItem, NativePolyline nativeItem)
        {
            nativeItem.ZIndex = outerItem.ZIndex;
        }
    }
}
