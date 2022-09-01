namespace Reginald.Core.Helpers
{
    using System.Reflection;
    using System.Windows.Media;

    public static class BrushHelper
    {
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
