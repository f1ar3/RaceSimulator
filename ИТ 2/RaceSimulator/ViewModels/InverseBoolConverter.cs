using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace RaceSimulator.ViewModels
{
    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is bool b && !b;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is bool b && !b;
        }
    }
}