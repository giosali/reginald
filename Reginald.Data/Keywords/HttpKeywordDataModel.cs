namespace Reginald.Data.Keywords
{
    using Newtonsoft.Json;

    public class HttpKeywordDataModel : DataModelBase, IKeywordDataModel
    {
        public string Name { get; set; }

        public string Keyword { get; set; }

        public string Icon { get; set; }

        public string Format { get; set; }

        public string Placeholder { get; set; }

        public string Caption { get; set; }

        public bool IsEnabled { get; set; }

        [JsonProperty("primaryIcon")]
        public string PrimaryIcon { get; set; }

        [JsonProperty("auxiliaryIcon")]
        public string AuxiliaryIcon { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("captionFormat")]
        public string CaptionFormat { get; set; }

        [JsonProperty("api")]
        public string Api { get; set; }
    }
}
