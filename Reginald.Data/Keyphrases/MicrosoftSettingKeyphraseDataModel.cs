namespace Reginald.Data.Keyphrases
{
    using Newtonsoft.Json;

    public class MicrosoftSettingKeyphraseDataModel : KeyphraseDataModelBase
    {
        [JsonProperty("uri")]
        public string Uri { get; set; }
    }
}
