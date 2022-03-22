namespace Reginald.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Reginald.Core.IO;
    using Reginald.Services;
    using Reginald.Services.Utilities;

    public class GeneralViewModel : ScrollViewModelBase
    {
        private Key _selectedKey;

        private ModifierKeys _selectedModifierKeyOne;

        private ModifierKeys _selectedModifierKeyTwo;

        public GeneralViewModel(ConfigurationService configurationService)
        {
            ConfigurationService = configurationService;
            SelectedKey = (Key)Enum.Parse(typeof(Key), ConfigurationService.Settings.SearchBoxKey);
            SelectedModifierKeyOne = (ModifierKeys)Enum.Parse(typeof(ModifierKeys), ConfigurationService.Settings.SearchBoxModifierOne);
            SelectedModifierKeyTwo = (ModifierKeys)Enum.Parse(typeof(ModifierKeys), ConfigurationService.Settings.SearchBoxModifierTwo);

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
        }

        public ConfigurationService ConfigurationService { get; set; }

        public IEnumerable<Key> Keys { get; set; }

        public IEnumerable<ModifierKeys> ModifierKeys { get; set; }

        public Key SelectedKey
        {
            get => _selectedKey;
            set
            {
                _selectedKey = value;
                NotifyOfPropertyChange(() => SelectedKey);
            }
        }

        public ModifierKeys SelectedModifierKeyOne
        {
            get => _selectedModifierKeyOne;
            set
            {
                _selectedModifierKeyOne = value;
                NotifyOfPropertyChange(() => SelectedModifierKeyOne);
            }
        }

        public ModifierKeys SelectedModifierKeyTwo
        {
            get => _selectedModifierKeyTwo;
            set
            {
                _selectedModifierKeyTwo = value;
                NotifyOfPropertyChange(() => SelectedModifierKeyTwo);
            }
        }

        public void SelectedKey_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedKey != Key.None)
            {
                ConfigurationService.Settings.SearchBoxKey = SelectedKey.ToString();
                ConfigurationService.Settings.Save();
            }
        }

        public void SelectedModifierKeyOne_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ConfigurationService.Settings.SearchBoxModifierOne = SelectedModifierKeyOne.ToString();
            ConfigurationService.Settings.Save();
        }

        public void SelectedModifierKeyTwo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ConfigurationService.Settings.SearchBoxModifierTwo = SelectedModifierKeyTwo.ToString();
            ConfigurationService.Settings.Save();
        }

        public void LaunchOnStartupToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (!FileOperations.TryCreateShortcut())
            {
                FileOperations.DeleteShortcut();
            }

            ConfigurationService.Settings.Save();
        }

        public void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUtility.RestartApplication();
        }

        public void ShutdownButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
