namespace Reginald.Data.DisplayItems
{
    using System.Collections.Generic;
    using System.Linq;
    using Reginald.Core.Helpers;
    using Reginald.Data.Keywords;

    public class SearchResult : DisplayItem
    {
        private const string ExclamationIconPath = "pack://application:,,,/Reginald;component/Images/Results/exclamation.png";

        private const string ExclamationCaptionFormat = "Confirmation Required ({0})";

        public SearchResult(Item item)
            : base(item)
        {
            if (item is Keyword keyword)
            {
                Keyword = keyword.Word;
            }
        }

        public bool IsPromptShown { get; set; }

        public string Keyword { get; set; }

        public static IEnumerable<SearchResult> FromItems(IEnumerable<Item> items)
        {
            return items.Select(item => new SearchResult(item));
        }

        public override void EnterKeyDown()
        {
            if (RequiresPrompt)
            {
                Guid = System.Guid.NewGuid();
                Icon = BitmapImageHelper.FromUri(ExclamationIconPath);
                Caption = string.Format(ExclamationCaptionFormat, Description);
                Description += "?";
                RequiresPrompt = false;
            }
            else
            {
                base.EnterKeyDown();
            }
        }
    }
}
