using Reginald.Services;

namespace Reginald.ViewModels
{
    internal sealed class FileSearchViewModel : ItemScreen
    {
        public FileSearchViewModel(DataModelService dms)
            : base("Features > File Search")
        {
            DMS = dms;
        }

        public DataModelService DMS { get; set; }
    }
}
