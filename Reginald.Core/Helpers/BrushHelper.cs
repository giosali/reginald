namespace Reginald.Core.Helpers
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Windows.Media;

    public static class BrushHelper
    {
        public static Brush InvertBrush(Brush brush, bool makeReadable)
        {
            SolidColorBrush solidColorBrush = brush as SolidColorBrush;
            byte r = (byte)~solidColorBrush.Color.R;
            byte g = (byte)~solidColorBrush.Color.G;
            byte b = (byte)~solidColorBrush.Color.B;

            if (makeReadable)
            {
                // If the current RGB values create a grey background,
                // return a black brush for black text on a grey background
                r = r is > 110 and < 160 ? (byte)0 : r;
                g = g is > 110 and < 160 ? (byte)0 : g;
                b = b is > 110 and < 160 ? (byte)0 : b;
            }

            return new SolidColorBrush(Color.FromRgb(r, g, b));
        }

        public static Brush SolidColorBrushFromString(string expression)
        {
            return expression is null ? null : (SolidColorBrush)new BrushConverter().ConvertFromString(expression);
        }

        public static bool TryFromString(string expression, out Brush brush)
        {
            brush = null;
            if (expression.Length < 6 || expression.Length > 7)
            {
                return false;
            }

            if (!expression.StartsWith('#'))
            {
                expression = "#" + expression;
            }

            if (!int.TryParse(expression[1..], NumberStyles.HexNumber, null, out _))
            {
                return false;
            }

            try
            {
                brush = (Brush)new BrushConverter().ConvertFromString(expression);
            }
            catch (FormatException)
            {
                return false;
            }

            return true;
        }

        public static bool TryGetName(Brush brush, out string name)
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
