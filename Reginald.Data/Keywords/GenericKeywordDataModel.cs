namespace Reginald.Data.Keywords
{
    using Newtonsoft.Json;

    public class GenericKeywordDataModel : DataModelBase, IKeywordDataModel
    {
        public string Name { get; set; }

        public string Keyword { get; set; }

        public string Icon { get; set; }

        public string Format { get; set; }

        public string Placeholder { get; set; }

        public string Caption { get; set; }

        public bool IsEnabled { get; set; }

        [JsonProperty("useUtf8")]
        public bool UseUtf8 { get; set; }

        [JsonProperty("separator")]
        public string Separator { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("altUrl")]
        public string AltUrl { get; set; }

        [JsonProperty("altDescription")]
        public string AltDescription { get; set; }
    }
}
