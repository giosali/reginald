namespace Reginald.ViewModels
{
    using Reginald.Services;

    internal sealed class MicrosoftSettingsViewModel : ItemScreen
    {
        public MicrosoftSettingsViewModel(DataModelService dms)
            : base("Features > Microsoft Settings")
        {
            DMS = dms;
        }

        public DataModelService DMS { get; set; }
    }
}
