using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using GGMapMAUI.Platforms.Android.Extensions;
using Maui.GoogleMaps.Logics;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using static Android.Gms.Maps.GoogleMap;
using NativePolygon = Android.Gms.Maps.Model.Polygon;

namespace GGMapMAUI.Platforms.Android.Logics
{
    public class PolygonLogic : DefaultPolygonLogic<NativePolygon, GoogleMap>
    {
        protected override IList<Maui.GoogleMaps.Polygon> GetItems(Maui.GoogleMaps.Map map) => map.Polygons;

        public override void Register(GoogleMap oldNativeMap, Maui.GoogleMaps.Map oldMap, GoogleMap newNativeMap, Maui.GoogleMaps.Map newMap)
        {
            base.Register(oldNativeMap, oldMap, newNativeMap, newMap);

            if (newNativeMap != null)
            {
                newNativeMap.PolygonClick += OnPolygonClick;
            }
        }

        public override void Unregister(GoogleMap nativeMap, Maui.GoogleMaps.Map map)
        {
            if (nativeMap != null)
            {
                nativeMap.PolygonClick -= OnPolygonClick;
            }

            base.Unregister(nativeMap, map);
        }

        protected override NativePolygon CreateNativeItem(Maui.GoogleMaps.Polygon outerItem)
        {
            var opts = new PolygonOptions();

            foreach (var p in outerItem.Positions)
                opts.Add(new LatLng(p.Latitude, p.Longitude));

            opts.InvokeStrokeWidth(outerItem.StrokeWidth * this.ScaledDensity); // TODO: convert from px to pt. Is this collect? (looks like same iOS Maps)
            opts.InvokeStrokeColor(outerItem.StrokeColor.ToAndroid());
            opts.InvokeFillColor(outerItem.FillColor.ToAndroid());
            opts.Clickable(outerItem.IsClickable);
            opts.InvokeZIndex(outerItem.ZIndex);

            foreach (var hole in outerItem.Holes)
            {
                opts.Holes.Add(hole.Select(x => x.ToLatLng()).ToJavaList());
            }

            var nativePolygon = NativeMap.AddPolygon(opts);

            // associate pin with marker for later lookup in event handlers
            outerItem.NativeObject = nativePolygon;
            outerItem.SetOnPositionsChanged((polygon, e) =>
            {
                var native = polygon.NativeObject as NativePolygon;
                native.Points = polygon.Positions.ToLatLngs();
            });

            outerItem.SetOnHolesChanged((polygon, e) =>
            {
                var native = polygon.NativeObject as NativePolygon;
                native.SetHoles((IList<IList<LatLng>>)polygon.Holes
                    .Select(x => (IList<LatLng>)x.Select(y => y.ToLatLng()).ToJavaList())
                                .ToJavaList());
            });

            return nativePolygon;
        }

        protected override NativePolygon DeleteNativeItem(Maui.GoogleMaps.Polygon outerItem)
        {
            outerItem.SetOnHolesChanged(null);
            outerItem.SetOnPositionsChanged(null);

            var nativePolygon = outerItem.NativeObject as NativePolygon;
            if (nativePolygon == null)
                return null;

            nativePolygon.Remove();
            outerItem.NativeObject = null;
            return nativePolygon;
        }

        void OnPolygonClick(object sender, PolygonClickEventArgs e)
        {
            // clicked polyline
            var nativeItem = e.Polygon;

            // lookup pin
            var targetOuterItem = GetItems(Map).FirstOrDefault(
                outerItem => ((NativePolygon)outerItem.NativeObject).Id == nativeItem.Id);

            // only consider event handled if a handler is present.
            // Else allow default behavior of displaying an info window.
            targetOuterItem?.SendTap();
        }

        protected override void OnUpdateIsClickable(Maui.GoogleMaps.Polygon outerItem, NativePolygon nativeItem)
        {
            nativeItem.Clickable = outerItem.IsClickable;
        }

        protected override void OnUpdateStrokeColor(Maui.GoogleMaps.Polygon outerItem, NativePolygon nativeItem)
        {
            nativeItem.StrokeColor = outerItem.StrokeColor.ToAndroid();
        }

        protected override void OnUpdateStrokeWidth(Maui.GoogleMaps.Polygon outerItem, NativePolygon nativeItem)
        {
            // TODO: convert from px to pt. Is this collect? (looks like same iOS Maps)
            nativeItem.StrokeWidth = outerItem.StrokeWidth * this.ScaledDensity;
        }

        protected override void OnUpdateFillColor(Maui.GoogleMaps.Polygon outerItem, NativePolygon nativeItem)
        {
            nativeItem.FillColor = outerItem.FillColor.ToAndroid();
        }

        protected override void OnUpdateZIndex(Maui.GoogleMaps.Polygon outerItem, NativePolygon nativeItem)
        {
            nativeItem.ZIndex = outerItem.ZIndex;
        }
    }
}
