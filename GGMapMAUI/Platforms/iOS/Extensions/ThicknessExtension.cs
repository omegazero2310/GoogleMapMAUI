using UIKit;

namespace GGMapMAUI.Platforms.iOS.Extensions
{
    internal static class ThicknessExtension
    {
        public static UIEdgeInsets ToUIEdgeInsets(this Thickness self)
        {
            return new UIEdgeInsets((nfloat)self.Top, (nfloat)self.Left, (nfloat)self.Bottom, (nfloat)self.Right);
        }
    }
}
