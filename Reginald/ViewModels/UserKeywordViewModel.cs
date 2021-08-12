using Caliburn.Micro;
using ModernWpf.Controls;
using Reginald.Core.Base;
using Reginald.Core.Enums;
using Reginald.Core.Helpers;
using Reginald.Core.IO;
using Reginald.Extensions;
using Reginald.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;

namespace Reginald.ViewModels
{
    public class UserKeywordViewModel : Screen
    {
        public UserKeywordViewModel()
        {
            LoadKeywordSearchResults(ApplicationPaths.XmlUserKeywordFilename, UserKeywordSearchResults);
        }

        public void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
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

        public void UserKeywordSearchResults_IsCheckedChanged(object sender, RoutedEventArgs e)
        {
            ChangeIsEnabledState(ApplicationPaths.XmlUserKeywordFilename, SelectedUserKeywordSearchResult.ID);
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

        public async Task DeleteKeyword_ClickAsync(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new Dialogs.DeleteUserKeywordDialog(SelectedUserKeywordSearchResult.Name);
            ContentDialogResult result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Secondary)
            {
                XmlDocument doc = XmlHelper.GetXmlDocument(ApplicationPaths.XmlUserKeywordFilename);
                XmlNode currentNode = XmlHelper.GetCurrentNodeFromID(doc, SelectedUserKeywordSearchResult.ID);
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

        public async void EditKeyword_ClickAsync(object sender, RoutedEventArgs e)
        {
            IWindowManager manager = new WindowManager();
            await manager.ShowWindowAsync(new EditUserKeywordViewModel(UserKeywordSearchResults, SelectedUserKeywordSearchResult));

        }

        public async void CreateKeyword_ClickAsync(object sender, RoutedEventArgs e)
        {
            IWindowManager manager = new WindowManager();
            await manager.ShowWindowAsync(new CreateUserKeywordViewModel(UserKeywordSearchResults));
        }

        private static void LoadKeywordSearchResults(string name, BindableCollection<SearchResultModel> collection)
        {
            XmlDocument doc = XmlHelper.GetXmlDocument(name);
            IEnumerable<string> attributes = doc.GetNodesAttributes(Constants.NamespacesXpath)
                                                .Where(x => !x.StartsWith("__"))
                                                .Distinct();

            IEnumerable<SearchResultModel> keywordSearchResults = Array.Empty<SearchResultModel>();
            foreach (string attribute in attributes)
            {
                keywordSearchResults = keywordSearchResults.Concat(SearchResultModel.MakeArray(doc, attribute, Category.Keyword));
            }
            collection.AddRange(keywordSearchResults);
        }
    }
}
