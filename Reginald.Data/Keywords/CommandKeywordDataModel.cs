namespace Reginald.Data.Keywords
{
    using Newtonsoft.Json;

    public class CommandKeywordDataModel : KeywordDataModelBase
    {
        [JsonProperty("command")]
        public string Command { get; set; }
    }
}
