namespace Reginald.Core.DataModels
{
    using System;
    using Newtonsoft.Json;

    [Serializable]
    public class HttpKeywordDataModel : KeywordDataModelBase
    {
        [JsonProperty("primaryIcon")]
        public string PrimaryIcon { get; set; }

        [JsonProperty("auxiliaryIcon")]
        public string AuxiliaryIcon { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("captionFormat")]
        public string CaptionFormat { get; set; }

        [JsonProperty("api")]
        public string Api { get; set; }
    }
}
