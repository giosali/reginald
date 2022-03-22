namespace Reginald.ViewModels
{
    using Reginald.Core.IO;
    using Reginald.Data.Keywords;
    using Reginald.Services;

    public class CommandsViewModel : ItemViewModelBase<CommandKeywordDataModel>
    {
        public CommandsViewModel(ConfigurationService configurationService)
            : base(CommandKeyword.Filename)
        {
            ConfigurationService = configurationService;
            CommandKeywordDataModel[] models = FileOperations.GetGenericData<CommandKeywordDataModel>(Filename, true);
            Items.AddRange(models);
        }

        public ConfigurationService ConfigurationService { get; set; }
    }
}
