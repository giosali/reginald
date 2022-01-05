using Newtonsoft.Json;
using Reginald.Core.DataModels;
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

        public static int LongestTriggerLength(this IEnumerable<ExpansionDataModel> models)
        {
            int maxLength = 0;
            foreach (ExpansionDataModel model in models)
            {
                int triggerLength = model.Trigger.Length;
                if (triggerLength > maxLength)
                {
                    maxLength = triggerLength;
                }
            }
            return maxLength;
        }
    }
}
