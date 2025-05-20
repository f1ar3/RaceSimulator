using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace RaceSimulator.ViewModels
{
    public class Add25Converter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is double y)
            {
                return y + 25;
            }
            return value;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}