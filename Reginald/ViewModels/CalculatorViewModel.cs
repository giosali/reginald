namespace Reginald.ViewModels
{
    using Reginald.Services;

    internal class CalculatorViewModel : ItemScreen
    {
        public CalculatorViewModel(DataModelService dms)
            : base("Features > Calculator")
        {
            DataModelService = dms;
        }

        public DataModelService DataModelService { get; set; }
    }
}
