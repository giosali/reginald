namespace Reginald.ViewModels
{
    using Reginald.Data.Keywords;
    using Reginald.Services;

    public class HttpKeywordViewModel : ItemViewModelScreen
    {
        public HttpKeywordViewModel(ConfigurationService configurationService)
            : base(HttpKeyword.Filename, true, "Keywords > HTTP Keywords")
        {
            ConfigurationService = configurationService;
        }

        public ConfigurationService ConfigurationService { get; set; }
    }
}
