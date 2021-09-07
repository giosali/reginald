using Caliburn.Micro;
using Reginald.Core.Helpers;
using Reginald.Core.IO;
using Reginald.Extensions;
using Reginald.Models;
using System.IO;
using System.Windows;
using System.Xml;

namespace Reginald.ViewModels
{
    public class EditUserKeywordViewModel : CustomKeywordViewModel
    {
        public EditUserKeywordViewModel(BindableCollection<SearchResultModel> models, SearchResultModel model = null) : base(ApplicationPaths.XmlUserKeywordFilename, true)
        {
            KeywordSearchResults = models;
            SelectedKeywordSearchResult = model ?? new SearchResultModel
            {
                Separator = "+",
                DefaultText = "..."
            };
        }

        public override void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string format = SelectedKeywordSearchResult.Format;
            string defaultText = SelectedKeywordSearchResult.DefaultText;

            XmlDocument doc = XmlHelper.GetXmlDocument(Filename);
            XmlNode currentNode = XmlHelper.GetCurrentNodeFromID(doc, SelectedKeywordSearchResult.ID);
            UpdateCurrentNode(currentNode);
            XmlHelper.SaveXmlDocument(doc, Filename);

            SelectedKeywordSearchResult.Description = string.Format(format, defaultText);
            NotifyOfPropertyChange(() => SelectedKeywordSearchResult);
            KeywordSearchResults.Clear();
            LoadKeywordSearchResults(Filename, KeywordSearchResults);
            _ = TryCloseAsync();
        }

        private void UpdateCurrentNode(XmlNode currentNode)
        {
            currentNode["Name"].InnerText = SelectedKeywordSearchResult.Alt.Capitalize();
            currentNode["Keyword"].InnerText = SelectedKeywordSearchResult.Keyword;
            if (IconPath is not null)
            {
                string iconPath = currentNode["Icon"].InnerText;
                File.Delete(iconPath);
                currentNode["Icon"].InnerText = IconPath;
            }
            currentNode["URL"].InnerText = SelectedKeywordSearchResult.URL;
            currentNode["Separator"].InnerText = SelectedKeywordSearchResult.Separator;
            currentNode["Format"].InnerText = SelectedKeywordSearchResult.Format;
            currentNode["DefaultText"].InnerText = SelectedKeywordSearchResult.DefaultText;
            currentNode["Alt"].InnerText = SelectedKeywordSearchResult.Alt;
        }
    }
}
