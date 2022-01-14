namespace Reginald.Core.DataModels
{
    using System;
    using Newtonsoft.Json;

    [Serializable]
    public class MicrosoftSettingDataModel : KeyphraseDataModelBase
    {
        [JsonProperty("uri")]
        public string Uri { get; set; }
    }
}
