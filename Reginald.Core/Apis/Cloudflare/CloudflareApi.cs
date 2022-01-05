using Newtonsoft.Json;
using Reginald.Core.Base;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Reginald.Core.Apis.Cloudflare
{
    public class CloudflareApi
    {
        public static async Task<CloudflareIpAddress> GetIpAddress()
        {
            try
            {
                using HttpClient client = new();
                HttpResponseMessage response = await client.GetAsync(Constants.CloudflareEp);
                _ = response.EnsureSuccessStatusCode();
                CloudflareIpAddress ipAddress = JsonConvert.DeserializeObject<CloudflareIpAddress>(await response.Content.ReadAsStringAsync());
                return ipAddress;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public static async Task<CloudflareIpAddress> GetIpAddress(CancellationToken token)
        {
            try
            {
                using HttpClient client = new();
                HttpResponseMessage response = await client.GetAsync(Constants.CloudflareEp, token);
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
