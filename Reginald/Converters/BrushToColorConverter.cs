namespace Reginald.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    [ValueConversion(typeof(Brush), typeof(Color))]
    internal class BrushToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color color = (value as SolidColorBrush).Color;
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
