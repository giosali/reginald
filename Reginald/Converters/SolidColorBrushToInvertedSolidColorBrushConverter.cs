using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Reginald.Converters
{
    [ValueConversion(typeof(SolidColorBrush), typeof(SolidColorBrush))]
    public class SolidColorBrushToInvertedSolidColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush brush = value as SolidColorBrush;
            byte a = brush.Color.A;
            byte r = (byte)(byte.MaxValue - brush.Color.R);
            byte g = (byte)(byte.MaxValue - brush.Color.G);
            byte b = (byte)(byte.MaxValue - brush.Color.B);
            Color invertedColor = Color.FromArgb(a, r, g, b);
            return new SolidColorBrush(invertedColor);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
