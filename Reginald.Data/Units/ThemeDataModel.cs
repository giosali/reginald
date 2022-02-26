namespace Reginald.Data.Units
{
    using Newtonsoft.Json;

    public class ThemeDataModel : UnitDataModelBase
    {
        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("isEditable")]
        public bool IsEditable { get; set; }

        [JsonProperty("fontFamily")]
        public string FontFamily { get; set; }

        [JsonProperty("backgroundColor")]
        public string BackgroundColor { get; set; }

        [JsonProperty("tintOpacity")]
        public double TintOpacity { get; set; }

        [JsonProperty("inputFontSize")]
        public double InputFontSize { get; set; }

        [JsonProperty("inputFontWeight")]
        public string InputFontWeight { get; set; }

        [JsonProperty("inputBrush")]
        public string InputBrush { get; set; }

        [JsonProperty("placeholderInputBrush")]
        public string PlaceholderInputBrush { get; set; }

        [JsonProperty("caretBrush")]
        public string CaretBrush { get; set; }

        [JsonProperty("descriptionFontSize")]
        public double DescriptionFontSize { get; set; }

        [JsonProperty("descriptionFontWeight")]
        public string DescriptionFontWeight { get; set; }

        [JsonProperty("descriptionBrush")]
        public string DescriptionBrush { get; set; }

        [JsonProperty("selectedDescriptionBrush")]
        public string SelectedDescriptionBrush { get; set; }

        [JsonProperty("captionFontSize")]
        public double CaptionFontSize { get; set; }

        [JsonProperty("captionFontWeight")]
        public string CaptionFontWeight { get; set; }

        [JsonProperty("captionBrush")]
        public string CaptionBrush { get; set; }

        [JsonProperty("selectedCaptionBrush")]
        public string SelectedCaptionBrush { get; set; }

        [JsonProperty("borderBrush")]
        public string BorderBrush { get; set; }

        [JsonProperty("borderThickness")]
        public double BorderThickness { get; set; }

        [JsonProperty("cornerRadius")]
        public double CornerRadius { get; set; }

        [JsonProperty("highlightBrush")]
        public string HighlightBrush { get; set; }

        [JsonProperty("selectionBrush")]
        public string SelectionBrush { get; set; }

        [JsonProperty("selectionOpacity")]
        public double SelectionOpacity { get; set; }

        [JsonProperty("clipboardItemFontSize")]
        public double ClipboardItemFontSize { get; set; }

        [JsonProperty("clipboardDisplayFontSize")]
        public double ClipboardDisplayFontSize { get; set; }

        public override bool Predicate(string guid)
        {
            return Guid == guid;
        }
    }
}
