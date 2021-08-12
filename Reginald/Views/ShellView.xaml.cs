using Reginald.Core.Base;
using Reginald.Core.Helpers;
using Reginald.Core.IO;
using System;
using System.Windows;
using System.Windows.Input;
using System.Xml;

namespace Reginald.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class ShellView : Window
    {
        public ShellView()
        {
            XmlDocument settingsDoc = XmlHelper.GetXmlDocument(ApplicationPaths.XmlSettingsFilename);
            SearchBoxKeyGesture = MakeSearchBoxKeyGesture(settingsDoc);

            InitializeComponent();
        }

        public static KeyGesture SearchBoxKeyGesture { get; set; }

        private static KeyGesture MakeSearchBoxKeyGesture(XmlDocument doc)
        {
            string searchBoxKey = doc.SelectSingleNode(string.Format(Constants.SettingsNamespaceNameXpathFormat, "SearchBoxKey"))["Value"].InnerText;
            string searchBoxModifierKeyOne = doc.SelectSingleNode(string.Format(Constants.SettingsNamespaceNameXpathFormat, "SearchBoxModifierKeyOne"))["Value"].InnerText;
            string searchBoxModifierKeyTwo = doc.SelectSingleNode(string.Format(Constants.SettingsNamespaceNameXpathFormat, "SearchBoxModifierKeyTwo"))["Value"].InnerText;

            Key sbKey = (Key)Enum.Parse(typeof(Key), searchBoxKey);
            ModifierKeys sbModifierKeyOne = (ModifierKeys)Enum.Parse(typeof(ModifierKeys), searchBoxModifierKeyOne);
            ModifierKeys sbModifierKeyTwo = (ModifierKeys)Enum.Parse(typeof(ModifierKeys), searchBoxModifierKeyTwo);
            KeyGesture gesture = new KeyGesture(sbKey, sbModifierKeyOne | sbModifierKeyTwo);
            return gesture;
        }
    }
}
