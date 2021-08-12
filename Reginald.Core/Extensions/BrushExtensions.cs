using System.Windows.Media;

namespace Reginald.Core.Extensions
{
    public static class BrushExtensions
    {
        public static string GetTransparentHex(this Brush brush, string transparency)
        {
            string hex = brush.ToString();
            if (hex.Length > 7)
            {
                hex = transparency + hex.Substring(3);
            }
            return hex;
        }
    }
}
