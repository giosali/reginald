namespace Reginald.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    [ValueConversion(typeof(SolidColorBrush), typeof(SolidColorBrush))]
    internal sealed class SolidColorBrushToInvertedSolidColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color color = (value as SolidColorBrush).Color;
            byte a = color.A;
            byte r = (byte)(byte.MaxValue - color.R);
            byte g = (byte)(byte.MaxValue - color.G);
            byte b = (byte)(byte.MaxValue - color.B);
            return new SolidColorBrush(Color.FromArgb(a, r, g, b));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
