namespace Reginald.Core.DataModels
{
    using System;
    using Newtonsoft.Json;

    [Serializable]
    public class CommandDataModel : KeywordDataModelBase
    {
        [JsonProperty("command")]
        public string Command { get; set; }
    }
}
