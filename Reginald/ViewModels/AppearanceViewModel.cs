using Caliburn.Micro;
using Reginald.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Reginald.ViewModels
{
    public class AppearanceViewModel : Screen
    {
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

        public AppearanceViewModel()
        {
            Settings = new SettingsModel
            {
                IsDarkModeEnabled = Properties.Settings.Default.IsDarkModeEnabled
            };
        }

        public void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        public void IsDarkModeEnabledToggleButton_Click(object sender, RoutedEventArgs e)
        {
            bool isDarkModeEnabled = Properties.Settings.Default.IsDarkModeEnabled;
            Properties.Settings.Default.IsDarkModeEnabled = !isDarkModeEnabled;
            Properties.Settings.Default.Save();

            Settings.IsDarkModeEnabled = !isDarkModeEnabled;
        }
    }
}
