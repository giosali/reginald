namespace Reginald.Core.Helpers
{
    using Reginald.Core.AbstractProducts;
    using Reginald.Core.Clients;
    using Reginald.Core.Factories;

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
