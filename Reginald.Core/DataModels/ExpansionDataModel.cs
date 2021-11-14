using Newtonsoft.Json;
using System;

namespace Reginald.Core.DataModels
{
    [Serializable]
    public class ExpansionDataModel : DataModelBase
    {
        [JsonProperty("trigger")]
        public string Trigger { get; set; }

        [JsonProperty("replacement")]
        public string Replacement { get; set; }
    }
}
