using Reginald.Core.Extensions;
using Reginald.Core.IO;
using Reginald.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Reginald.ViewModels
{
    public class GeneralViewModel : ViewViewModelBase
    {
        public IEnumerable<Key> Keys { get; set; }

        public IEnumerable<ModifierKeys> ModifierKeys { get; set; }

        private Key _selectedKey;
        public Key SelectedKey
        {
            get => _selectedKey;
            set
            {
                _selectedKey = value;
                NotifyOfPropertyChange(() => SelectedKey);
            }
        }

        private ModifierKeys _selectedModifierKeyOne;
        public ModifierKeys SelectedModifierKeyOne
        {
            get => _selectedModifierKeyOne;
            set
            {
                _selectedModifierKeyOne = value;
                NotifyOfPropertyChange(() => SelectedModifierKeyOne);
            }
        }

        private ModifierKeys _selectedModifierKeyTwo;
        public ModifierKeys SelectedModifierKeyTwo
        {
            get => _selectedModifierKeyTwo;
            set
            {
                _selectedModifierKeyTwo = value;
                NotifyOfPropertyChange(() => SelectedModifierKeyTwo);
            }
        }

        public GeneralViewModel()
        {
            SelectedKey = (Key)Enum.Parse(typeof(Key), Settings.SearchBoxKey);
            SelectedModifierKeyOne = (ModifierKeys)Enum.Parse(typeof(ModifierKeys), Settings.SearchBoxModifierOne);
            SelectedModifierKeyTwo = (ModifierKeys)Enum.Parse(typeof(ModifierKeys), Settings.SearchBoxModifierTwo);
        }

        public void SelectedKey_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedKey != Key.None)
            {
                Settings.SearchBoxKey = SelectedKey.ToString();
                FileOperations.WriteFile(ApplicationPaths.SettingsFilename, Settings.Serialize());
            }
        }

        public void SelectedModifierKeyOne_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Settings.SearchBoxModifierOne = SelectedModifierKeyOne.ToString();
            FileOperations.WriteFile(ApplicationPaths.SettingsFilename, Settings.Serialize());
        }

        public void SelectedModifierKeyTwo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Settings.SearchBoxModifierTwo = SelectedModifierKeyTwo.ToString();
            FileOperations.WriteFile(ApplicationPaths.SettingsFilename, Settings.Serialize());
        }

        public void LaunchOnStartupToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (!FileOperations.TryCreateShortcut())
            {
                FileOperations.DeleteShortcut();
            }
            FileOperations.WriteFile(ApplicationPaths.SettingsFilename, Settings.Serialize());
        }

        public void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            Processes.RestartApplication();
        }

        public void ShutdownButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        protected override Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            Keys = Enum.GetValues(typeof(Key))
                       .Cast<Key>()
                       .Where(key =>
                       {
                           Regex rx = new(@"mode|dbe|eof|oem|system|ime|abnt|launch|browser|shift|ctrl|alt|scroll|win|apps|noname|pa1|dead|none", RegexOptions.IgnoreCase);
                           return !rx.IsMatch(key.ToString());
                       })
                       .Distinct();

            ModifierKeys = Enum.GetValues(typeof(ModifierKeys))
                               .Cast<ModifierKeys>()
                               .Where(modifierKey =>
                               {
                                   Regex rx = new(@"windows", RegexOptions.IgnoreCase);
                                   return !rx.IsMatch(modifierKey.ToString());
                               });

            return base.OnInitializeAsync(cancellationToken);
        }
    }
}
