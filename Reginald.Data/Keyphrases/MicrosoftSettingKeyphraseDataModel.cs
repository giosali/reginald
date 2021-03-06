namespace Reginald.Data.Keyphrases
{
    using Newtonsoft.Json;

    public class MicrosoftSettingKeyphraseDataModel : DataModelBase, IKeyphraseDataModel
    {
        public string Name { get; set; }

        public string Keyphrase { get; set; }

        public string Icon { get; set; }

        public string Caption { get; set; }

        public bool IsEnabled { get; set; }

        public string Description { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; }
    }
}
