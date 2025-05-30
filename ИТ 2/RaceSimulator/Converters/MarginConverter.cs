using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia;

namespace RaceSimulator.Converters
{
    public class MarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double position)
            {
                return new Thickness(position, 0, 0, 0);
            }

            return new Thickness(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}