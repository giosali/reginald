using Newtonsoft.Json;
using System;

namespace Reginald.Core.DataModels
{
    [Serializable]
    public abstract class UnitDataModelBase : DataModelBase
    {
        [JsonProperty("guid")]
        public string Guid { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        public abstract bool Predicate(UnitDataModelBase model, string guid);
    }
}
