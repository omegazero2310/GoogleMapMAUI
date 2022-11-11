using Foundation;
using Google.Maps;
using Maui.GoogleMaps.Logics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NativeTileLayer = Google.Maps.TileLayer;
using NativeUrlTileLayer = Google.Maps.UrlTileLayer;

namespace GGMapMAUI.Platforms.iOS.Logics
{
    public class TileLayerLogic : DefaultLogic<Maui.GoogleMaps.TileLayer, NativeTileLayer, MapView>
    {
        protected override IList<Maui.GoogleMaps.TileLayer> GetItems(Maui.GoogleMaps.Map map) => map.TileLayers;

        protected override NativeTileLayer CreateNativeItem(Maui.GoogleMaps.TileLayer outerItem)
        {
            NativeTileLayer nativeTileLayer;

            if (outerItem.MakeTileUri != null)
            {
                nativeTileLayer = NativeUrlTileLayer.FromUrlConstructor((nuint x, nuint y, nuint zoom) =>
                {
                    var uri = outerItem.MakeTileUri((int)x, (int)y, (int)zoom);
                    return new NSUrl(uri.AbsoluteUri);
                });
                nativeTileLayer.TileSize = (nint)outerItem.TileSize;
            }
            else if (outerItem.TileImageSync != null)
            {
                nativeTileLayer = new TouchSyncTileLayer(outerItem.TileImageSync);
                nativeTileLayer.TileSize = (nint)outerItem.TileSize;
            }
            else
            {
                nativeTileLayer = new TouchAsyncTileLayer(outerItem.TileImageAsync);
                nativeTileLayer.TileSize = (nint)outerItem.TileSize;
            }

            nativeTileLayer.ZIndex = outerItem.ZIndex;

            outerItem.NativeObject = nativeTileLayer;
            nativeTileLayer.Map = NativeMap;

            return nativeTileLayer;
        }

        protected override NativeTileLayer DeleteNativeItem(Maui.GoogleMaps.TileLayer outerItem)
        {
            var nativeTileLayer = outerItem.NativeObject as NativeTileLayer;
            nativeTileLayer.Map = null;
            return nativeTileLayer;
        }

        protected override void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnItemPropertyChanged(sender, e);
            var outerItem = sender as Maui.GoogleMaps.TileLayer;
            var nativeItem = outerItem?.NativeObject as NativeTileLayer;

            if (nativeItem == null)
                return;

            if (e.PropertyName == Maui.GoogleMaps.TileLayer.ZIndexProperty.PropertyName) OnUpdateZIndex(outerItem, nativeItem);
        }

        private void OnUpdateZIndex(Maui.GoogleMaps.TileLayer outerItem, NativeTileLayer nativeItem)
        {
            nativeItem.ZIndex = outerItem.ZIndex;
        }

    }
}
