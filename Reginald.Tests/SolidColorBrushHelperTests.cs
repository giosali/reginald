using Reginald.Core.Helpers;
using System.Windows.Media;
using Xunit;

namespace Reginald.Tests
{
    public class SolidColorBrushHelperTests
    {
        [Fact]
        public static void FromRgb_ShouldReturnSolidColorBrush()
        {
            System.Drawing.Color color = System.Drawing.Color.White;
            var brush = SolidColorBrushHelper.FromRgb(color);
            Assert.True(brush is SolidColorBrush);
        }

        [Fact]
        public static void FromArgb_ShouldReturnSolidColorBrush()
        {
            System.Drawing.Color color = System.Drawing.Color.White;
            var brush = SolidColorBrushHelper.FromArgb(color);
            Assert.True(brush is SolidColorBrush);
        }
    }
}
