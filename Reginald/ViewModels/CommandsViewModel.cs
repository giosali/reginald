using Caliburn.Micro;
using Reginald.Core.Base;
using Reginald.Core.Enums;
using Reginald.Core.Helpers;
using Reginald.Core.IO;
using Reginald.Extensions;
using Reginald.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;

namespace Reginald.ViewModels
{
    public class CommandsViewModel : Screen
    {
        public CommandsViewModel()
        {
            LoadKeywordSearchResultsAsync(ApplicationPaths.XmlCommandsFilename, Commands);
            Settings.IncludeCommands = Properties.Settings.Default.IncludeDefaultKeywords;
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

        private BindableCollection<SearchResultModel> _commands = new();
        public BindableCollection<SearchResultModel> Commands
        {
            get => _commands;
            set
            {
                _commands = value;
                NotifyOfPropertyChange(() => Commands);
            }
        }

        private SearchResultModel _selectedCommand = new();
        public SearchResultModel SelectedCommand
        {
            get => _selectedCommand;
            set
            {
                _selectedCommand = value;
                NotifyOfPropertyChange(() => SelectedCommand);
            }
        }

        public void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        public void IncludeCommandsToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            bool value = !Properties.Settings.Default.IncludeCommands;
            Properties.Settings.Default.IncludeCommands = value;
            Properties.Settings.Default.Save();
            Settings.IncludeCommands = value;
        }

        public void Commands_IsCheckedChanged(object sender, RoutedEventArgs e)
        {
            ChangeIsEnabledState(ApplicationPaths.XmlCommandsFilename, SelectedCommand.ID);
        }

        private static void ChangeIsEnabledState(string name, int id)
        {
            XmlDocument doc = XmlHelper.GetXmlDocument(name);
            string xpath = string.Format(Constants.NamespaceIDXpathFormat, id);
            XmlNode node = doc.SelectSingleNode(xpath);
            XmlNode isEnabledNode = node.SelectSingleNode("IsEnabled");
            bool isEnabled = bool.Parse(isEnabledNode.InnerText);
            isEnabledNode.InnerText = (!isEnabled).ToString().ToLower();
            XmlHelper.SaveXmlDocument(doc, name);
        }

        private static async void LoadKeywordSearchResultsAsync(string name, BindableCollection<SearchResultModel> collection)
        {
            XmlDocument doc = XmlHelper.GetXmlDocument(name);
            IEnumerable<string> attributes = doc.GetNodesAttributes(Constants.NamespacesXpath);

            IEnumerable<SearchResultModel> commands = Array.Empty<SearchResultModel>();
            string description = string.Empty;
            foreach (string attribute in attributes)
            {
                commands = commands.Concat(await SearchResultModel.MakeListForCommandsAsync(doc, description, attribute, Category.Notifier));
            }
            collection.AddRange(commands);
        }
    }
}
