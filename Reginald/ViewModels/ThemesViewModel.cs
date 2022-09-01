namespace Reginald.ViewModels
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using Caliburn.Micro;
    using Reginald.Core.IO;
    using Reginald.Data.DataModels;
    using Reginald.Messages;
    using Reginald.Services;

    internal class ThemesViewModel : ItemsScreen<Theme>
    {
        private readonly IEventAggregator _eventAggregator;

        private readonly DataModelService _dms;

        public ThemesViewModel(IEventAggregator eventAggregator, DataModelService dms)
            : base("Themes")
        {
            _eventAggregator = eventAggregator;
            _dms = dms;

            Items.AddRange(FileOperations.GetGenericData<Theme>(Theme.FileName, true));
            SelectedItem = Items.FirstOrDefault(theme => theme.Guid == _dms.Settings.ThemeIdentifier, Items.First());
        }

        public override void IsEnabled_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void ThemesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _dms.Settings.ThemeIdentifier = SelectedItem.Guid;
            _dms.Settings.Save();
            _ = _eventAggregator.PublishOnUIThreadAsync(new UpdatePageMessage($"Themes > {SelectedItem?.Name}"));
        }

        protected override void OnViewLoaded(object view)
        {
            _ = _eventAggregator.PublishOnUIThreadAsync(new UpdatePageMessage($"Themes > {SelectedItem?.Name}"));
            base.OnViewLoaded(view);
        }
    }
}
