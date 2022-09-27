namespace Reginald.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    internal sealed class Int32ToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not int num)
            {
                return DependencyProperty.UnsetValue;
            }

            return num.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string text || !int.TryParse(text, out int num))
            {
                return DependencyProperty.UnsetValue;
            }

            return num;
        }
    }
}
