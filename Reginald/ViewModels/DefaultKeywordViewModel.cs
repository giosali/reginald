namespace Reginald.ViewModels
{
    using Reginald.Data.Keywords;
    using Reginald.Services;

    public class DefaultKeywordViewModel : ItemViewModelScreen
    {
        public DefaultKeywordViewModel(ConfigurationService configurationService)
            : base(GenericKeyword.KeywordsFilename, true, "Keywords > Default Keywords")
        {
            ConfigurationService = configurationService;
        }

        public ConfigurationService ConfigurationService { get; set; }
    }
}
