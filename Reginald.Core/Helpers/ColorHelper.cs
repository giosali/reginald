namespace Reginald.Core.Helpers
{
    using System.Reflection;
    using System.Windows.Media;

    public static class ColorHelper
    {
        public static Color FromString(string expression)
        {
            return (Color)ColorConverter.ConvertFromString(expression);
        }

        public static bool TryGetName(Color color, out string name)
        {
            name = color.ToString();
            PropertyInfo[] properties = typeof(Colors).GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo property = properties[i];
                string hex = property.GetValue(color).ToString();
                if (name == hex)
                {
                    name = property.Name;
                    return true;
                }
            }

            return false;
        }

        public static Color InvertColor(Color color, bool makeReadable)
        {
            byte r = (byte)~color.R;
            byte g = (byte)~color.G;
            byte b = (byte)~color.B;

            if (makeReadable)
            {
                // If the current RGB values create a grey background,
                // return a black brush for black text on a grey background
                r = r is > 110 and < 160 ? (byte)0 : r;
                g = g is > 110 and < 160 ? (byte)0 : g;
                b = b is > 110 and < 160 ? (byte)0 : b;
            }

            return Color.FromRgb(r, g, b);
        }
    }
}
