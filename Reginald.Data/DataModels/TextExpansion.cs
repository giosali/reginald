namespace Reginald.Data.DataModels
{
    using Newtonsoft.Json;

    public class TextExpansion
    {
        public const string FileName = "Expansions.json";

        public TextExpansion(string trigger, string replacement)
        {
            Trigger = trigger;
            Replacement = replacement;
        }

        [JsonProperty("replacement")]
        public string Replacement { get; set; }

        [JsonProperty("trigger")]
        public string Trigger { get; set; }
    }
}
