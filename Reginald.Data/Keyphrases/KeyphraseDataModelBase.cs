namespace Reginald.Data.Keyphrases
{
    using Newtonsoft.Json;

    public abstract class KeyphraseDataModelBase
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

        public static bool operator ==(KeyphraseDataModelBase a, KeyphraseDataModelBase b)
        {
            return a.Guid == b.Guid;
        }

        public static bool operator !=(KeyphraseDataModelBase a, KeyphraseDataModelBase b)
        {
            return a.Guid != b.Guid;
        }

        public override bool Equals(object obj)
        {
            return obj is not null && obj is KeyphraseDataModelBase model && Guid == model.Guid;
        }

        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }
    }
}
