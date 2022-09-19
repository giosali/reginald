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
        private readonly IEventAggregator _eventAggregator;

        private readonly DataModelService _dms;

        public ThemesViewModel(IEventAggregator eventAggregator, DataModelService dms)
            : base("Themes")
        {
            _eventAggregator = eventAggregator;
            _dms = dms;

            Items.AddRange(FileOperations.GetGenericData<Theme>(Theme.FileName, true));
            SelectedItem = Items.FirstOrDefault(theme => theme.Id == _dms.Settings.ThemeId, Items.First());
        }

        public override void IsEnabled_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void ThemesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _dms.Settings.ThemeId = SelectedItem.Id;
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
