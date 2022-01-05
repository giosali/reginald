using Reginald.Core.AbstractProducts;
using Reginald.Core.DataModels;
using Reginald.Core.IO;
using System.Collections.Generic;

namespace Reginald.ViewModels
{
    public class DefaultKeywordViewModel : KeywordViewModelBase
    {
        public DefaultKeywordViewModel() : base(ApplicationPaths.KeywordsJsonFilename)
        {
            IEnumerable<Keyword> keywords = UpdateKeywords<GenericKeywordDataModel>(Filename, true, false);
            foreach (Keyword keyword in keywords)
            {
                keyword.Description = string.Format(keyword.Format, keyword.Placeholder);
            }
            Keywords.AddRange(keywords);
        }
    }
}
