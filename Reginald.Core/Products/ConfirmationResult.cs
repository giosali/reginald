namespace Reginald.Core.Products
{
    using Reginald.Core.AbstractProducts;
    using Reginald.Core.Helpers;

    public class ConfirmationResult : SearchResult
    {
        private const string ConfirmationIconPath = "pack://application:,,,/Reginald;component/Images/Results/exclamation.png";

        private const string ConfirmationCaption = "Confirmation Required - This Action Cannot Be Undone";

        public ConfirmationResult(Keyword keyword)
        {
            Guid = System.Guid.NewGuid();
            Name = keyword.Name;
            Icon = BitmapImageHelper.FromUri(ConfirmationIconPath);
            Caption = ConfirmationCaption;
            Description = keyword.Description + "?";
            Keyword = keyword;
            IsPrompted = true;
        }

        public ConfirmationResult(Keyphrase keyphrase)
        {
            Guid = System.Guid.NewGuid();
            Name = keyphrase.Name;
            Icon = BitmapImageHelper.FromUri(ConfirmationIconPath);
            Caption = ConfirmationCaption;
            Description = keyphrase.Description + "?";
            Keyphrase = keyphrase;
            IsPrompted = true;
        }
    }
}
