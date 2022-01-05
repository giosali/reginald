using Newtonsoft.Json;
using System;

namespace Reginald.Core.Apis.Cloudflare
{
    [Serializable]
    public class CloudflareIpAddress
    {
        [JsonProperty("origin")]
        public string Origin { get; set; }
    }
}
