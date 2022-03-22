namespace Reginald.Data.Keywords
{
    using Newtonsoft.Json;

    public interface IKeywordDataModel
    {
        [JsonProperty("guid")]
        public string Guid { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("keyword")]
        public string Keyword { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("format")]
        public string Format { get; set; }

        [JsonProperty("placeholder")]
        public string Placeholder { get; set; }

        [JsonProperty("caption")]
        public string Caption { get; set; }

        [JsonProperty("isEnabled")]
        public bool IsEnabled { get; set; }
    }
}
