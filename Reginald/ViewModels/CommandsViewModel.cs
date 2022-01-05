using Reginald.Core.DataModels;
using Reginald.Core.IO;

namespace Reginald.ViewModels
{
    public class CommandsViewModel : KeywordViewModelBase
    {
        public CommandsViewModel() : base(ApplicationPaths.CommandsJsonFilename)
        {
            Keywords.AddRange(UpdateKeywords<CommandDataModel>(Filename, true, false));
        }
    }
}
