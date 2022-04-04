namespace Reginald.Data.Keyphrases
{
    /// <summary>
    /// Represents the contents of a Keyphrase JSON file to use with Newtonsoft.Json.
    /// </summary>
    public class KeyphraseDataModel : IKeyphraseDataModel
    {
        public string Guid { get; set; }

        public string Name { get; set; }

        public string Keyphrase { get; set; }

        public string Icon { get; set; }

        public string Caption { get; set; }

        public bool IsEnabled { get; set; }

        public string Description { get; set; }

        public string DisplayDescription { get; set; }
    }
}
