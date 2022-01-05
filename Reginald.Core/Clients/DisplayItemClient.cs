using Reginald.Core.AbstractFactories;
using Reginald.Core.AbstractProducts;
using System.Collections.Generic;

namespace Reginald.Core.Clients
{
    public class DisplayItemClient
    {
        public DisplayItem Item { get; set; }

        public IEnumerable<DisplayItem> Items { get; set; }

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
    }
}
