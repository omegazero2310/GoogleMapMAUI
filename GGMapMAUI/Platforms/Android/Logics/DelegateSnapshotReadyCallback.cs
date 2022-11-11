using Android.Gms.Maps;
using Android.Graphics;

namespace GGMapMAUI.Platforms.Android.Logics
{
    public sealed class DelegateSnapshotReadyCallback : Java.Lang.Object, GoogleMap.ISnapshotReadyCallback
    {
        private readonly Action<Bitmap> _handler;

        public DelegateSnapshotReadyCallback(Action<Bitmap> handler)
        {
            _handler = handler;
        }

        public void OnSnapshotReady(Bitmap snapshot)
        {
            _handler?.Invoke(snapshot);
        }
    }
}
