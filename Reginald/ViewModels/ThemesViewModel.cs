namespace Reginald.ViewModels
{
    using System.Linq;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Reginald.Core.DataModels;
    using Reginald.Core.Extensions;
    using Reginald.Core.IO;
    using Reginald.Core.Products;

    public class ThemesViewModel : UnitViewModelBase<Theme>
    {
        public ThemesViewModel()
        {
            Units.AddRange(UpdateUnits<ThemeDataModel>(ApplicationPaths.ThemesJsonFilename, true));
            SelectedUnit = Units.First(u => u.Guid.ToString() == Settings.ThemeIdentifier) as Theme;
        }

        public void ThemesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Settings.ThemeIdentifier = SelectedUnit.Guid.ToString();
            FileOperations.WriteFile(ApplicationPaths.SettingsFilename, Settings.Serialize());
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
