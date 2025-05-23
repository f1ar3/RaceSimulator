using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace RaceSimulator.Converters
{
    public class BoolToIconConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is true ? "🛠" : "🚗";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}