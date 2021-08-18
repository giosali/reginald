using Caliburn.Micro;
using ModernWpf.Controls;
using Reginald.Core.Base;
using Reginald.Core.Helpers;
using Reginald.Core.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;

namespace Reginald.ViewModels
{
    public class GeneralViewModel : Screen
    {
        public GeneralViewModel()
        {
            ModKeys = Enum.GetValues(typeof(ModifierKeys))
                          .Cast<ModifierKeys>()
                          .Where(x =>
                          {
                              Regex rx = new(@"windows", RegexOptions.IgnoreCase);
                              return !rx.IsMatch(x.ToString());
                          });

            Keys = Enum.GetValues(typeof(Key))
                       .Cast<Key>()
                       .Where(x =>
                       {
                           Regex rx = new(@"mode|dbe|eof|oem|system|ime|abnt|launch|browser|shift|ctrl|alt|scroll|win|apps|noname|pa1|dead|none", RegexOptions.IgnoreCase);
                           return !rx.IsMatch(x.ToString());
                       })
                       .Distinct();

            XmlDocument settingsDoc = XmlHelper.GetXmlDocument(ApplicationPaths.XmlSettingsFilename);
            string searchBoxKey = settingsDoc.SelectSingleNode(string.Format(Constants.SettingsNamespaceNameXpathFormat, "SearchBoxKey"))["Value"].InnerText;
            string searchBoxModifierKeyOne = settingsDoc.SelectSingleNode(string.Format(Constants.SettingsNamespaceNameXpathFormat, "SearchBoxModifierKeyOne"))["Value"].InnerText;
            string searchBoxModifierKeyTwo = settingsDoc.SelectSingleNode(string.Format(Constants.SettingsNamespaceNameXpathFormat, "SearchBoxModifierKeyTwo"))["Value"].InnerText;

            Properties.Settings.Default.SearchBoxKey = searchBoxKey;
            Properties.Settings.Default.SearchBoxModifierOne = searchBoxModifierKeyOne;
            Properties.Settings.Default.SearchBoxModifierTwo = searchBoxModifierKeyTwo;
            SelectedKey = (Key)Enum.Parse(typeof(Key), searchBoxKey);
            SelectedModKeyOne = (ModifierKeys)Enum.Parse(typeof(ModifierKeys), searchBoxModifierKeyOne);
            SelectedModKeyTwo = (ModifierKeys)Enum.Parse(typeof(ModifierKeys), searchBoxModifierKeyTwo);
        }

        public static IEnumerable<ModifierKeys> ModKeys { get; set; }

        private ModifierKeys _selectedModKeyTwo;
        public ModifierKeys SelectedModKeyTwo
        {
            get => _selectedModKeyTwo;
            set
            {
                _selectedModKeyTwo = value;
                NotifyOfPropertyChange(() => SelectedModKeyTwo);
                UpdatePropertySettings(SelectedModKeyTwo, 2);
                UpdateSetting("SearchBoxModifierKeyTwo", SelectedModKeyTwo.ToString());
            }
        }

        private ModifierKeys _selectedModKeyOne;
        public ModifierKeys SelectedModKeyOne
        {
            get => _selectedModKeyOne;
            set
            {
                _selectedModKeyOne = value;
                NotifyOfPropertyChange(() => SelectedModKeyOne);
                UpdatePropertySettings(SelectedModKeyOne, 1);
                UpdateSetting("SearchBoxModifierKeyOne", SelectedModKeyOne.ToString());
            }
        }

        public static IEnumerable<Key> Keys { get; set; }

        private Key _selectedKey;
        public Key SelectedKey
        {
            get => _selectedKey;
            set
            {
                if (value == Key.None)
                {
                    string message = "This key cannot be left blank";
                    _ = DisplayWarningAsync(message);
                    SelectedKey = _selectedKey;
                }
                else
                {
                    _selectedKey = value;
                    UpdatePropertySettings(SelectedKey);
                    UpdateSetting("SearchBoxKey", SelectedKey.ToString());
                }
                NotifyOfPropertyChange(() => SelectedKey);
            }
        }

        public void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo startInfo = new();
            string filename = Process.GetCurrentProcess().MainModule.FileName;
            startInfo.Arguments = $"/C ping 127.0.0.1 -n 2 && \"{filename}\"";
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.CreateNoWindow = true;
            startInfo.FileName = "cmd.exe";
            Process.Start(startInfo);
            Application.Current.Shutdown();
        }

        public void ShutdownButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private static async Task DisplayWarningAsync(string message)
        {
            ContentDialog dialog = new Dialogs.WarningDialog(message);
            _ = await dialog.ShowAsync();
        }

        private static void UpdatePropertySettings(Key key)
        {
            Properties.Settings.Default.SearchBoxKey = key.ToString();
            Properties.Settings.Default.Save();
        }

        private static void UpdatePropertySettings(ModifierKeys modifier, int number)
        {
            switch (number)
            {
                case 1:
                    Properties.Settings.Default.SearchBoxKey = modifier.ToString();
                    break;

                case 2:
                    Properties.Settings.Default.SearchBoxModifierTwo = modifier.ToString();
                    break;

                default:
                    break;
            }
            Properties.Settings.Default.Save();
        }

        private static void UpdateSetting(string setting, string value)
        {
            XmlDocument settingsDoc = XmlHelper.GetXmlDocument(ApplicationPaths.XmlSettingsFilename);
            string xpath = string.Format(Constants.SettingsNamespaceNameXpathFormat, setting);
            XmlNode node = settingsDoc.SelectSingleNode(xpath);
            node["Value"].InnerText = value;
            XmlHelper.SaveXmlDocument(settingsDoc, ApplicationPaths.XmlSettingsFilename);
        }
    }
}
