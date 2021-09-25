using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Reginald.Converters
{
    [ValueConversion(typeof(Color), typeof(Brush))]
    public class ColorToSolidColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color color = (Color)value;
            return new SolidColorBrush(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
