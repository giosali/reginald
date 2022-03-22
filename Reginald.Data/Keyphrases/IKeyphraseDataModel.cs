namespace Reginald.Data.Keyphrases
{
    using Newtonsoft.Json;

    public interface IKeyphraseDataModel
    {
        [JsonProperty("guid")]
        public string Guid { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("keyphrase")]
        public string Keyphrase { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("caption")]
        public string Caption { get; set; }

        [JsonProperty("isEnabled")]
        public bool IsEnabled { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
