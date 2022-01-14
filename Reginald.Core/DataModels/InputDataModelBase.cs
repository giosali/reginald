namespace Reginald.Core.DataModels
{
    using Newtonsoft.Json;

    public abstract class InputDataModelBase : DataModelBase
    {
        [JsonProperty("guid")]
        public string Guid { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("caption")]
        public string Caption { get; set; }

        [JsonProperty("altCaption")]
        public string AltCaption { get; set; }

        [JsonProperty("isEnabled")]
        public bool IsEnabled { get; set; }
    }
}
