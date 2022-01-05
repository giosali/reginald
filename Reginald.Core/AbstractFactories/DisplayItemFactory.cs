using Reginald.Core.AbstractProducts;
using System.Collections.Generic;

namespace Reginald.Core.AbstractFactories
{
    public abstract class DisplayItemFactory
    {
        public abstract DisplayItem CreateDisplayItem(Keyword keyword);

        public abstract DisplayItem CreateDisplayItem(Representation representation);

        public abstract DisplayItem CreateDisplayItem(Keyphrase keyphrase);

        public abstract IEnumerable<DisplayItem> CreateDisplayItems(IEnumerable<Keyword> keywords);

        public abstract IEnumerable<DisplayItem> CreateDisplayItems(IEnumerable<ShellItem> items);

        public abstract IEnumerable<DisplayItem> CreateDisplayItems(IEnumerable<Keyphrase> phrases);
    }
}
