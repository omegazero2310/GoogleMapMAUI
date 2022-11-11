using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using GGMapMAUI.Platforms.Android.Extensions;
using Maui.GoogleMaps.Logics;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using NativeCircle = Android.Gms.Maps.Model.Circle;

namespace GGMapMAUI.Platforms.Android.Logics
{
    public class CircleLogic : DefaultCircleLogic<NativeCircle, GoogleMap>
    {
        public override void Register(GoogleMap oldNativeMap, Maui.GoogleMaps.Map oldMap, GoogleMap newNativeMap, Maui.GoogleMaps.Map newMap)
        {
            base.Register(oldNativeMap, oldMap, newNativeMap, newMap);

            if (newNativeMap != null)
            {
                newNativeMap.CircleClick += OnCircleClick;
            }
        }

        public override void Unregister(GoogleMap nativeMap, Maui.GoogleMaps.Map map)
        {
            if (nativeMap != null)
            {
                nativeMap.CircleClick -= OnCircleClick;
            }

            base.Unregister(nativeMap, map);
        }

        protected override IList<Maui.GoogleMaps.Circle> GetItems(Maui.GoogleMaps.Map map)
        {
            return map.Circles;
        }

        protected override NativeCircle CreateNativeItem(Maui.GoogleMaps.Circle outerItem)
        {
            var opts = new CircleOptions();

            opts.InvokeCenter(new LatLng(outerItem.Center.Latitude, outerItem.Center.Longitude));
            opts.InvokeRadius(outerItem.Radius.Meters);
            opts.InvokeStrokeWidth(outerItem.StrokeWidth * this.ScaledDensity); // TODO: convert from px to pt. Is this collect? (looks like same iOS Maps)
            opts.InvokeStrokeColor(outerItem.StrokeColor.ToAndroid());
            opts.InvokeFillColor(outerItem.FillColor.ToAndroid());
            opts.Clickable(outerItem.IsClickable);
            opts.InvokeZIndex(outerItem.ZIndex);

            var nativeCircle = NativeMap.AddCircle(opts);

            // associate pin with marker for later lookup in event handlers
            outerItem.NativeObject = nativeCircle;
            return nativeCircle;
        }

        protected override NativeCircle DeleteNativeItem(Maui.GoogleMaps.Circle outerItem)
        {
            var nativeCircle = outerItem.NativeObject as NativeCircle;
            if (nativeCircle == null)
                return null;
            nativeCircle.Remove();
            return nativeCircle;
        }

        private void OnCircleClick(object sender, GoogleMap.CircleClickEventArgs e)
        {
            // clicked circle
            var nativeItem = e.Circle;

            // lookup circle
            var targetOuterItem = GetItems(Map).FirstOrDefault(
                outerItem => ((NativeCircle)outerItem.NativeObject).Id == nativeItem.Id);

            // only consider event handled if a handler is present.
            // Else allow default behavior of displaying an info window.
            targetOuterItem?.SendTap();
        }

        protected override void OnUpdateStrokeWidth(Maui.GoogleMaps.Circle outerItem, NativeCircle nativeItem)
        {
            nativeItem.StrokeWidth = outerItem.StrokeWidth;
        }
        protected override void OnUpdateStrokeColor(Maui.GoogleMaps.Circle outerItem, NativeCircle nativeItem)
        {
            nativeItem.StrokeColor = outerItem.StrokeColor.ToAndroid();
        }
        protected override void OnUpdateFillColor(Maui.GoogleMaps.Circle outerItem, NativeCircle nativeItem)
        {
            nativeItem.FillColor = outerItem.FillColor.ToAndroid();
        }
        protected override void OnUpdateCenter(Maui.GoogleMaps.Circle outerItem, NativeCircle nativeItem)
        {
            nativeItem.Center = outerItem.Center.ToLatLng();
        }
        protected override void OnUpdateRadius(Maui.GoogleMaps.Circle outerItem, NativeCircle nativeItem)
        {
            nativeItem.Radius = outerItem.Radius.Meters;
        }
        protected override void OnUpdateIsClickable(Maui.GoogleMaps.Circle outerItem, NativeCircle nativeItem)
        {
            nativeItem.Clickable = outerItem.IsClickable;
        }

        protected override void OnUpdateZIndex(Maui.GoogleMaps.Circle outerItem, NativeCircle nativeItem)
        {
            nativeItem.ZIndex = outerItem.ZIndex;
        }
    }
}
