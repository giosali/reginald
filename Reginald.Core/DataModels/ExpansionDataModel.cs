namespace Reginald.Core.DataModels
{
    using System;
    using Newtonsoft.Json;

    [Serializable]
    public class ExpansionDataModel : DataModelBase
    {
        [JsonProperty("trigger")]
        public string Trigger { get; set; }

        [JsonProperty("replacement")]
        public string Replacement { get; set; }
    }
}
