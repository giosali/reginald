namespace Reginald.ViewModels
{
    using System.Collections.Generic;
    using Reginald.Core.IO;
    using Reginald.Data.Keywords;

    public class DefaultKeywordViewModel : KeywordViewModelBase
    {
        public DefaultKeywordViewModel()
            : base(ApplicationPaths.KeywordsJsonFilename, true)
        {
            IEnumerable<Keyword> keywords = KeywordHelper.ToKeywords(UpdateData<GenericKeywordDataModel>(FilePath, IsResource));
            foreach (Keyword keyword in keywords)
            {
                keyword.Description = string.Format(keyword.Format, keyword.Placeholder);
            }

            Keywords.AddRange(keywords);
        }
    }
}
