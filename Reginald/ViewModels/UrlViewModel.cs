namespace Reginald.ViewModels
{
    using Reginald.Services;

    internal sealed class UrlViewModel : ItemScreen
    {
        public UrlViewModel(DataModelService dms)
            : base("Features > URLs")
        {
            DataModelService = dms;
        }

        public DataModelService DataModelService { get; set; }
    }
}
