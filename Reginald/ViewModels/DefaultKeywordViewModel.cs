namespace Reginald.ViewModels
{
    using System.Collections.Generic;
    using Reginald.Core.IO;
    using Reginald.Data.Keywords;
    using Reginald.Services;

    public class DefaultKeywordViewModel : ItemViewModelBase<Keyword>
    {
        public DefaultKeywordViewModel(ConfigurationService configurationService)
            : base(GenericKeyword.KeywordsFilename)
        {
            ConfigurationService = configurationService;
            IEnumerable<Keyword> keywords = KeywordFactory.CreateKeywords(FileOperations.GetGenericData<GenericKeywordDataModel>(Filename, true));
            foreach (Keyword keyword in keywords)
            {
                keyword.Description = string.Format(keyword.Format, keyword.Placeholder);
            }

            Items.AddRange(keywords);
        }

        public ConfigurationService ConfigurationService { get; set; }
    }
}
