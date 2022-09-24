namespace Reginald.ViewModels
{
    using Reginald.Services;

    internal sealed class QuitViewModel : ItemScreen
    {
        public QuitViewModel(DataModelService dms)
            : base("Features > Quit")
        {
            DMS = dms;
        }

        public DataModelService DMS { get; set; }
    }
}
