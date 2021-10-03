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
using System.Xml;

namespace Reginald.ViewModels
{
    public class CommandsViewModel : KeywordViewModel
    {
        public CommandsViewModel() : base(ApplicationPaths.XmlCommandsFilename, true)
        {
            LoadKeywordSearchResults(ApplicationPaths.XmlCommandsFilename, KeywordSearchResults);
            Settings.IncludeCommands = Properties.Settings.Default.IncludeCommands;
        }

        public void IncludeCommandsToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            bool value = !Properties.Settings.Default.IncludeCommands;
            Properties.Settings.Default.IncludeCommands = value;
            Properties.Settings.Default.Save();
            Settings.IncludeCommands = value;
        }

        public override async void LoadKeywordSearchResults(string name, BindableCollection<SearchResultModel> models)
        {
            XmlDocument doc = XmlHelper.GetXmlDocument(name);
            IEnumerable<string> attributes = doc.GetNodesAttributes(Constants.NamespacesXpath)
                                                .Distinct();

            IEnumerable<SearchResultModel> commands = Array.Empty<SearchResultModel>();
            string description = string.Empty;
            foreach (string attribute in attributes)
            {
                commands = commands.Concat(await SearchResultModel.MakeListForCommandsAsync(doc, description, attribute, Category.Command, true));
            }
            models.AddRange(commands);
        }
    }
}
