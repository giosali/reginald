namespace Reginald.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    internal sealed class DescriptionMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string description = (string)values[0];
            if (!string.IsNullOrEmpty(description))
            {
                return description;
            }

            string descriptionFormat = (string)values[1];
            string placeholder = (string)values[2];
            return string.Format(descriptionFormat, placeholder);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
