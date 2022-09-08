namespace Reginald.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;

    internal class BorderClipMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // For more information, see: https://stackoverflow.com/a/5650367/18831815
            if (values.Length != 3)
            {
                return DependencyProperty.UnsetValue;
            }

            if (values[0] is not double width)
            {
                return DependencyProperty.UnsetValue;
            }

            if (values[1] is not double height)
            {
                return DependencyProperty.UnsetValue;
            }

            if (width < double.Epsilon || height < double.Epsilon)
            {
                return Geometry.Empty;
            }

            if (values[2] is not CornerRadius cornerRadius)
            {
                return DependencyProperty.UnsetValue;
            }

            RectangleGeometry clip = new(new Rect(0, 0, width, height), cornerRadius.TopLeft, cornerRadius.TopLeft);
            clip.Freeze();
            return clip;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }       
    }
}
