namespace Reginald.Models.DataModels
{
    using System.Windows;
    using System.Windows.Media;
    using Newtonsoft.Json;
    using Reginald.Models.Converters;

    internal sealed class MainTheme
    {
        [JsonProperty("height")]
        public double Height { get; set; }

        [JsonConverter(typeof(SolidColorBrushConverter))]
        [JsonProperty("searchResultCaptionBrush")]
        public SolidColorBrush SearchResultCaptionBrush { get; set; }

        [JsonProperty("searchResultCaptionFontSize")]
        public double SearchResultCaptionFontSize { get; set; }

        [JsonProperty("searchResultCaptionFontWeight")]
        public FontWeight SearchResultCaptionFontWeight { get; set; }

        [JsonProperty("searchResultDescriptionFontSize")]
        public double SearchResultDescriptionFontSize { get; set; }

        [JsonProperty("searchResultDescriptionFontWeight")]
        public FontWeight SearchResultDescriptionFontWeight { get; set; }

        [JsonProperty("searchResultMargin")]
        public Thickness SearchResultMargin { get; set; }

        [JsonConverter(typeof(SolidColorBrushConverter))]
        [JsonProperty("separatorBrush")]
        public SolidColorBrush SeparatorBrush { get; set; }

        [JsonProperty("separatorHeight")]
        public double SeparatorHeight { get; set; }

        [JsonProperty("separatorMargin")]
        public Thickness SeparatorMargin { get; set; }

        [JsonProperty("separatorWidth")]
        public double SeparatorWidth { get; set; }

        [JsonProperty("width")]
        public double Width { get; set; }
    }
}
