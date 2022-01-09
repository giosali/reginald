using Newtonsoft.Json;
using System.Collections.Generic;

namespace Reginald.Core.Extensions
{
    public static class IEnumerableExtensions
    {
        public static string Serialize<T>(this IEnumerable<T> collection)
        {
            string json = JsonConvert.SerializeObject(collection, Formatting.Indented);
            return json;
        }
    }
}
