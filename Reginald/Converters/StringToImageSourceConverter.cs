namespace Reginald.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;
    using Reginald.Data.Drawing;
    using Reginald.Services.Helpers;

    [ValueConversion(typeof(string), typeof(ImageSource))]
    internal class StringToImageSourceConverter : IValueConverter
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


            // string str = (string)value;
            // if (str.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
            // {
            //     return BitmapSourceHelper.ExtractAssociatedBitmapSource(str);
            // }

            // if (!uint.TryParse(str, out uint result))
            // {
            //     return value;
            // }

            // return BitmapSourceHelper.GetStockIcon(result);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
