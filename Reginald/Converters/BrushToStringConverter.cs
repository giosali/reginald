namespace Reginald.Converters
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Windows.Data;
    using System.Windows.Media;

    [ValueConversion(typeof(Brush), typeof(string))]
    public class BrushToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush brush = value as SolidColorBrush;
            if (parameter is not string format)
            {
                _ = TryGetName(brush, out string name);
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

        private static bool TryGetName(Brush brush, out string name)
        {
            name = null;
            if (brush is null)
            {
                return false;
            }

            name = brush.ToString();
            PropertyInfo[] properties = typeof(Brushes).GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo property = properties[i];
                string hex = property.GetValue(brush).ToString();
                if (name == hex)
                {
                    name = property.Name;
                    break;
                }
            }

            return true;
        }
    }
}
