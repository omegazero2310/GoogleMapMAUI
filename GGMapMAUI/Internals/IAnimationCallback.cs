using System;
namespace Maui.GoogleMaps.Internals
{
    public interface IAnimationCallback
    {
        void OnFinished();
        void OnCanceled();
    }
}
