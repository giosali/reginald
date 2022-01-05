using Newtonsoft.Json;
using System;

namespace Reginald.Core.DataModels
{
    [Serializable]
    public class CommandDataModel : KeywordDataModelBase
    {
        [JsonProperty("command")]
        public string Command { get; set; }
    }
}
