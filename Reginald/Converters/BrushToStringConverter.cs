using Reginald.Core.Helpers;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Reginald.Converters
{
    [ValueConversion(typeof(Brush), typeof(string))]
    public class BrushToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Brush brush = value as Brush;
            _ = BrushHelper.TryGetName(brush, out string name);
            return name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string color = value as string;
            Brush brush = (Brush)new BrushConverter().ConvertFromString(color);
            return brush;
        }
    }
}
