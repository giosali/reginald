namespace Reginald.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;
    using Reginald.Models.Drawing;
    using Reginald.Services.Helpers;

    [ValueConversion(typeof(Icon), typeof(ImageSource))]
    internal sealed class IconToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Icon icon)
            {
                return null;
            }

            string iconPath = icon.Path;
            if (iconPath is null)
            {
                return icon.Source;
            }

            if (iconPath.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
            {
                return BitmapSourceHelper.ExtractAssociatedBitmapSource(iconPath);
            }

            if (!uint.TryParse(iconPath, out uint result))
            {
                return iconPath;
            }

            return BitmapSourceHelper.GetStockIcon(result);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
