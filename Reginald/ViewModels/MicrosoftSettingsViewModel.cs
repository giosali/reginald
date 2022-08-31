namespace Reginald.ViewModels
{
    using Reginald.Services;

    internal class MicrosoftSettingsViewModel : ItemScreen
    {
        public MicrosoftSettingsViewModel(DataModelService dms)
            : base("Features > Microsoft Settings")
        {
            DataModelService = dms;
        }

        public DataModelService DataModelService { get; set; }
    }
}
