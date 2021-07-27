using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Reginald.Core.Api.Styvio
{
    [Serializable]
    public class StyvioStock
    {
        [JsonProperty("ticker")]
        public string Ticker { get; set; }

        [JsonProperty("shortName")]
        public string ShortName { get; set; }

        [JsonProperty("companyLocation")]
        public string CompanyLocation { get; set; }

        [JsonProperty("currentPrice")]
        public string CurrentPrice { get; set; }

        [JsonProperty("percentText")]
        public string PercentText { get; set; }

        [JsonProperty("dailyPrices")]
        public List<float> DailyPrices { get; set; }
    }
}
