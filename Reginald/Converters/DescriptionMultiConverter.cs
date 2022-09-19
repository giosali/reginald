namespace Reginald.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    internal sealed class DescriptionMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is not string description || values[1] is not string placeholder)
            {
                return string.Empty;
            }

            return description.Contains("{0}") ? string.Format(description, placeholder) : description;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
