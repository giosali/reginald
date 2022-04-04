namespace Reginald.Data.Expansions
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class TextExpansion
    {
        public const string Filename = "Expansions.json";

        public TextExpansion(string trigger, string replacement)
        {
            Trigger = trigger;
            Replacement = replacement;
        }

        [JsonProperty("trigger")]
        public string Trigger { get; set; }

        [JsonProperty("replacement")]
        public string Replacement { get; set; }
    }
}
