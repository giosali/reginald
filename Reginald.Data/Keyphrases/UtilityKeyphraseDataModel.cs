namespace Reginald.Data.Keyphrases
{
    using Newtonsoft.Json;

    public class UtilityKeyphraseDataModel : DataModelBase, IKeyphraseDataModel
    {
        public string Name { get; set; }

        public string Keyphrase { get; set; }

        public string Icon { get; set; }

        public string Caption { get; set; }

        public bool IsEnabled { get; set; }

        public string Description { get; set; }

        [JsonProperty("altDescription")]
        public string AltDescription { get; set; }

        [JsonProperty("requiresPrompt")]
        public bool RequiresPrompt { get; set; }

        [JsonProperty("utility")]
        public string Utility { get; set; }
    }
}
