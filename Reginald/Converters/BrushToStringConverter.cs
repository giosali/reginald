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
            if (parameter is string format && value is SolidColorBrush solidColorBrush)
            {
                try
                {
                    return string.Format(format, solidColorBrush.Color.R, solidColorBrush.Color.G, solidColorBrush.Color.B);
                }
                catch (Exception ex)
                {
                    if (ex is FormatException || ex is ArgumentException)
                    {
                    }
                }
            }

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
