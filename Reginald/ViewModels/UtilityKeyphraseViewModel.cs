namespace Reginald.ViewModels
{
    using Reginald.Data.Keyphrases;
    using Reginald.Services;

    public class UtilityKeyphraseViewModel : ItemViewModelBase
    {
        public UtilityKeyphraseViewModel(ConfigurationService configurationService)
            : base(UtilityKeyphrase.Filename, true, "Keyphrases > Utilities")
        {
            ConfigurationService = configurationService;
        }

        public ConfigurationService ConfigurationService { get; set; }
    }
}
