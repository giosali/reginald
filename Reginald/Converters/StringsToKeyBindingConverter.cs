namespace Reginald.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Input;

    public class StringsToKeyBindingConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Key key = (Key)Enum.Parse(typeof(Key), (string)values[0]);
            ModifierKeys modifierOne = (ModifierKeys)Enum.Parse(typeof(ModifierKeys), (string)values[1]);
            ModifierKeys modifierTwo = (ModifierKeys)Enum.Parse(typeof(ModifierKeys), (string)values[2]);
            return new KeyBinding()
            {
                Key = key,
                Modifiers = modifierOne | modifierTwo,
            };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
