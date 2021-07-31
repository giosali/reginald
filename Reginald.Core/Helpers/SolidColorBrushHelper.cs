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
    }
}
