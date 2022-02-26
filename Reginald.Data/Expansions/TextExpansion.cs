namespace Reginald.Data.Expansions
{
    using Newtonsoft.Json;

    public class TextExpansion
    {
        [JsonProperty("trigger")]
        public string Trigger { get; set; }

        [JsonProperty("replacement")]
        public string Replacement { get; set; }
    }
}
