namespace Reginald.Core.Helpers
{
    using System;
    using System.Reflection;
    using System.Windows.Media;

    public static class BrushHelper
    {
        public static Brush SolidColorBrushFromString(string expression)
        {
            return expression is null ? null : (SolidColorBrush)new BrushConverter().ConvertFromString(expression);
        }

        public static bool TryFromString(string expression, out Brush brush)
        {
            if (!expression.StartsWith("#"))
            {
                expression = expression.Insert(0, "#");
            }

            if (expression.Length == 7)
            {
                try
                {
                    brush = (Brush)new BrushConverter().ConvertFromString(expression);
                    return true;
                }
                catch (FormatException)
                {
                }
            }

            brush = null;
            return false;
        }

        public static bool TryGetName(Brush brush, out string name)
        {
            name = null;
            if (brush is not null)
            {
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

            return false;
        }

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
    }
}
