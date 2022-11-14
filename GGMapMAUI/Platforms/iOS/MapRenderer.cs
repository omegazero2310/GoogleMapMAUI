using GGMapMAUI.Platforms.iOS.Extensions;
using GGMapMAUI.Platforms.iOS.Logics;
using Google.Maps;
using Maui.GoogleMaps;
using Maui.GoogleMaps.Internals;
using Maui.GoogleMaps.Logics;
using Microsoft.Maui.Controls.Compatibility.Platform.iOS;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Platform;
using System.ComponentModel;
using System.Drawing;
using UIKit;
using GCameraPosition = Google.Maps.CameraPosition;

namespace GGMapMAUI.Platforms.iOS
{
    public class MapRenderer : ViewRenderer
    {
        public static MauiContext MauiContext { get; set; }
        bool _shouldUpdateRegion = true;

        // ReSharper disable once MemberCanBePrivate.Global
        protected MapView NativeMap => (MapView)Control;
        // ReSharper disable once MemberCanBePrivate.Global
        protected Maui.GoogleMaps.Map Map => (Maui.GoogleMaps.Map)Element;

        protected internal static PlatformConfig Config { protected get; set; }

        readonly UiSettingsLogic _uiSettingsLogic = new UiSettingsLogic();
        readonly CameraLogic _cameraLogic;

        private bool _ready;

        internal readonly IList<BaseLogic<MapView>> Logics;

        public MapRenderer()
        {
            Logics = new List<BaseLogic<MapView>>
            {
                new PolylineLogic(),
                new PolygonLogic(),
                new CircleLogic(),
                new PinLogic(Config.ImageFactory, OnMarkerCreating, OnMarkerCreated, OnMarkerDeleting, OnMarkerDeleted),
                new TileLayerLogic(),
                new GroundOverlayLogic(Config.ImageFactory)
            };

            _cameraLogic = new CameraLogic(() =>
            {
                OnCameraPositionChanged(NativeMap.Camera);
            });
        }

        public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            return Control.GetSizeRequest(widthConstraint, heightConstraint);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Map != null)
                {
                    Map.OnSnapshot -= OnSnapshot;
                    foreach (var logic in Logics)
                    {
                        logic.Unregister(NativeMap, Map);
                    }
                }
                _cameraLogic.Unregister();
                _uiSettingsLogic.Unregister();

