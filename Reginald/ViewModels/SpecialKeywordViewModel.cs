using Caliburn.Micro;
using Reginald.Core.Base;
using Reginald.Core.Helpers;
using Reginald.Core.IO;
using Reginald.Extensions;
using Reginald.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;

namespace Reginald.ViewModels
{
    public class SpecialKeywordViewModel : KeywordViewModel
    {
        public SpecialKeywordViewModel() : base(ApplicationPaths.XmlSpecialKeywordFilename)
        {
            //LoadSpecialKeywordSearchResults(ApplicationPaths.XmlSpecialKeywordFilename, SpecialKeywordSearchResults);
            Settings.IncludeSpecialKeywords = Properties.Settings.Default.IncludeSpecialKeywords;
        }

        //private SettingsModel _settings = new();
        //public SettingsModel Settings
        //{
        //    get => _settings;
        //    set
        //    {
        //        _settings = value;
        //        NotifyOfPropertyChange(() => Settings);
        //    }
        //}

        //private BindableCollection<SpecialSearchResultModel> _specialKeywordSearchResults = new();
        //public BindableCollection<SpecialSearchResultModel> SpecialKeywordSearchResults
        //{
        //    get => _specialKeywordSearchResults;
        //    set
        //    {
        //        _specialKeywordSearchResults = value;
        //        NotifyOfPropertyChange(() => SpecialKeywordSearchResults);
        //    }
        //}

        //private SpecialSearchResultModel _specialKeywordSearchResult = new();
        //public SpecialSearchResultModel SpecialKeywordSearchResult
        //{
        //    get => _specialKeywordSearchResult;
        //    set
        //    {
        //        _specialKeywordSearchResult = value;
        //        NotifyOfPropertyChange(() => SpecialKeywordSearchResult);
        //    }
        //}

        //private SpecialSearchResultModel _selectedKeywordSpecialSearchResult = new();
        //public SpecialSearchResultModel SelectedSpecialKeywordSearchResult
        //{
        //    get => _selectedKeywordSpecialSearchResult;
        //    set
        //    {
        //        _selectedKeywordSpecialSearchResult = value;
        //        NotifyOfPropertyChange(() => SelectedSpecialKeywordSearchResult);
        //    }
        //}

        //public void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        //{
        //    ScrollViewer scv = (ScrollViewer)sender;
        //    scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
        //    e.Handled = true;
        //}

        public void IncludeSpecialKeywordsToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            bool value = !Properties.Settings.Default.IncludeSpecialKeywords;
            Properties.Settings.Default.IncludeSpecialKeywords = value;
            Properties.Settings.Default.Save();
            Settings.IncludeSpecialKeywords = value;
        }

        //public void SpecialKeywordSearchResults_IsCheckedChanged(object sender, RoutedEventArgs e)
        //{
        //    ChangeIsEnabledState(ApplicationPaths.XmlSpecialKeywordFilename, SelectedSpecialKeywordSearchResult.ID);
        //}

        //private static void ChangeIsEnabledState(string name, int id)
        //{
        //    XmlDocument doc = XmlHelper.GetXmlDocument(name);
        //    string xpath = string.Format(Constants.NamespaceIDXpathFormat, id);
        //    XmlNode node = doc.SelectSingleNode(xpath);
        //    XmlNode isEnabledNode = node.SelectSingleNode("IsEnabled");
        //    bool isEnabled = bool.Parse(isEnabledNode.InnerText);
        //    isEnabledNode.InnerText = (!isEnabled).ToString().ToLower();
        //    XmlHelper.SaveXmlDocument(doc, name);
        //}

        //private static void LoadSpecialKeywordSearchResults(string name, BindableCollection<SpecialSearchResultModel> collection)
        //{
        //    XmlDocument doc = XmlHelper.GetXmlDocument(name);
        //    XmlNodeList nodes = doc.GetNodes(Constants.NamespacesXpath);

        //    foreach (XmlNode node in nodes)
        //    {
        //        collection.Add(new SpecialSearchResultModel(node, string.Empty));
        //    }
        //}
    }
}
