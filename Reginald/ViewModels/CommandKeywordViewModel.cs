namespace Reginald.ViewModels
{
    using Reginald.Data.Keywords;
    using Reginald.Services;

    public class CommandKeywordViewModel : ItemViewModelScreen
    {
        public CommandKeywordViewModel(ConfigurationService configurationService)
            : base(CommandKeyword.Filename, true, "Keywords > Command Keywords")
        {
            ConfigurationService = configurationService;
        }

        public ConfigurationService ConfigurationService { get; set; }
    }
}
