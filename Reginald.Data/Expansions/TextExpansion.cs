namespace Reginald.Data.Expansions
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class TextExpansion
    {
        public const string Filename = "Expansions.json";

        [JsonProperty("trigger")]
        public string Trigger { get; set; }

        [JsonProperty("replacement")]
        public string Replacement { get; set; }
    }
}
