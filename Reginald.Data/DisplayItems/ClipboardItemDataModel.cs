namespace Reginald.Data.DisplayItems
{
    using Newtonsoft.Json;

    public class ClipboardItemDataModel
    {
        [JsonProperty("guid")]
        public string Guid { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("dateTime")]
        public string DateTime { get; set; }

        [JsonProperty("clipboardItemType")]
        public int ClipboardItemType { get; set; }
    }
}
