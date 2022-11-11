﻿using CoreGraphics;
using Microsoft.Maui.Platform;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using UIKit;

namespace GGMapMAUI.Platforms.iOS
{
    internal static class Utils
    {
        public static UIView ConvertFormsToNative(View view, CGRect size)
        {
            var renderer = Microsoft.Maui.Controls.Compatibility.Platform.iOS.Platform.GetRenderer(view);

            renderer.NativeView.Frame = size;

            renderer.NativeView.AutoresizingMask = UIViewAutoresizing.All;
            renderer.NativeView.ContentMode = UIViewContentMode.ScaleToFill;

            renderer.Element.Layout(size.ToRectangle());

            var nativeView = renderer.NativeView;

            nativeView.SetNeedsLayout();

            return nativeView;
        }

        private static LinkedList<string> lruTracker = new LinkedList<string>();
        private static ConcurrentDictionary<string, UIImage> cache = new ConcurrentDictionary<string, UIImage>();

        public static UIImage ConvertViewToImage(UIView view)
        {
            UIGraphics.BeginImageContextWithOptions(view.Bounds.Size, false, 0);
            view.Layer.RenderInContext(UIGraphics.GetCurrentContext());
            UIImage img = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            // Optimization: Let's try to reuse any of the last 10 images we generated
            var bytes = img.AsPNG().ToArray();
            var md5 = MD5.Create();
            var hash = Convert.ToBase64String(md5.ComputeHash(bytes));

            var exists = cache.ContainsKey(hash);
            if (exists)
            {
                lruTracker.Remove(hash);
                lruTracker.AddLast(hash);
                return cache[hash];
            }
            if (lruTracker.Count > 10) // O(1)
            {
                UIImage tmp;
                cache.TryRemove(lruTracker.First.Value, out tmp);
                lruTracker.RemoveFirst();
            }
            cache.GetOrAdd(hash, img);
            lruTracker.AddLast(hash);
            return img;
        }
    }
}

