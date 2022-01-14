namespace Reginald.Core.DataModels
{
    using System;
    using Newtonsoft.Json;

    [Serializable]
    public abstract class KeywordDataModelBase : DataModelBase
    {
        [JsonProperty("guid")]
        public string Guid { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("keyword")]
        public string Keyword { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("format")]
        public string Format { get; set; }

        [JsonProperty("placeholder")]
        public string Placeholder { get; set; }

        [JsonProperty("caption")]
        public string Caption { get; set; }

        [JsonProperty("isEnabled")]
        public bool IsEnabled { get; set; }

        public static bool operator ==(KeywordDataModelBase a, KeywordDataModelBase b)
        {
            return a is not null && b is not null && a.Guid == b.Guid;
        }

        public static bool operator !=(KeywordDataModelBase a, KeywordDataModelBase b)
        {
            return a is not null && b is not null && a.Guid != b.Guid;
        }

        public override bool Equals(object obj)
        {
            return obj is not null && obj is KeywordDataModelBase model && Guid == model.Guid;
        }

        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }
    }
}
