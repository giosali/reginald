namespace Reginald.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Data;

    internal sealed class IEnumerableToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not IEnumerable<string> strs)
            {
                return DependencyProperty.UnsetValue;
            }

            return string.Join(Environment.NewLine, strs);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string text)
            {
                return DependencyProperty.UnsetValue;
            }

            HashSet<string> hashSet = new();
            using StringReader reader = new(text);
            while (reader.ReadLine() is string line)
            {
                hashSet.Add(line);
            }

            return hashSet;
        }
    }
}
