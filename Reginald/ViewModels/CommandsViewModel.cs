namespace Reginald.ViewModels
{
    using Reginald.Core.DataModels;
    using Reginald.Core.IO;

    public class CommandsViewModel : KeywordViewModelBase
    {
        public CommandsViewModel()
            : base(ApplicationPaths.CommandsJsonFilename)
        {
            Keywords.AddRange(UpdateKeywords<CommandDataModel>(Filename, true, false));
        }
    }
}
