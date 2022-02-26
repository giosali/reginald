namespace Reginald.Data.DisplayItems
{
    using System.Collections.Generic;
    using System.Linq;
    using Reginald.Data.Keyphrases;
    using Reginald.Data.Keywords;
    using Reginald.Data.Representations;
    using Reginald.Data.ShellItems;

    public class ResultFactory : DisplayItemFactory
    {
        public override DisplayItem CreateDisplayItem(Keyword keyword)
        {
            // Check to see if the TimerKeyword has been signaled to start by keeping
            // track of its IsRunning property. We also use this to differentiate it
            // from TimerKeywords that haven't been signaled.
            if (keyword is TimerKeyword timerKeyword)
            {
                if (timerKeyword.IsRunning)
                {
                    TimerResult result = new(timerKeyword);
                    result.StartTimer();
                    return result;
                }

                return null;
            }
            else
            {
                return new SearchResult(keyword);
            }
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
