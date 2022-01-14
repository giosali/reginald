namespace Reginald.ViewModels
{
    using Reginald.Core.DataModels;
    using Reginald.Core.IO;

    public class HttpKeywordsViewModel : KeywordViewModelBase
    {
        public HttpKeywordsViewModel()
            : base(ApplicationPaths.HttpKeywordsJsonFilename)
        {
            Keywords.AddRange(UpdateKeywords<HttpKeywordDataModel>(Filename, true, false));
        }
    }
}
