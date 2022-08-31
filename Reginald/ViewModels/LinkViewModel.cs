namespace Reginald.ViewModels
{
    using Reginald.Services;

    internal class LinkViewModel : ItemScreen
    {
        public LinkViewModel(DataModelService dms)
            : base("Features > Link")
        {
            DataModelService = dms;
        }

        public DataModelService DataModelService { get; set; }
    }
}
