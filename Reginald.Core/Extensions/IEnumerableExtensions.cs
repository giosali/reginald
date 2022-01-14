namespace Reginald.Core.Extensions
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public static class IEnumerableExtensions
    {
        public static string Serialize<T>(this IEnumerable<T> collection)
        {
            string json = JsonConvert.SerializeObject(collection, Formatting.Indented);
            return json;
        }
    }
}
