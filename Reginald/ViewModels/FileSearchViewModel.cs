namespace Reginald.ViewModels
{
    using System.Windows;
    using Caliburn.Micro;
    using Reginald.Services;

    internal sealed class FileSearchViewModel : ItemScreen
    {
        public FileSearchViewModel(DataModelService dms)
            : base("Features > File Search")
        {
            DMS = dms;
        }

        public DataModelService DMS { get; private set; }

        public void ApplyFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            DMS.Settings.Save();
            IoC.Get<ObjectModelService>().ManageFileSystemEntries(DMS.Settings.IsFileSearchEnabled);
        }
    }
}
