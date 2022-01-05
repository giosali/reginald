using Reginald.Core.AbstractProducts;
using Reginald.Core.Base;
using Reginald.Core.Helpers;

namespace Reginald.Core.Products
{
    public class ConfirmationResult : SearchResult
    {
        public ConfirmationResult(Keyword keyword)
        {
            Guid = System.Guid.NewGuid();
            Name = keyword.Name;
            Icon = BitmapImageHelper.FromUri(Constants.ConfirmationIconPath);
            Caption = Constants.ConfirmationCaption;
            Description = keyword.Description + "?";
            Keyword = keyword;
            IsPrompted = true;
        }

        public ConfirmationResult(Keyphrase keyphrase)
        {
            Guid = System.Guid.NewGuid();
            Name = keyphrase.Name;
            Icon = BitmapImageHelper.FromUri(Constants.ConfirmationIconPath);
            Caption = Constants.ConfirmationCaption;
            Description = keyphrase.Description + "?";
            Keyphrase = keyphrase;
            IsPrompted = true;
        }
    }
}
