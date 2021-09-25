using System.Reflection;
using System.Windows.Media;

namespace Reginald.Core.Helpers
{
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
    }
}
