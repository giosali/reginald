using Caliburn.Micro;
using Reginald.Core.Base;
using Reginald.Core.Enums;
using Reginald.Core.Helpers;
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
    public class KeywordViewModel : Screen
    {
        public KeywordViewModel(string filename, bool overrideMethod = false)
        {
            Filename = filename;
            if (!overrideMethod)
            {
                LoadKeywordSearchResults(filename, KeywordSearchResults);
            }
        }

        protected string Filename { get; set; }

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

        public void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        public void KeywordSearchResults_IsCheckedChanged(object sender, RoutedEventArgs e)
        {
            ChangeIsEnabledState(Filename, SelectedKeywordSearchResult.ID);
        }

        public static void ChangeIsEnabledState(string name, int id)
        {
            XmlDocument doc = XmlHelper.GetXmlDocument(name);
            string xpath = string.Format(Constants.NamespaceIDXpathFormat, id);
            XmlNode node = doc.SelectSingleNode(xpath);
            XmlNode isEnabledNode = node.SelectSingleNode("IsEnabled");
            bool isEnabled = bool.Parse(isEnabledNode.InnerText);
            isEnabledNode.InnerText = (!isEnabled).ToString().ToLower();
            XmlHelper.SaveXmlDocument(doc, name);
        }

        public virtual void LoadKeywordSearchResults(string name, BindableCollection<SearchResultModel> models)
        {
            XmlDocument doc = XmlHelper.GetXmlDocument(name);
            IEnumerable<string> attributes = doc.GetNodesAttributes(Constants.NamespacesXpath)
                                                .Where(x => !x.StartsWith("__"))
                                                .Distinct();

            IEnumerable<SearchResultModel> keywordSearchResults = Array.Empty<SearchResultModel>();
            foreach (string attribute in attributes)
            {
                keywordSearchResults = keywordSearchResults.Concat(SearchResultModel.MakeList(doc, attribute, Category.Keyword, overrideIsEnabled: true));
            }
            models.AddRange(keywordSearchResults);
        }
    }
}
