namespace Reginald.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Reginald.Core.AbstractProducts;
    using Reginald.Core.DataModels;
    using Reginald.Core.Extensions;
    using Reginald.Core.IO;
    using Reginald.Core.Products;

    public class ThemesViewModel : UnitViewModelBase<Theme>
    {
        public ThemesViewModel()
        {
            int build = Environment.OSVersion.Version.Build;
            IEnumerable<Unit> units = UpdateUnits<ThemeDataModel>(ApplicationPaths.ThemesJsonFilename, true).Where(unit =>
            {
                Theme theme = unit as Theme;
                if (theme.MinimumBuild >= Theme.Windows11Build)
                {
                    return build >= theme.MinimumBuild;
                }

                return true;
            });
            Units.AddRange(units);
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
