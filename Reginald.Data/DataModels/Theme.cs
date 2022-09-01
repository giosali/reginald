namespace Reginald.Data.DataModels
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using Newtonsoft.Json;

    public class Theme
    {
        public const string FileName = "Themes.json";

        [JsonProperty("acrylicOpacity")]
        public byte AcrylicOpacity { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("backgroundBrush")]
        public SolidColorBrush BackgroundBrush { get; set; }

        [JsonProperty("borderBrush")]
        public SolidColorBrush BorderBrush { get; set; }

        [JsonProperty("borderThickness")]
        public double BorderThickness { get; set; }

        [JsonProperty("captionBrush")]
        public SolidColorBrush CaptionBrush { get; set; }

        [JsonProperty("captionFontSize")]
        public double CaptionFontSize { get; set; }

        [JsonProperty("captionFontWeight")]
        public FontWeight CaptionFontWeight { get; set; }

        [JsonProperty("caretBrush")]
        public SolidColorBrush CaretBrush { get; set; }

        [JsonProperty("clipboardDisplayFontSize")]
        public double ClipboardDisplayFontSize { get; set; }

        [JsonProperty("clipboardHeight")]
        public double ClipboardHeight { get; set; }

        [JsonProperty("clipboardItemFontSize")]
        public double ClipboardItemFontSize { get; set; }

        [JsonProperty("clipboardWidth")]
        public double ClipboardWidth { get; set; }

        [JsonProperty("cornerRadius")]
        public double CornerRadius { get; set; }

        [JsonProperty("descriptionBrush")]
        public SolidColorBrush DescriptionBrush { get; set; }

        [JsonProperty("descriptionFontSize")]
        public double DescriptionFontSize { get; set; }

        [JsonProperty("descriptionFontWeight")]
        public FontWeight DescriptionFontWeight { get; set; }

        [JsonProperty("fontFamily")]
        public FontFamily FontFamily { get; set; }

        [JsonProperty("guid")]
        public Guid Guid { get; set; }

        [JsonProperty("HighlightBrush")]
        public SolidColorBrush HighlightBrush { get; set; }

        [JsonProperty("inputBrush")]
        public SolidColorBrush InputBrush { get; set; }

        [JsonProperty("inputFontSize")]
        public double InputFontSize { get; set; }

        [JsonProperty("inputFontWeight")]
        public FontWeight InputFontWeight { get; set; }

        [JsonProperty("isAcrylicEnabled")]
        public bool IsAcrylicEnabled { get; set; }

        [JsonProperty("isEditable")]
        public bool IsEditable { get; set; }

        [JsonProperty("mainHeight")]
        public double MainHeight { get; set; }

        [JsonProperty("mainWidth")]
        public double MainWidth { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("placeholderInputBrush")]
        public SolidColorBrush PlaceholderInputBrush { get; set; }

        [JsonProperty("selectedCaptionBrush")]
        public SolidColorBrush SelectedCaptionBrush { get; set; }

        [JsonProperty("SelectedDescriptionBrush")]
        public SolidColorBrush SelectedDescriptionBrush { get; set; }

        [JsonProperty("selectionBrush")]
        public SolidColorBrush SelectionBrush { get; set; }

        [JsonProperty("selectionOpacity")]
        public double SelectionOpacity { get; set; }
    }
}
