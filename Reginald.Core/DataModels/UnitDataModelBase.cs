namespace Reginald.Core.DataModels
{
    using System;
    using Newtonsoft.Json;

    [Serializable]
    public abstract class UnitDataModelBase : DataModelBase
    {
        [JsonProperty("guid")]
        public string Guid { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        public abstract bool Predicate(string guid);
    }
}
