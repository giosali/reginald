namespace Reginald.Data.ObjectModels
{
    using System;
    using Newtonsoft.Json;

    public abstract class ObjectModel
    {
        [JsonProperty("caption")]
        public string Caption { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("guid")]
        public Guid Guid { get; set; }

        [JsonProperty("iconPath")]
        public string IconPath { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
