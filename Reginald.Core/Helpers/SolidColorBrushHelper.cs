using System.Reflection;
using System.Windows.Media;

namespace Reginald.Core.Helpers
{
    public static class SolidColorBrushHelper
    {
        public static SolidColorBrush FromRgb(System.Drawing.Color color)
        {
            return new SolidColorBrush(Color.FromRgb(color.R, color.G, color.B));
        }

        public static SolidColorBrush FromArgb(System.Drawing.Color color)
        {
            return new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
        }

        public static SolidColorBrush FromString(string expression)
        {
            return expression is null ? null : (SolidColorBrush)new BrushConverter().ConvertFromString(expression);
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
    }
}
