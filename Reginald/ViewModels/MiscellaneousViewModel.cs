namespace Reginald.ViewModels
{
    using Reginald.Data.Keywords;
    using Reginald.Services;

    public class MiscellaneousViewModel : ItemViewModelBase<Keyword>
    {
        public MiscellaneousViewModel(ConfigurationService configurationService)
            : base(null)
        {
            ConfigurationService = configurationService;
        }

        public ConfigurationService ConfigurationService { get; set; }
    }
}
