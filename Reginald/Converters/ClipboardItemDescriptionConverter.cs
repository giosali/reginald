namespace Reginald.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    [ValueConversion(typeof(string), typeof(string))]
    internal class ClipboardItemDescriptionConverter : IValueConverter
    {
        private const int MaxLength = 15000;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string str)
            {
                return null;
            }

            return str.Length <= MaxLength ? str : str[..MaxLength] + "\nTRUNCATED";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
