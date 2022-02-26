namespace Reginald.Data.Representations
{
    using Reginald.Data.DisplayItems;

    public static class RepresentationHelper
    {
        public static DisplayItem ToSearchResult(Representation representation)
        {
            ResultFactory factory = new();
            DisplayItemClient client = new(factory, representation);
            return client.Item;
        }

        public static Representation ToRepresentation(RepresentationDataModelBase model)
        {
            RepresentationFactory factory = new();
            RepresentationClient client = new(factory, model);
            return client.Representation;
        }
    }
}
