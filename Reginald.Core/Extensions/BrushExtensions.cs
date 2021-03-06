namespace Reginald.Core.Extensions
{
    using System.Windows.Media;

    public static class BrushExtensions
    {
        public static string MakeTransparent(this Brush brush, string transparency)
        {
            string hex = brush.ToString();
            if (hex.Length > 7)
            {
                hex = transparency + hex[3..];
            }

            return hex;
        }
    }
}
