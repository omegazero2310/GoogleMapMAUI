using Android.App;
using Android.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GGMapMAUI.Platforms.Android.Extensions
{
    public static class ActivityExtensions
    {
        public static float GetScaledDensity(this Activity self)
        {
            var metrics = new DisplayMetrics();
            self.WindowManager.DefaultDisplay.GetMetrics(metrics);
            return metrics.ScaledDensity;
        }
    }
}
