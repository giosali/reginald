using Caliburn.Micro;
using Reginald.Core.Base;
using Reginald.Core.Helpers;
using Reginald.Core.IO;
using Reginald.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;

namespace Reginald.ViewModels
{
    public class SearchBoxAppearanceViewModel : Screen
    {
        public SearchBoxAppearanceViewModel()
        {
            XmlDocument settingsDoc = XmlHelper.GetXmlDocument(ApplicationPaths.XmlSettingsFilename);
            bool isDefaultColorEnabled = bool.Parse(settingsDoc.SelectSingleNode(string.Format(Constants.SettingsNamespaceNameXpathFormat, "IsDefaultColorEnabled"))["Value"].InnerText);
            bool isSystemColorEnabled = bool.Parse(settingsDoc.SelectSingleNode(string.Format(Constants.SettingsNamespaceNameXpathFormat, "IsSystemColorEnabled"))["Value"].InnerText);

            Settings.IsDefaultColorEnabled = isDefaultColorEnabled;
            Settings.IsSystemColorEnabled = isSystemColorEnabled;
            Properties.Settings.Default.IsDefaultColorEnabled = Settings.IsDefaultColorEnabled;
            Properties.Settings.Default.IsSystemColorEnabled = Settings.IsSystemColorEnabled;
            Properties.Settings.Default.Save();
        }

        private SettingsModel _settings = new();
        public SettingsModel Settings
        {
            get => _settings;
            set
            {
                _settings = value;
                NotifyOfPropertyChange(() => Settings);
            }
        }

        //public int Count { get; set; } = 1;

        public void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        public void ColorRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            UpdateBoolSetting("IsDefaultColorEnabled", Settings.IsDefaultColorEnabled);
            UpdateBoolSetting("IsSystemColorEnabled", Settings.IsSystemColorEnabled);
            UpdatePropertySetting("IsDefaultColorEnabled", Settings.IsDefaultColorEnabled);
            UpdatePropertySetting("IsSystemColorEnabled", Settings.IsSystemColorEnabled);
        }

        private static void UpdateBoolSetting(string setting, bool value)
        {
            XmlDocument settingsDoc = XmlHelper.GetXmlDocument(ApplicationPaths.XmlSettingsFilename);
            string xpath = string.Format(Constants.SettingsNamespaceNameXpathFormat, setting);
            XmlNode node = settingsDoc.SelectSingleNode(xpath);
            node["Value"].InnerText = (value).ToString().ToLower();
            XmlHelper.SaveXmlDocument(settingsDoc, ApplicationPaths.XmlSettingsFilename);
        }

        private static void UpdatePropertySetting(string setting, bool value)
        {
            switch (setting)
            {
                case "IsDefaultColorEnabled":
                    Properties.Settings.Default.IsDefaultColorEnabled = value;
                    break;

                case "IsSystemColorEnabled":
                    Properties.Settings.Default.IsSystemColorEnabled = value;
                    break;

                default:
                    break;
            }
            Properties.Settings.Default.Save();
        }
    }
}
