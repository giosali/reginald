using Caliburn.Micro;
using Reginald.Models;
using System.Windows;

namespace Reginald.ViewModels
{
    public class ThemesViewModel : Screen
    {
        public ThemesViewModel()
        {
            Settings = new SettingsModel
            {
                IsDarkModeEnabled = Properties.Settings.Default.IsDarkModeEnabled,
                IsSearchBoxBorderEnabled = Properties.Settings.Default.IsSearchBoxBorderEnabled
            };
        }

        private SettingsModel _settings;
        public SettingsModel Settings
        {
            get => _settings;
            set
            {
                _settings = value;
                NotifyOfPropertyChange(() => Settings);
            }
        }

        public void IsDarkModeEnabledToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            bool isDarkModeEnabled = Properties.Settings.Default.IsDarkModeEnabled;
            Properties.Settings.Default.IsDarkModeEnabled = !isDarkModeEnabled;
            Properties.Settings.Default.Save();

            Settings.IsDarkModeEnabled = !isDarkModeEnabled;
        }

        public void IsSearchBoxBorderEnabledToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            bool isSearchBoxBorderEnabled = Properties.Settings.Default.IsSearchBoxBorderEnabled;
            Properties.Settings.Default.IsSearchBoxBorderEnabled = !isSearchBoxBorderEnabled;
            Properties.Settings.Default.Save();

            Settings.IsSearchBoxBorderEnabled = !isSearchBoxBorderEnabled;
        }
    }
}
