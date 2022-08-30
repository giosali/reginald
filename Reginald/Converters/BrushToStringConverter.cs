namespace Reginald.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;
    using Reginald.Core.Helpers;

    [ValueConversion(typeof(Brush), typeof(string))]
    public class BrushToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush brush = value as SolidColorBrush;
            if (parameter is not string format)
            {
                _ = BrushHelper.TryGetName(brush, out string name);
                return name;
            }

            if (brush is null)
            {
                return null;
            }

            try
            {
                Color color = brush.Color;
                return string.Format(format, color.R, color.G, color.B);
            }
            catch (SystemException)
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string color = value as string;
            Brush brush = (Brush)new BrushConverter().ConvertFromString(color);
            return brush;
        }
    }
}
