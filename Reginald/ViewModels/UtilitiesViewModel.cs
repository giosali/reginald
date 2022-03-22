namespace Reginald.ViewModels
{
    using Reginald.Core.IO;
    using Reginald.Data.Keyphrases;
    using Reginald.Services;

    public class UtilitiesViewModel : ItemViewModelBase<Keyphrase>
    {
        public UtilitiesViewModel(ConfigurationService configurationService)
            : base(UtilityKeyphrase.Filename)
        {
            ConfigurationService = configurationService;
            Items.AddRange(KeyphraseFactory.CreateKeyphrases(FileOperations.GetGenericData<UtilityKeyphraseDataModel>(Filename, true)));
        }

        public ConfigurationService ConfigurationService { get; set; }
    }
}
