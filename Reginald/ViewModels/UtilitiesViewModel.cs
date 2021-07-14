using Caliburn.Micro;
using Reginald.Core.Base;
using Reginald.Core.Helpers;
using Reginald.Core.IO;
using Reginald.Extensions;
using Reginald.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;

namespace Reginald.ViewModels
{
    public class UtilitiesViewModel : Screen
    {
        public UtilitiesViewModel()
        {
            LoadKeywordSearchResults(ApplicationPaths.XmlKeywordFilename, KeywordSearchResults);
            LoadKeywordSearchResults(ApplicationPaths.XmlUserKeywordFilename, UserKeywordSearchResults);

            Settings.IncludeInstalledApplications = Properties.Settings.Default.IncludeInstalledApplications;
            Settings.IncludeDefaultKeywords = Properties.Settings.Default.IncludeDefaultKeywords;
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

        private BindableCollection<SearchResultModel> _keywordSearchResults = new();
        public BindableCollection<SearchResultModel> KeywordSearchResults
        {
            get => _keywordSearchResults;
            set
            {
                _keywordSearchResults = value;
                NotifyOfPropertyChange(() => KeywordSearchResults);
            }
        }

        private SearchResultModel _selectedKeywordSearchResult = new();
        public SearchResultModel SelectedKeywordSearchResult
        {
            get => _selectedKeywordSearchResult;
            set
            {
                _selectedKeywordSearchResult = value;
                NotifyOfPropertyChange(() => SelectedKeywordSearchResult);
            }
        }

        private BindableCollection<SearchResultModel> _userKeywordSearchResults = new();
        public BindableCollection<SearchResultModel> UserKeywordSearchResults
        {
            get => _userKeywordSearchResults;
            set
            {
                _userKeywordSearchResults = value;
                NotifyOfPropertyChange(() => UserKeywordSearchResults);
            }
        }

        private SearchResultModel _selectedUserKeywordSearchResult = new();
        public SearchResultModel SelectedUserKeywordSearchResult
        {
            get => _selectedUserKeywordSearchResult;
            set
            {
                _selectedUserKeywordSearchResult = value;
                NotifyOfPropertyChange(() => SelectedUserKeywordSearchResult);
            }
        }

        private void LoadKeywordSearchResults(string name, BindableCollection<SearchResultModel> collection)
        {
            XmlDocument doc = XmlHelper.GetXmlDocument(name);
            IEnumerable<string> attributes = doc.GetNodesAttributes(Constants.NamespacesXpath).Where(x => !x.StartsWith("__"))
                                                                                              .Distinct();

            IEnumerable<SearchResultModel> keywordSearchResults = Array.Empty<SearchResultModel>();
            foreach (string attribute in attributes)
            {
                //keywordSearchResults = keywordSearchResults.Concat(MakeSearchResultModels(doc, attribute, SearchResultModel.Category.Keyword));
                keywordSearchResults = keywordSearchResults.Concat(SearchResultModel.MakeArray(doc, attribute, SearchResultModel.Category.Keyword));
            }
            collection.AddRange(keywordSearchResults);
        }

        public void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        public void IncludeApplicationsToggleButton_Click(object sender, RoutedEventArgs e)
        {
            bool value = !Properties.Settings.Default.IncludeInstalledApplications;
            Properties.Settings.Default.IncludeInstalledApplications = value;
            Properties.Settings.Default.Save();
            Settings.IncludeInstalledApplications = value;
        }

        public void IncludeDefaultKeywordToggleButton_Click(object sender, RoutedEventArgs e)
        {
            bool value = !Properties.Settings.Default.IncludeDefaultKeywords;
            Properties.Settings.Default.IncludeDefaultKeywords = value;
            Properties.Settings.Default.Save();
            Settings.IncludeDefaultKeywords = value;
        }

        public void KeywordSearchResults_IsCheckedChanged(object sender, RoutedEventArgs e)
        {
            ChangeIsEnabledState(ApplicationPaths.XmlKeywordFilename, SelectedKeywordSearchResult.ID);
        }

        public void UserKeywordSearchResults_IsCheckedChanged(object sender, RoutedEventArgs e)
        {
            ChangeIsEnabledState(ApplicationPaths.XmlUserKeywordFilename, SelectedKeywordSearchResult.ID);
        }

        public async void CreateKeyword_ClickAsync(object sender, RoutedEventArgs e)
        {
            IWindowManager manager = new WindowManager();
            await manager.ShowWindowAsync(new NewUserKeywordViewModel(UserKeywordSearchResults));
        }

        public async void EditKeyword_ClickAsync(object sender, RoutedEventArgs e)
        {
            IWindowManager manager = new WindowManager();
            await manager.ShowWindowAsync(new EditUserKeywordViewModel(UserKeywordSearchResults, SelectedUserKeywordSearchResult));

        }

        public void DeleteKeyword_Click(object sender, RoutedEventArgs e)
        {
            string message = $"Are you sure you would like to delete this keyword for '{SelectedUserKeywordSearchResult.Name}'?";
            string caption = "Keyword Deleted";
            MessageBoxResult result = MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                XmlDocument doc = XmlHelper.GetXmlDocument(ApplicationPaths.XmlUserKeywordFilename);
                string xpath = $"//Searches/Namespace[@ID='{SelectedUserKeywordSearchResult.ID}']";
                XmlNode currentNode = doc.SelectSingleNode(xpath);
                string iconPath = currentNode["Icon"].InnerText;
                if (File.Exists(iconPath))
                {
                    File.Delete(iconPath);
                }
                currentNode.ParentNode.RemoveChild(currentNode);
                XmlHelper.SaveXmlDocument(doc, ApplicationPaths.XmlUserKeywordFilename);
                UserKeywordSearchResults.Remove(SelectedUserKeywordSearchResult);
            }
        }

