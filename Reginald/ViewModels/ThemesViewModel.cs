namespace Reginald.ViewModels
{
    using System.Linq;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Reginald.Core.IO;
    using Reginald.Data.Units;
    using Reginald.Services;

    public class ThemesViewModel : ItemViewModelBase<Theme>
    {
        private readonly ConfigurationService _configurationService;

        public ThemesViewModel(ConfigurationService configurationService)
            : base(Theme.Filename)
        {
            _configurationService = configurationService;
            Unit[] units = UnitFactory.CreateUnits(FileOperations.GetGenericData<ThemeDataModel>(Filename, true));
            Items.AddRange(units.OfType<Theme>().ToArray());
            SelectedItem = Items.FirstOrDefault(u => u.Guid.ToString() == _configurationService.Settings.ThemeIdentifier, Items.First());
        }

        public void ThemesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _configurationService.Settings.ThemeIdentifier = SelectedItem.Guid.ToString();
            _configurationService.Settings.Save();
        }

        public void ThemesListBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
            }
        }
    }
}
