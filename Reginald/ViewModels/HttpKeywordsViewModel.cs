namespace Reginald.ViewModels
{
    using Reginald.Core.IO;
    using Reginald.Data.Keywords;
    using Reginald.Services;

    public class HttpKeywordsViewModel : ItemViewModelBase<Keyword>
    {
        public HttpKeywordsViewModel(ConfigurationService configurationService)
            : base(HttpKeyword.Filename)
        {
            ConfigurationService = configurationService;
            Items.AddRange(KeywordFactory.CreateKeywords(FileOperations.GetGenericData<HttpKeywordDataModel>(Filename, true)));
        }

        public ConfigurationService ConfigurationService { get; set; }
    }
}
