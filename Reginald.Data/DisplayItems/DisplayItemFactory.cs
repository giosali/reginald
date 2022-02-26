namespace Reginald.Data.DisplayItems
{
    using System.Collections.Generic;
    using Reginald.Data.Keyphrases;
    using Reginald.Data.Keywords;
    using Reginald.Data.Representations;
    using Reginald.Data.ShellItems;

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
