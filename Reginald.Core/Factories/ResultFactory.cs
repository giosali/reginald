﻿using Reginald.Core.AbstractFactories;
using Reginald.Core.AbstractProducts;
using Reginald.Core.Products;
using System.Collections.Generic;
using System.Linq;

namespace Reginald.Core.Factories
{
    public class ResultFactory : DisplayItemFactory
    {
        public override DisplayItem CreateDisplayItem(Keyword keyword)
        {
            return new SearchResult(keyword);
        }

        public override DisplayItem CreateDisplayItem(Representation representation)
        {
            return new SearchResult(representation);
        }

        public override DisplayItem CreateDisplayItem(Keyphrase keyphrase)
        {
            return new ConfirmationResult(keyphrase);
        }

        public override IEnumerable<DisplayItem> CreateDisplayItems(IEnumerable<Keyword> keywords)
        {
            List<DisplayItem> displayItems = new(keywords.Count());
            foreach (Keyword keyword in keywords)
            {
                displayItems.Add(new SearchResult(keyword));
            }
            return displayItems;
        }

        public override IEnumerable<DisplayItem> CreateDisplayItems(IEnumerable<ShellItem> items)
        {
            List<DisplayItem> displayItems = new(items.Count());
            foreach (ShellItem item in items)
            {
                displayItems.Add(new SearchResult(item));
            }
            return displayItems;
        }

        public override IEnumerable<DisplayItem> CreateDisplayItems(IEnumerable<Keyphrase> phrases)
        {
            List<DisplayItem> displayItems = new(phrases.Count());
            foreach (Keyphrase phrase in phrases)
            {
                displayItems.Add(new SearchResult(phrase));
            }
            return displayItems;
        }
    }
}
