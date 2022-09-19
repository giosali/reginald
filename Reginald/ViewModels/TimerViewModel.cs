namespace Reginald.ViewModels
{
    using Reginald.Services;

    internal sealed class TimerViewModel : ItemScreen
    {
        public TimerViewModel(DataModelService dms)
            : base("Features > Timer")
        {
            DataModelService = dms;
        }

        public DataModelService DataModelService { get; set; }
    }
}
