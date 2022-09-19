namespace Reginald.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Input;

    internal sealed class StringToKeyBindingMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Key key = (Key)Enum.Parse(typeof(Key), (string)values[0]);
            ModifierKeys modifiers = (ModifierKeys)Enum.Parse(typeof(ModifierKeys), (string)values[1]);
            return new KeyBinding()
            {
                Key = key,
                Modifiers = modifiers,
            };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
