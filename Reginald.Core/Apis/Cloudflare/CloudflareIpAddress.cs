namespace Reginald.Core.Apis.Cloudflare
{
    using System;
    using Newtonsoft.Json;

    [Serializable]
    public class CloudflareIpAddress
    {
        [JsonProperty("origin")]
        public string Origin { get; set; }
    }
}