                var mkMapView = (MapView)Control;
                if (mkMapView != null)
                {
                    mkMapView.CoordinateLongPressed -= CoordinateLongPressed;
                    mkMapView.CoordinateTapped -= CoordinateTapped;
                    mkMapView.CameraPositionChanged -= CameraPositionChanged;
                    mkMapView.DidTapMyLocationButton = null;
                }
            }

            base.Dispose(disposing);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            // For XAML Previewer or FormsGoogleMaps.Init not called.
            if (!FormsGoogleMaps.IsInitialized)
            {
                var label = new UILabel()
                {
                    Text = "GGMapMAUI.GoogleMaps",
                    BackgroundColor = Microsoft.Maui.Graphics.Color.FromRgb(0, 125, 125).ToPlatform(),
                    TextColor = Microsoft.Maui.Graphics.Color.FromRgb(0, 0, 0).ToPlatform(),
                    TextAlignment = UITextAlignment.Center
                };
                SetNativeControl(label);
                return;
            }

            var oldMapView = (MapView)Control;
            if (e.OldElement != null)
            {
                var oldMapModel = (Maui.GoogleMaps.Map)e.OldElement;
                oldMapModel.OnSnapshot -= OnSnapshot;
                _cameraLogic.Unregister();

                if (oldMapView != null)
                {
                    oldMapView.CoordinateLongPressed -= CoordinateLongPressed;
                    oldMapView.CoordinateTapped -= CoordinateTapped;
                    oldMapView.CameraPositionChanged -= CameraPositionChanged;
                    oldMapView.DidTapMyLocationButton = null;
                }
            }

            if (e.NewElement != null)
            {
                var mapModel = (Maui.GoogleMaps.Map)e.NewElement;

                if (Control == null)
                {
                    SetNativeControl(new MapView(RectangleF.Empty));
                    var mkMapView = (MapView)Control;
                    mkMapView.CameraPositionChanged += CameraPositionChanged;
                    mkMapView.CoordinateTapped += CoordinateTapped;
                    mkMapView.CoordinateLongPressed += CoordinateLongPressed;
                    mkMapView.DidTapMyLocationButton = DidTapMyLocation;
                }

                _cameraLogic.Register(Map, NativeMap);
                Map.OnSnapshot += OnSnapshot;

                //_cameraLogic.MoveCamera(mapModel.InitialCameraUpdate);
                //_ready = true;

                _uiSettingsLogic.Register(Map, NativeMap);
                UpdateMapType();
                UpdateIsShowingUser(_uiSettingsLogic.MyLocationButtonEnabled);
                UpdateHasScrollEnabled(_uiSettingsLogic.ScrollGesturesEnabled);
                UpdateHasZoomEnabled(_uiSettingsLogic.ZoomGesturesEnabled);
                UpdateHasRotationEnabled(_uiSettingsLogic.RotateGesturesEnabled);
                UpdateIsTrafficEnabled();
                UpdatePadding();
                UpdateMapStyle();
                UpdateMyLocationEnabled();
                _uiSettingsLogic.Initialize();

                foreach (var logic in Logics)
                {
                    logic.Register(oldMapView, (Maui.GoogleMaps.Map)e.OldElement, NativeMap, Map);
                    logic.RestoreItems();
                    logic.OnMapPropertyChanged(new PropertyChangedEventArgs(Maui.GoogleMaps.Map.SelectedPinProperty.PropertyName));
                }

            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            // For XAML Previewer or FormsGoogleMaps.Init not called.
            if (!FormsGoogleMaps.IsInitialized)
            {
                return;
            }

            if (e.PropertyName == Maui.GoogleMaps.Map.MapTypeProperty.PropertyName)
            {
                UpdateMapType();
            }
            else if (e.PropertyName == Maui.GoogleMaps.Map.IsShowingUserProperty.PropertyName)
            {
                UpdateIsShowingUser();
            }
            else if (e.PropertyName == Maui.GoogleMaps.Map.MyLocationEnabledProperty.PropertyName)
            {
                UpdateMyLocationEnabled();
            }
            else if (e.PropertyName == Maui.GoogleMaps.Map.HasScrollEnabledProperty.PropertyName)
            {
                UpdateHasScrollEnabled();
            }
            else if (e.PropertyName == Maui.GoogleMaps.Map.HasRotationEnabledProperty.PropertyName)
            {
                UpdateHasRotationEnabled();
            }
            else if (e.PropertyName == Maui.GoogleMaps.Map.HasZoomEnabledProperty.PropertyName)
            {
                UpdateHasZoomEnabled();
            }
            else if (e.PropertyName == Maui.GoogleMaps.Map.IsTrafficEnabledProperty.PropertyName)
            {
                UpdateIsTrafficEnabled();
            }
            else if (e.PropertyName == VisualElement.HeightProperty.PropertyName &&
                     ((Maui.GoogleMaps.Map)Element).InitialCameraUpdate != null)
            {
                _shouldUpdateRegion = true;
            }
            else if (e.PropertyName == Maui.GoogleMaps.Map.IndoorEnabledProperty.PropertyName)
            {
                UpdateHasIndoorEnabled();
            }
            else if (e.PropertyName == Maui.GoogleMaps.Map.PaddingProperty.PropertyName)
            {
                UpdatePadding();
            }
            else if (e.PropertyName == Maui.GoogleMaps.Map.MapStyleProperty.PropertyName)
            {
                UpdateMapStyle();
            }

            foreach (var logic in Logics)
            {
                logic.OnMapPropertyChanged(e);
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            // For XAML Previewer or FormsGoogleMaps.Init not called.
            if (!FormsGoogleMaps.IsInitialized)
            {
                return;
            }

            if (_shouldUpdateRegion && !_ready)
            {
                _cameraLogic.MoveCamera(((Maui.GoogleMaps.Map)Element).InitialCameraUpdate);
                _ready = true;
                _shouldUpdateRegion = false;
            }

        }

        void OnSnapshot(TakeSnapshotMessage snapshotMessage)
        {
            UIGraphics.BeginImageContextWithOptions(NativeMap.Frame.Size, false, 0f);
            NativeMap.Layer.RenderInContext(UIGraphics.GetCurrentContext());
            var snapshot = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            // Why using task? Because Android side is asynchronous. 
            Task.Run(() =>
            {
                snapshotMessage.OnSnapshot.Invoke(snapshot.AsPNG().AsStream());
            });
        }

        protected void CameraPositionChanged(object sender, GMSCameraEventArgs args)
        {
            OnCameraPositionChanged(args.Position);
        }

        void OnCameraPositionChanged(GCameraPosition pos)
        {
            if (Element == null)
                return;

            var mapModel = (Maui.GoogleMaps.Map)Element;
            var mkMapView = (MapView)Control;

            var region = mkMapView.Projection.VisibleRegion;
            var minLat = Math.Min(Math.Min(Math.Min(region.NearLeft.Latitude, region.NearRight.Latitude), region.FarLeft.Latitude), region.FarRight.Latitude);
            var minLon = Math.Min(Math.Min(Math.Min(region.NearLeft.Longitude, region.NearRight.Longitude), region.FarLeft.Longitude), region.FarRight.Longitude);
            var maxLat = Math.Max(Math.Max(Math.Max(region.NearLeft.Latitude, region.NearRight.Latitude), region.FarLeft.Latitude), region.FarRight.Latitude);
            var maxLon = Math.Max(Math.Max(Math.Max(region.NearLeft.Longitude, region.NearRight.Longitude), region.FarLeft.Longitude), region.FarRight.Longitude);

#pragma warning disable 618
            mapModel.VisibleRegion = new MapSpan(pos.Target.ToPosition(), maxLat - minLat, maxLon - minLon);
#pragma warning restore 618

            Map.Region = mkMapView.Projection.VisibleRegion.ToRegion();

            var camera = pos.ToXamarinForms();
            Map.CameraPosition = camera;
            Map.SendCameraChanged(camera);
        }

        protected void CoordinateTapped(object sender, GMSCoordEventArgs e)
        {
            Map.SendMapClicked(e.Coordinate.ToPosition());
        }

        protected void CoordinateLongPressed(object sender, GMSCoordEventArgs e)
        {
            Map.SendMapLongClicked(e.Coordinate.ToPosition());
        }

        bool DidTapMyLocation(MapView mapView)
        {
            return Map.SendMyLocationClicked();
        }

        private void UpdateHasScrollEnabled(bool? initialScrollGesturesEnabled = null)
        {
#pragma warning disable 618
            NativeMap.Settings.ScrollGestures = initialScrollGesturesEnabled ?? ((Maui.GoogleMaps.Map)Element).HasScrollEnabled;
#pragma warning restore 618
        }

        private void UpdateHasZoomEnabled(bool? initialZoomGesturesEnabled = null)
        {
#pragma warning disable 618
            NativeMap.Settings.ZoomGestures = initialZoomGesturesEnabled ?? ((Maui.GoogleMaps.Map)Element).HasZoomEnabled;
#pragma warning restore 618
        }

        private void UpdateHasRotationEnabled(bool? initialRotateGesturesEnabled = null)
        {
#pragma warning disable 618
            NativeMap.Settings.RotateGestures = initialRotateGesturesEnabled ?? ((Maui.GoogleMaps.Map)Element).HasRotationEnabled;
#pragma warning restore 618
        }

        private void UpdateIsShowingUser(bool? initialMyLocationButtonEnabled = null)
        {
#pragma warning disable 618
            ((MapView)Control).MyLocationEnabled = ((Maui.GoogleMaps.Map)Element).IsShowingUser;
            ((MapView)Control).Settings.MyLocationButton = initialMyLocationButtonEnabled ?? ((Maui.GoogleMaps.Map)Element).IsShowingUser;
#pragma warning restore 618
        }

        void UpdateMyLocationEnabled()
        {
            ((MapView)Control).MyLocationEnabled = ((Maui.GoogleMaps.Map)Element).MyLocationEnabled;
        }

        void UpdateIsTrafficEnabled()
        {
            ((MapView)Control).TrafficEnabled = ((Maui.GoogleMaps.Map)Element).IsTrafficEnabled;
        }

        void UpdateHasIndoorEnabled()
        {
            ((MapView)Control).IndoorEnabled = ((Maui.GoogleMaps.Map)Element).IsIndoorEnabled;
        }

        void UpdateMapType()
        {
            switch (((Maui.GoogleMaps.Map)Element).MapType)
            {
                case MapType.Street:
                    ((MapView)Control).MapType = MapViewType.Normal;
                    break;
                case MapType.Satellite:
                    ((MapView)Control).MapType = MapViewType.Satellite;
                    break;
                case MapType.Hybrid:
                    ((MapView)Control).MapType = MapViewType.Hybrid;
                    break;
                case MapType.Terrain:
                    ((MapView)Control).MapType = MapViewType.Terrain;
                    break;
                case MapType.None:
                    ((MapView)Control).MapType = MapViewType.None;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void UpdatePadding()
        {
            ((MapView)Control).Padding = ((Maui.GoogleMaps.Map)Element).Padding.ToUIEdgeInsets();
        }

        void UpdateMapStyle()
        {
            if (Map.MapStyle == null)
            {
                ((MapView)Control).MapStyle = null;
            }
            else
            {
                var mapStyle = Google.Maps.MapStyle.FromJson(Map.MapStyle.JsonStyle, null);
                ((MapView)Control).MapStyle = mapStyle;
            }
        }

        #region Overridable Members

        /// <summary>
        /// Call when before marker create.
        /// You can override your custom renderer for customize marker.
        /// </summary>
        /// <param name="outerItem">the pin.</param>
        /// <param name="innerItem">the marker options.</param>
        protected virtual void OnMarkerCreating(Pin outerItem, Marker innerItem)
        {
        }

        /// <summary>
        /// Call when after marker create.
        /// You can override your custom renderer for customize marker.
        /// </summary>
        /// <param name="outerItem">the pin.</param>
        /// <param name="innerItem">thr marker.</param>
        protected virtual void OnMarkerCreated(Pin outerItem, Marker innerItem)
        {
        }

        /// <summary>
        /// Call when before marker delete.
        /// You can override your custom renderer for customize marker.
        /// </summary>
        /// <param name="outerItem">the pin.</param>
        /// <param name="innerItem">thr marker.</param>
        protected virtual void OnMarkerDeleting(Pin outerItem, Marker innerItem)
        {
        }

        /// <summary>
        /// Call when after marker delete.
        /// You can override your custom renderer for customize marker.
        /// </summary>
        /// <param name="outerItem">the pin.</param>
        /// <param name="innerItem">thr marker.</param>
        protected virtual void OnMarkerDeleted(Pin outerItem, Marker innerItem)
        {
        }

        #endregion    
    }
}