        //private static SearchResultModel[] MakeSearchResultModels(XmlDocument doc, string attribute, SearchResultModel.Category category)
        //{
        //    //XmlNodeList nodes = doc.GetNodes(attribute);
        //    XmlNodeList nodes = doc.GetNodes(String.Format(Constants.NamespaceNameXpathFormat, attribute));
        //    SearchResultModel[] models = new SearchResultModel[nodes.Count];
        //    for (int i = 0; i < nodes.Count; i++)
        //    {
        //        try
        //        {
        //            XmlNode node = nodes[i];
        //            string name = node.Attributes["Name"].Value;
        //            int id = int.Parse(node.Attributes["ID"].Value);

        //            BitmapImage icon = new();
        //            icon.BeginInit();
        //            icon.UriSource = new Uri(node["Icon"].InnerText);
        //            icon.CacheOption = BitmapCacheOption.OnLoad;
        //            icon.DecodePixelWidth = 75;
        //            icon.DecodePixelHeight = 75;
        //            icon.EndInit();
        //            icon.Freeze();

        //            string keyword = node["Keyword"].InnerText;
        //            string separator = node["Separator"].InnerText;
        //            string url = node["URL"].InnerText;
        //            string format = node["Format"].InnerText;
        //            string defaultText = node["DefaultText"].InnerText;
        //            string text = defaultText;
        //            string description = String.Format(format, text);
        //            string alt = node["Alt"].InnerText;
        //            bool isEnabled = Boolean.Parse(node["IsEnabled"].InnerText);

        //            SearchResultModel model = new()
        //            {
        //                Name = name,
        //                CategoryName = category,
        //                Icon = icon,
        //                ID = id,
        //                Keyword = keyword,
        //                Separator = separator,
        //                URL = url,
        //                Text = text,
        //                Format = format,
        //                DefaultText = defaultText,
        //                Description = description,
        //                Alt = alt,
        //                IsEnabled = isEnabled
        //            };
        //            models[i] = model;
        //        }
        //        catch (NullReferenceException)
        //        {
        //            continue;
        //        }
        //    }
        //    return models;
        //}

        private static void ChangeIsEnabledState(string name, int id)
        {
            XmlDocument doc = XmlHelper.GetXmlDocument(name);
            string xpath = $"//Searches/Namespace[@ID='{id}']";
            XmlNode node = doc.SelectSingleNode(xpath);
            XmlNode isEnabledNode = node.SelectSingleNode("IsEnabled");
            bool isEnabled = Boolean.Parse(isEnabledNode.InnerText);
            isEnabledNode.InnerText = (!isEnabled).ToString().ToLower();
            XmlHelper.SaveXmlDocument(doc, name);
        }
    }
}