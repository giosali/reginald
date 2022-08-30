namespace Reginald.ViewModels
{
    using Reginald.Services;

    internal class ApplicationsViewModel : ItemScreen
    {
        public ApplicationsViewModel(DataModelService dms)
            : base("Features > Applications")
        {
            DataModelService = dms;
        }

        public DataModelService DataModelService { get; set; }
    }
}
