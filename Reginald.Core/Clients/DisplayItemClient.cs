namespace Reginald.Core.Clients
{
    using System.Collections.Generic;
    using Reginald.Core.AbstractFactories;
    using Reginald.Core.AbstractProducts;

    public class DisplayItemClient
    {
        public DisplayItemClient(DisplayItemFactory factory, Keyword keyword)
        {
            Item = factory.CreateDisplayItem(keyword);
        }

        public DisplayItemClient(DisplayItemFactory factory, IEnumerable<Keyword> keywords)
        {
            Items = factory.CreateDisplayItems(keywords);
        }

        public DisplayItemClient(DisplayItemFactory factory, IEnumerable<ShellItem> items)
        {
            Items = factory.CreateDisplayItems(items);
        }

        public DisplayItemClient(DisplayItemFactory factory, Representation representation)
        {
            Item = factory.CreateDisplayItem(representation);
        }

        public DisplayItemClient(DisplayItemFactory factory, Keyphrase keyphrase)
        {
            Item = factory.CreateDisplayItem(keyphrase);
        }

        public DisplayItemClient(DisplayItemFactory factory, IEnumerable<Keyphrase> phrases)
        {
            Items = factory.CreateDisplayItems(phrases);
        }

        public DisplayItem Item { get; set; }

        public IEnumerable<DisplayItem> Items { get; set; }
    }
}
