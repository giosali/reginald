using Caliburn.Micro;
using Reginald.Extensions;
using Reginald.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xml;

namespace Reginald.ViewModels
{
    public class UtilitiesViewModel : Screen
    {
        public UtilitiesViewModel()
        {
            LoadKeywordSearchResults();
        }

        private BindableCollection<SearchResultModel> _keywordSearchResults = new();
        public BindableCollection<SearchResultModel> KeywordSearchResults
        {
            get
            {
                return _keywordSearchResults;
            }
            set
            {
                _keywordSearchResults = value;
                NotifyOfPropertyChange(() => KeywordSearchResults);
            }
        }

        private SearchResultModel _selectedKeywordSearchResult = new();
        public SearchResultModel SelectedKeywordSearchResult
        {
            get
            {
                return _selectedKeywordSearchResult;
            }
            set
            {
                _selectedKeywordSearchResult = value;
                NotifyOfPropertyChange(() => SelectedKeywordSearchResult);
            }
        }

        private void LoadKeywordSearchResults()
        {
            XmlDocument doc = GetXmlDocument("Search");
            IEnumerable<string> attributes = doc.GetNodesAttributes().Where(x => !x.StartsWith("__"))
                                                                     .Distinct();

            IEnumerable<SearchResultModel> keywordSearchResults = Array.Empty<SearchResultModel>();
            foreach (string attribute in attributes)
            {
                keywordSearchResults = keywordSearchResults.Concat(MakeSearchResultModels(doc, attribute, SearchResultModel.Category.Keyword));
            }
            KeywordSearchResults.AddRange(keywordSearchResults);
        }

        public void KeywordSearchResults_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        public void KeywordSearchResults_Checked(object sender, RoutedEventArgs e)
        {
            ChangeIsEnabledState("Search", SelectedKeywordSearchResult.ID);
        }

        public void KeywordSearchResults_Unchecked(object sender, RoutedEventArgs e)
        {
            ChangeIsEnabledState("Search", SelectedKeywordSearchResult.ID);
        }

        private static SearchResultModel[] MakeSearchResultModels(XmlDocument doc, string attribute, SearchResultModel.Category category)
        {
            XmlNodeList nodes = doc.GetNodes(attribute);
            SearchResultModel[] models = new SearchResultModel[nodes.Count];
            for (int i = 0; i < nodes.Count; i++)
            {
                try
                {
                    XmlNode node = nodes[i];
                    string name = node.Attributes["Name"].Value;
                    int id = int.Parse(node.Attributes["ID"].Value);

                    BitmapImage icon = new();
                    icon.BeginInit();
                    icon.UriSource = new Uri(node["Icon"].InnerText);
                    icon.CacheOption = BitmapCacheOption.OnLoad;
                    icon.DecodePixelWidth = 75;
                    icon.DecodePixelHeight = 75;
                    icon.EndInit();
                    icon.Freeze();

                    string keyword = node["Keyword"].InnerText;
                    string separator = node["Separator"].InnerText;
                    string url = node["URL"].InnerText;
                    string format = node["Format"].InnerText;
                    string defaultText = node["DefaultText"].InnerText;
                    string text = defaultText;
                    string description = String.Format(format, text);
                    string alt = node["Alt"].InnerText;
                    bool isEnabled = Boolean.Parse(node["IsEnabled"].InnerText);

                    SearchResultModel model = new()
                    {
                        Name = name,
                        CategoryName = category,
                        Icon = icon,
                        ID = id,
                        Keyword = keyword,
                        Separator = separator,
                        URL = url,
                        Text = text,
                        Format = format,
                        DefaultText = defaultText,
                        Description = description,
                        Alt = alt,
                        IsEnabled = isEnabled
                    };
                    models[i] = model;
                }
                catch (NullReferenceException)
                {
                    continue;
                }
            }
            return models;
        }

        private static void ChangeIsEnabledState(string name, int id)
        {
            XmlDocument doc = GetXmlDocument(name);
            string xpath = $"//Searches/Namespace[@ID='{id}']";
            XmlNode node = doc.SelectSingleNode(xpath);
            XmlNode isEnabledNode = node.SelectSingleNode("IsEnabled");
            bool isEnabled = Boolean.Parse(isEnabledNode.InnerText);
            isEnabledNode.InnerText = (!isEnabled).ToString().ToLower();
            SaveXmlDocument(doc, name);
        }

        private static XmlDocument GetXmlDocument(string name)
        {
            string appDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string xmlFileLocation = $"Reginald/{name}.xml";
            XmlDocument doc = new();
            doc.Load(Path.Combine(appDataDirectory, xmlFileLocation));
            return doc;
        }

        private static void SaveXmlDocument(XmlDocument doc, string name)
        {
            string appDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string xmlFileLocation = $"Reginald/{name}.xml";
            doc.Save(Path.Combine(appDataDirectory, xmlFileLocation));
        }
    }
}