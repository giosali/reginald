namespace Reginald.Core.Apis.Cloudflare
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Reginald.Core.Base;

    public class CloudflareApi
    {
        private const string CloudflareEp = "https://cloudflare-quic.com/b/ip";
        
        public static async Task<CloudflareIpAddress> GetIpAddress(CancellationToken token)
        {
            try
            {
                using HttpClient client = new();
                HttpResponseMessage response = await client.GetAsync(CloudflareEp, token);
                _ = response.EnsureSuccessStatusCode();
                CloudflareIpAddress ipAddress = JsonConvert.DeserializeObject<CloudflareIpAddress>(await response.Content.ReadAsStringAsync(token));
                return ipAddress;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }
    }
}
