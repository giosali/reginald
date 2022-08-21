namespace Reginald.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;
    using Reginald.Services.Helpers;

    [ValueConversion(typeof(string), typeof(ImageSource))]
    internal class StringToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!uint.TryParse((string)value, out uint result))
            {
                return value;
            }

            return BitmapSourceHelper.GetStockIcon(result);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
