using Newtonsoft.Json;
using System;

namespace Reginald.Core.Api.Cloudflare
{
    [Serializable]
    public class CloudflareIpAddress
    {
        [JsonProperty("origin")]
        public string Origin { get; set; }
    }
}
