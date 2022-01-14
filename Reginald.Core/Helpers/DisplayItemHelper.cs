namespace Reginald.Core.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Reginald.Core.AbstractProducts;

    public static class DisplayItemHelper
    {
        public static Task<IEnumerable<DisplayItem>> Filter(List<DisplayItem> items, string input, string expected)
        {
            for (int i = 0; i < items.Count; i++)
            {
                DisplayItem item = items[i];
                if (!item.Predicate())
                {
                    items.RemoveAt(i);
                }
            }

            return Task.FromResult(input.Equals(expected, StringComparison.OrdinalIgnoreCase) ? items : Enumerable.Empty<DisplayItem>());
        }
    }
}
