namespace Reginald.ViewModels
{
    using Reginald.Services;

    internal class QuitViewModel : ItemScreen
    {
        public QuitViewModel(DataModelService dms)
            : base("Features > Quit")
        {
            DataModelService = dms;
        }

        public DataModelService DataModelService { get; set; }
    }
}
