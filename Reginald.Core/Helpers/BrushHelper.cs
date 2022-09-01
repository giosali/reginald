namespace Reginald.Core.Helpers
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Windows.Media;

    public static class BrushHelper
    {
        public static bool TryFromString(string expression, out Brush brush)
        {
            brush = null;
            int expressionLength = expression.Length;
            if (expressionLength < 6 || expressionLength > 7 || (expressionLength == 7 && !expression.StartsWith("#")))
            {
                return false;
            }

            if (expressionLength == 6)
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
