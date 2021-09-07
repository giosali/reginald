using Caliburn.Micro;
using Reginald.Core.Helpers;
using Reginald.Core.IO;
using Reginald.Extensions;
using Reginald.Models;
using System.Windows;
using System.Xml;

namespace Reginald.ViewModels
{
    public class CreateUserKeywordViewModel : CustomKeywordViewModel
    {
        public CreateUserKeywordViewModel(BindableCollection<SearchResultModel> models, SearchResultModel model = null) : base(ApplicationPaths.XmlUserKeywordFilename, true)
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
            string keyword = SelectedKeywordSearchResult.Keyword;
            string name = SelectedKeywordSearchResult.Alt.Capitalize();
            string url = SelectedKeywordSearchResult.URL;
            string separator = SelectedKeywordSearchResult.Separator;
            string format = SelectedKeywordSearchResult.Format;
            string defaultText = SelectedKeywordSearchResult.DefaultText;
            string alt = SelectedKeywordSearchResult.Alt;

            XmlDocument doc = XmlHelper.GetXmlDocument(Filename);
            XmlNode lastNode = XmlHelper.GetLastNode(doc);
            int id = lastNode is null ? 0 : int.Parse(lastNode.Attributes["ID"].Value) + 1;
            XmlNode parentNode = lastNode is null ? doc.SelectSingleNode(@"//Searches") : lastNode.ParentNode;

            XmlNode node = XmlHelper.MakeXmlNode(keyword, id, name, IconPath, url, separator, format, defaultText, alt);
            XmlNode importedNode = parentNode.OwnerDocument.ImportNode(node, true);
            parentNode.AppendChild(importedNode);
            XmlHelper.SaveXmlDocument(doc, Filename);

            SelectedKeywordSearchResult.Description = string.Format(format, defaultText);
            SelectedKeywordSearchResult.IsEnabled = true;
            SelectedKeywordSearchResult.ID = id;
            KeywordSearchResults.Add(SelectedKeywordSearchResult);
            NotifyOfPropertyChange(() => SelectedKeywordSearchResult);
            KeywordSearchResults.Clear();
            LoadKeywordSearchResults(Filename, KeywordSearchResults);
            TryCloseAsync();
        }
    }
}
