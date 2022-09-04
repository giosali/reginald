namespace Reginald.Models.Converters
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using Newtonsoft.Json;

    internal class SolidColorBrushConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(SolidColorBrush);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value is not string value)
            {
                return null;
            }

            SolidColorBrush brush = new();
            Application.Current.Dispatcher.Invoke(() =>
            {
                brush = new BrushConverter().ConvertFromString(value) as SolidColorBrush;
            });
            return brush;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }
    }
}
