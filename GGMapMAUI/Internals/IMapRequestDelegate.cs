using System;
namespace Maui.GoogleMaps.Internals
{
    public interface IMapRequestDelegate
    {
        void OnMoveToRegionRequest(MoveToRegionMessage m);
        void OnMoveCameraRequest(CameraUpdateMessage m);
    }
}
