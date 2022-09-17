namespace Reginald.Models.DataModels
{
    using Newtonsoft.Json;

    public abstract class DataModel
    {
        [JsonProperty("caption")]
        public string Caption { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("iconPath")]
        public string IconPath { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
