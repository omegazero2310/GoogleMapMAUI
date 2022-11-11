﻿using System.ComponentModel;
using System.Globalization;

namespace Maui.GoogleMaps.Helpers
{
    public sealed class CameraUpdateConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(CameraUpdate);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var err = $@"{value} is invalid format. Expects are ""lat, lon"", ""lat, lon, zoom"", ""lat, lon, zoom, rotation"" and ""lat, lon, zoom, rotation, tilt"" ";
            var values = value?.ToString().Split(',') ?? new string[1];
            if (values.Length < 2)
            {
                throw new ArgumentException(err);
            }

            var nums = new List<double>();
            foreach (var v in values)
            {
                double ret;
                if (!double.TryParse(v, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out ret))
                {
                    throw new ArgumentException(err);
                }
                nums.Add(ret);
            }

            if (nums.Count == 2)
            {
                return CameraUpdateFactory.NewPosition(new Position(nums[0], nums[1]));
            }

            if (nums.Count == 3)
            {
                return CameraUpdateFactory.NewPositionZoom(new Position(nums[0], nums[1]), nums[2]);
            }

            if (nums.Count == 4)
            {
                return CameraUpdateFactory.NewCameraPosition(new CameraPosition(
                    new Position(nums[0], nums[1]),
                    nums[2],
                    nums[3]));
            }

            return CameraUpdateFactory.NewCameraPosition(new CameraPosition(
                new Position(nums[0], nums[1]),
                nums[2],
                nums[3],
                nums[4]));
        }
    }
}
