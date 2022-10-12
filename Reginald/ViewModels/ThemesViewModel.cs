namespace Reginald.ViewModels
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using Caliburn.Micro;
    using Reginald.Core.IO;
    using Reginald.Messages;
    using Reginald.Models.DataModels;
    using Reginald.Services;

    internal sealed class ThemesViewModel : ItemsScreen<Theme>
    {
        public ThemesViewModel()
            : base("Themes")
        {
            int themeId = IoC.Get<DataModelService>().Settings.ThemeId;
            Items.AddRange(FileOperations.GetGenericData<Theme>(Theme.FileName, true));
            SelectedItem = Items.FirstOrDefault(theme => theme.Id == themeId, Items.First());
        }

        public override void IsEnabled_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void ThemesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataModelService dms = IoC.Get<DataModelService>();
            dms.Settings.ThemeId = SelectedItem.Id;
            dms.Settings.Save();
            _ = IoC.Get<IEventAggregator>().PublishOnUIThreadAsync(new UpdatePageMessage($"Themes > {SelectedItem?.Name}"));
        }

        protected override void OnViewLoaded(object view)
        {
            _ = IoC.Get<IEventAggregator>().PublishOnUIThreadAsync(new UpdatePageMessage($"Themes > {SelectedItem?.Name}"));
            base.OnViewLoaded(view);
        }
    }
}
