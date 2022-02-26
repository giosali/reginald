namespace Reginald.ViewModels
{
    using Reginald.Core.IO;
    using Reginald.Data.Keywords;

    public class HttpKeywordsViewModel : KeywordViewModelBase
    {
        public HttpKeywordsViewModel()
            : base(ApplicationPaths.HttpKeywordsJsonFilename, true)
        {
            Keywords.AddRange(KeywordHelper.ToKeywords(UpdateData<HttpKeywordDataModel>(FilePath, true)));
        }
    }
}
