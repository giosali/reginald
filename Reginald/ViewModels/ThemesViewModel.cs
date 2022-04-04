namespace Reginald.ViewModels
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using Caliburn.Micro;
    using Reginald.Core.IO;
    using Reginald.Data.Units;
    using Reginald.Messages;
    using Reginald.Services;

    public class ThemesViewModel : ItemsViewModelBase<Theme>
    {
        private readonly IEventAggregator _eventAggregator;

        private readonly ConfigurationService _configurationService;

        public ThemesViewModel(IEventAggregator eventAggregator, ConfigurationService configurationService)
        {
            _eventAggregator = eventAggregator;
            _configurationService = configurationService;

            Filename = Theme.Filename;
            IsResource = true;

            Unit[] units = UnitFactory.CreateUnits(FileOperations.GetGenericData<ThemeDataModel>(Filename, true));
            Items.AddRange(units.OfType<Theme>());
            SelectedItem = Items.FirstOrDefault(i => i.Guid.ToString() == _configurationService.Settings.ThemeIdentifier, Items.First());
        }

        public void ThemesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _configurationService.Settings.ThemeIdentifier = SelectedItem.Guid.ToString();
            _configurationService.Settings.Save();
            _ = _eventAggregator.PublishOnUIThreadAsync(new UpdatePageMessage($"Themes > {SelectedItem.Name}"));
        }

        protected override void UpdateItems() => throw new NotImplementedException();

        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            _ = _eventAggregator.PublishOnUIThreadAsync(new UpdatePageMessage($"Themes > {SelectedItem.Name}"));
            return base.OnActivateAsync(cancellationToken);
        }
    }
}
