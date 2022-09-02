namespace Reginald.ViewModels
{
    using System.Windows;
    using System.Windows.Controls;
    using Reginald.Services;
    using Reginald.Services.Utilities;

    internal class TimerViewModel : ItemScreen
    {
        public TimerViewModel(DataModelService dms)
            : base("Features > Timer")
        {
            DataModelService = dms;
        }

        public DataModelService DataModelService { get; set; }

        public void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUtility.GoTo((sender as Button).Tag.ToString());
        }
    }
}
