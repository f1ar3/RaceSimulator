using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace RaceSimulator.ViewModels
{
    public class BusyToColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isBusy)
            {
                return isBusy ? Brushes.Red : Brushes.Green;
            }
            return Brushes.Gray;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}