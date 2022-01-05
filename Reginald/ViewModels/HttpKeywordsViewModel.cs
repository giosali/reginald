using Reginald.Core.DataModels;
using Reginald.Core.IO;

namespace Reginald.ViewModels
{
    public class HttpKeywordsViewModel : KeywordViewModelBase
    {
        public HttpKeywordsViewModel() : base(ApplicationPaths.HttpKeywordsJsonFilename)
        {
            Keywords.AddRange(UpdateKeywords<HttpKeywordDataModel>(Filename, true, false));
        }
    }
}
