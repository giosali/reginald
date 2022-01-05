using Reginald.Core.AbstractProducts;
using Reginald.Core.Clients;
using Reginald.Core.Factories;

namespace Reginald.Core.Helpers
{
    public static class RepresentationHelper
    {
        public static ResultFactory ResultFactory { get; set; } = new();

        public static DisplayItem ToSearchResult(Representation representation)
        {
            DisplayItemClient client = new(ResultFactory, representation);
            return client.Item;
        }
    }
}
