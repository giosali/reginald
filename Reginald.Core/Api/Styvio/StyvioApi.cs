using Newtonsoft.Json;
using Reginald.Core.Base;
using System.Net.Http;
using System.Threading.Tasks;

namespace Reginald.Core.Api.Styvio
{
    public class StyvioApi
    {
        public static async Task<StyvioStock> GetStock(string stock)
        {
            try
            {
                using (HttpClient client = new())
                {
                    HttpResponseMessage response = await client.GetAsync(string.Format(Constants.StyvioStockEpFormat, stock));
                    response.EnsureSuccessStatusCode();
                    StyvioStock styvioStock = JsonConvert.DeserializeObject<StyvioStock>(await response.Content.ReadAsStringAsync());
                    return styvioStock;
                    //IEnumerable<string> propertyNames = typeof(StyvioStock).GetProperties()
                    //                                                       .Select(p => p.GetCustomAttribute<JsonPropertyAttribute>())
                    //                                                       .Select(a => a.PropertyName);
                }
            }
            catch (HttpRequestException)
            {
                return new StyvioStock();
            }
        }
    }
}
