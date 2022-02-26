namespace Reginald.ViewModels
{
    using System.Linq;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Reginald.Core.Extensions;
    using Reginald.Core.IO;
    using Reginald.Data.Units;

    public class ThemesViewModel : UnitViewModelBase<Theme>
    {
        public ThemesViewModel()
        {
            string filePath = FileOperations.GetFilePath(ApplicationPaths.ThemesJsonFilename, true);
            Units.AddRange(UnitHelper.ToUnits(UpdateData<ThemeDataModel>(filePath, true)));
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
