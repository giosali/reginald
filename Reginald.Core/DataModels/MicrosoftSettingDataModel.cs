using Newtonsoft.Json;
using System;

namespace Reginald.Core.DataModels
{
    [Serializable]
    public class MicrosoftSettingDataModel : KeyphraseDataModelBase
    {
        [JsonProperty("uri")]
        public string Uri { get; set; }
    }
}
