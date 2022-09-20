namespace Reginald.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Reginald.Models.Drawing;
    using Reginald.Services.Helpers;

    [ValueConversion(typeof(Icon), typeof(ImageSource))]
    internal sealed class IconToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Icon icon)
            {
                return DependencyProperty.UnsetValue;
            }

            if (icon.Path is not string iconPath)
            {
                return icon.Source;
            }

            if (iconPath.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
            {
                return BitmapSourceHelper.ExtractAssociatedBitmapSource(iconPath);
            }

            if (!uint.TryParse(iconPath, out uint result))
            {
                BitmapImage bi = new();
                bi.BeginInit();
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bi.UriSource = new Uri(iconPath);
                bi.EndInit();
                bi.Freeze();
                return bi;
                // return iconPath;
            }

            return BitmapSourceHelper.GetStockIcon(result);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
