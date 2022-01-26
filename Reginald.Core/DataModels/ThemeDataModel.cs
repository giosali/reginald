namespace Reginald.Core.DataModels
{
    using System;
    using Newtonsoft.Json;

    [Serializable]
    public class ThemeDataModel : UnitDataModelBase
    {
        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("minimumBuild")]
        public int MinimumBuild { get; set; }

        [JsonProperty("isEditable")]
        public bool IsEditable { get; set; }

        [JsonProperty("requiresRefresh")]
        public bool RequiresRefresh { get; set; }

        [JsonProperty("isLightTheme")]
        public bool IsLightTheme { get; set; }

        [JsonProperty("isAcrylicEnabled")]
        public bool IsAcrylicEnabled { get; set; }

        [JsonProperty("isMicaEnabled")]
        public bool IsMicaEnabled { get; set; }

        [JsonProperty("fontFamily")]
        public string FontFamily { get; set; }

        [JsonProperty("backgroundColor")]
        public string BackgroundColor { get; set; }

        [JsonProperty("tintOpacity")]
        public double TintOpacity { get; set; }

        [JsonProperty("inputFontWeight")]
        public string InputFontWeight { get; set; }

        [JsonProperty("inputBrush")]
        public string InputBrush { get; set; }

        [JsonProperty("caretBrush")]
        public string CaretBrush { get; set; }

        [JsonProperty("descriptionFontWeight")]
        public string DescriptionFontWeight { get; set; }

        [JsonProperty("descriptionBrush")]
        public string DescriptionBrush { get; set; }

        [JsonProperty("selectedDescriptionBrush")]
        public string SelectedDescriptionBrush { get; set; }

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

        public override bool Predicate(string guid)
        {
            return Guid == guid;
        }
    }
}
