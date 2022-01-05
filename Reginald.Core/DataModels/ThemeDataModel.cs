using Newtonsoft.Json;
using System;

namespace Reginald.Core.DataModels
{
    [Serializable]
    public class ThemeDataModel : UnitDataModelBase
    {
        // Info

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("isEditable")]
        public bool IsEditable { get; set; }

        // Main

        [JsonProperty("backgroundColor")]
        public string BackgroundColor { get; set; }

        [JsonProperty("tintOpacity")]
        public double TintOpacity { get; set; }

        [JsonProperty("inputBrush")]
        public string InputBrush { get; set; }

        [JsonProperty("caretBrush")]
        public string CaretBrush { get; set; }

        [JsonProperty("descriptionBrush")]
        public string DescriptionBrush { get; set; }

        [JsonProperty("captionBrush")]
        public string CaptionBrush { get; set; }

        [JsonProperty("borderBrush")]
        public string BorderBrush { get; set; }

        [JsonProperty("highlightBrush")]
        public string HighlightBrush { get; set; }

        public override bool Predicate(UnitDataModelBase model, string guid)
        {
            return model.Guid == guid;
        }
    }
}
