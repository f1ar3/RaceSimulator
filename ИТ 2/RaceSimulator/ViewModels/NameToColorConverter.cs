using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace RaceSimulator.ViewModels
{
    public class NameToColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string name)
            {
                return name switch
                {
                    "Red Bull" => Brushes.DarkBlue,
                    "Ferrari" => Brushes.Red,
                    "Mercedes" => Brushes.Turquoise,
                    "McLaren" => Brushes.Orange,
                    "Williams" => Brushes.White,
                    "Alpine" => Brushes.Blue,
                    "Aston Martin" => Brushes.DarkGreen,
                    "AlphaTauri" => Brushes.White,
                    "Alfa Romeo" => Brushes.DarkRed,
                    "Haas" => Brushes.Black,
                    _ => Brushes.Gray
                };
            }
            return Brushes.Gray;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}