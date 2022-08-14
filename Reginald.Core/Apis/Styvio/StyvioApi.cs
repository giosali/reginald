namespace Reginald.Core.Apis.Styvio
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Reginald.Core.Base;

    public class StyvioApi
    {
        public static async Task<StyvioStock> GetStock(string stock, CancellationToken token)
        {
            try
            {
                using HttpClient client = new();
                HttpResponseMessage response = await client.GetAsync(string.Format(Constants.StyvioStockEpFormat, stock), token);
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
