namespace Reginald.ViewModels
{
    using Reginald.Services;

    internal class RecycleViewModel : ItemScreen
    {
        public RecycleViewModel(DataModelService dms)
            : base("Features > Recycle")
        {
            DataModelService = dms;
        }

        public DataModelService DataModelService { get; set; }
    }
}
