namespace Reginald.Data.Representations
{
    using Newtonsoft.Json;

    public interface IRepresentationDataModel
    {
        [JsonProperty("guid")]
        public string Guid { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("caption")]
        public string Caption { get; set; }

        [JsonProperty("isEnabled")]
        public bool IsEnabled { get; set; }
    }
}
