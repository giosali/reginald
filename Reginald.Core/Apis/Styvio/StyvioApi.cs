namespace Reginald.Core.Apis.Styvio
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public class StyvioApi
    {
        private const string StyvioStockEpFormat = "https://www.styvio.com/api/{0}";

        public static async Task<StyvioStock> GetStock(string stock, CancellationToken token)
        {
            try
            {
                using HttpClient client = new();
                HttpResponseMessage response = await client.GetAsync(string.Format(StyvioStockEpFormat, stock), token);
                response.EnsureSuccessStatusCode();
                StyvioStock styvioStock = JsonConvert.DeserializeObject<StyvioStock>(await response.Content.ReadAsStringAsync(token));
                return styvioStock;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }
    }
}
