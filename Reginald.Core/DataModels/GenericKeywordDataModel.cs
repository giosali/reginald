using Newtonsoft.Json;

namespace Reginald.Core.DataModels
{
    public class GenericKeywordDataModel : KeywordDataModelBase
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("altUrl")]
        public string AltUrl { get; set; }

        [JsonProperty("separator")]
        public string Separator { get; set; }

        [JsonProperty("altDescription")]
        public string AltDescription { get; set; }
    }
}
