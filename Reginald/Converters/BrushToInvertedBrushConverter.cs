namespace Reginald.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    [ValueConversion(typeof(Brush), typeof(Brush))]
    public class BrushToInvertedBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not SolidColorBrush brush)
            {
                return null;
            }

            Color color = brush.Color;
            byte r = (byte)~color.R;
            byte g = (byte)~color.G;
            byte b = (byte)~color.B;

            // If the current RGB values create a grey background,
            // return a black brush for black text on a grey background.
            // This aims to make text readable on a grey background.
            r = r is > 110 and < 160 ? (byte)0 : r;
            g = g is > 110 and < 160 ? (byte)0 : g;
            b = b is > 110 and < 160 ? (byte)0 : b;
            return new SolidColorBrush(Color.FromRgb(r, g, b));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
