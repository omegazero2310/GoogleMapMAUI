using CoreGraphics;
using GGMapMAUI.Platforms.iOS.Extensions;
using GGMapMAUI.Platforms.iOS.Factories;
using Google.Maps;
using Maui.GoogleMaps;
using Maui.GoogleMaps.Logics;
using Microsoft.Maui.Platform;
using UIKit;
using NativeGroundOverlay = Google.Maps.GroundOverlay;

namespace GGMapMAUI.Platforms.iOS.Logics
{
    public class GroundOverlayLogic : DefaultGroundOverlayLogic<NativeGroundOverlay, MapView>
    {
        private readonly IImageFactory _imageFactory;

        protected override IList<Maui.GoogleMaps.GroundOverlay> GetItems(Maui.GoogleMaps.Map map) => map.GroundOverlays;

        public GroundOverlayLogic(IImageFactory imageFactory)
        {
            _imageFactory = imageFactory;
        }

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

        protected override NativeGroundOverlay CreateNativeItem(Maui.GoogleMaps.GroundOverlay outerItem)
        {
            var factory = _imageFactory ?? DefaultImageFactory.Instance;
            var nativeOverlay = NativeGroundOverlay.GetGroundOverlay(
                outerItem.Bounds.ToCoordinateBounds(), factory.ToUIImage(outerItem.Icon));
            nativeOverlay.Bearing = outerItem.Bearing;
            nativeOverlay.Opacity = 1 - outerItem.Transparency;
            nativeOverlay.Tappable = outerItem.IsClickable;
            nativeOverlay.ZIndex = outerItem.ZIndex;

            if (outerItem.Icon != null)
            {
                nativeOverlay.Icon = factory.ToUIImage(outerItem.Icon);
            }

            outerItem.NativeObject = nativeOverlay;
            nativeOverlay.Map = outerItem.IsVisible ? NativeMap : null;

            OnUpdateIconView(outerItem, nativeOverlay);

            return nativeOverlay;
        }

        protected override NativeGroundOverlay DeleteNativeItem(Maui.GoogleMaps.GroundOverlay outerItem)
        {
            var nativeOverlay = outerItem.NativeObject as NativeGroundOverlay;
            nativeOverlay.Map = null;

            return nativeOverlay;
        }

        void OnOverlayTapped(object sender, GMSOverlayEventEventArgs e)
        {
            var targetOuterItem = GetItems(Map).FirstOrDefault(
                outerItem => object.ReferenceEquals(outerItem.NativeObject, e.Overlay));
            targetOuterItem?.SendTap();
        }

        protected override void OnUpdateBearing(Maui.GoogleMaps.GroundOverlay outerItem, NativeGroundOverlay nativeItem)
        {
            nativeItem.Bearing = outerItem.Bearing;
        }

        protected override void OnUpdateBounds(Maui.GoogleMaps.GroundOverlay outerItem, NativeGroundOverlay nativeItem)
        {
            nativeItem.Bounds = outerItem.Bounds.ToCoordinateBounds();
        }

        protected override void OnUpdateIcon(Maui.GoogleMaps.GroundOverlay outerItem, NativeGroundOverlay nativeItem)
        {
            if (outerItem.Icon.Type == BitmapDescriptorType.View)
            {
                OnUpdateIconView(outerItem, nativeItem);
            }
            else
            {
                var factory = _imageFactory ?? DefaultImageFactory.Instance;
                nativeItem.Icon = factory.ToUIImage(outerItem.Icon);
            }
        }

        protected override void OnUpdateIsClickable(Maui.GoogleMaps.GroundOverlay outerItem, NativeGroundOverlay nativeItem)
        {
            nativeItem.Tappable = outerItem.IsClickable;
        }

        protected override void OnUpdateTransparency(Maui.GoogleMaps.GroundOverlay outerItem, NativeGroundOverlay nativeItem)
        {
            nativeItem.Opacity = 1f - outerItem.Transparency;
        }

        protected override void OnUpdateZIndex(Maui.GoogleMaps.GroundOverlay outerItem, NativeGroundOverlay nativeItem)
        {
            nativeItem.ZIndex = outerItem.ZIndex;
        }

        protected void OnUpdateIconView(Maui.GoogleMaps.GroundOverlay outerItem, NativeGroundOverlay nativeItem)
        {
            if (outerItem?.Icon?.Type == BitmapDescriptorType.View && outerItem?.Icon?.View != null)
            {
                NativeMap.InvokeOnMainThread(() =>
                {
                    var iconView = outerItem.Icon.View;
                    var nativeView = iconView.ToPlatform(iconView.Handler?.MauiContext ?? MapRenderer.MauiContext);
                    nativeView.BackgroundColor = UIColor.Clear;
                    //nativeItem.GroundAnchor = new CGPoint(iconView.AnchorX, iconView.AnchorY);
                    nativeItem.Icon = Utils.ConvertViewToImage(nativeView);

                    // Would have been way cooler to do this instead, but surprisingly, we can't do this on Android:
                    // nativeItem.IconView = nativeView;
                });
            }
        }
    }
}
