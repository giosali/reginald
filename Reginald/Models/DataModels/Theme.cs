namespace Reginald.Models.DataModels
{
    using System.Windows;
    using System.Windows.Media;
    using Newtonsoft.Json;
    using Reginald.Models.Converters;

    internal sealed class Theme
    {
        public const string FileName = "Themes.json";

        [JsonProperty("acrylicOpacity")]
        public byte AcrylicOpacity { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonConverter(typeof(SolidColorBrushConverter))]
        [JsonProperty("backgroundBrush")]
        public SolidColorBrush BackgroundBrush { get; set; }

        [JsonConverter(typeof(SolidColorBrushConverter))]
        [JsonProperty("borderBrush")]
        public SolidColorBrush BorderBrush { get; set; }

        [JsonProperty("borderThickness")]
        public double BorderThickness { get; set; }

        [JsonConverter(typeof(SolidColorBrushConverter))]
        [JsonProperty("caretBrush")]
        public SolidColorBrush CaretBrush { get; set; }

        [JsonProperty("clipboardManager")]
        public ClipboardManagerTheme ClipboardManager { get; set; }

        [JsonProperty("cornerRadius")]
        public double CornerRadius { get; set; }

        [JsonConverter(typeof(SolidColorBrushConverter))]
        [JsonProperty("descriptionBrush")]
        public SolidColorBrush DescriptionBrush { get; set; }

        [JsonProperty("fontFamily")]
        public FontFamily FontFamily { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonConverter(typeof(SolidColorBrushConverter))]
        [JsonProperty("highlightBrush")]
        public SolidColorBrush HighlightBrush { get; set; }

        [JsonConverter(typeof(SolidColorBrushConverter))]
        [JsonProperty("inactiveBrush")]
        public SolidColorBrush InactiveBrush { get; set; }

        [JsonProperty("isAcrylicEnabled")]
        public bool IsAcrylicEnabled { get; set; }

        [JsonProperty("isEditable")]
        public bool IsEditable { get; set; }

        [JsonProperty("main")]
        public MainTheme Main { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonConverter(typeof(SolidColorBrushConverter))]
        [JsonProperty("selectedCaptionBrush")]
        public SolidColorBrush SelectedCaptionBrush { get; set; }

        [JsonConverter(typeof(SolidColorBrushConverter))]
        [JsonProperty("selectedDescriptionBrush")]
        public SolidColorBrush SelectedDescriptionBrush { get; set; }

        [JsonConverter(typeof(SolidColorBrushConverter))]
        [JsonProperty("selectionBrush")]
        public SolidColorBrush SelectionBrush { get; set; }

        [JsonProperty("selectionOpacity")]
        public double SelectionOpacity { get; set; }

        [JsonConverter(typeof(SolidColorBrushConverter))]
        [JsonProperty("textBoxBrush")]
        public SolidColorBrush TextBoxBrush { get; set; }

        [JsonProperty("textBoxFontSize")]
        public double TextBoxFontSize { get; set; }

        [JsonProperty("textBoxFontWeight")]
        public FontWeight TextBoxFontWeight { get; set; }

        [JsonProperty("textBoxPadding")]
        public Thickness TextBoxPadding { get; set; }
    }
}
