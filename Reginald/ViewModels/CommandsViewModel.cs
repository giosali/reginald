namespace Reginald.ViewModels
{
    using System.Collections.Generic;
    using Reginald.Core.IO;
    using Reginald.Data.Keywords;

    public class CommandsViewModel : KeywordViewModelBase
    {
        public CommandsViewModel()
            : base(ApplicationPaths.CommandsJsonFilename, true)
        {
            Keywords.AddRange(KeywordHelper.ToKeywords(UpdateData<CommandKeywordDataModel>(FilePath, IsResource)));
        }
    }
}
