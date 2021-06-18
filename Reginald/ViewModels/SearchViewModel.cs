using Caliburn.Micro;
using Reginald.Extensions;
using Reginald.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;

namespace Reginald.ViewModels
{
    public class SearchViewModel : Screen
    {
        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            UserInput = String.Empty;
            SearchResults.Clear();

            return base.OnDeactivateAsync(close, cancellationToken);
        }

        public SearchViewModel()
        {

        }

        private string _userInput;
        public string UserInput
        {
            get
            {
                return _userInput;
            }
            set
            {
                _userInput = value;
                NotifyOfPropertyChange(() => UserInput);
                foreach (SearchResultModel result in SearchResults)
                {
                    if (result.CategoryName == SearchResultModel.Category.Math)
                        value = value.Eval();
                    else if (result.CategoryName == SearchResultModel.Category.Keyword)
                    {
                        string[] inputs = value.Partition(' ');
                        if (inputs.Length > 1)
                            value = inputs[1];
                        else
                            value = String.Empty;
                    }

                    result.Description = String.Format(result.Format, value);
                }
            }
        }

        private BindableCollection<SearchResultModel> _searchResults = new();
        public BindableCollection<SearchResultModel> SearchResults
        {
            get
            {
                return _searchResults;
            }
            set
            {
                _searchResults = value;
            }
        }

        private SearchResultModel _selectedSearchResult = new();
        public SearchResultModel SelectedSearchResult
        {
            get
            {
                return _selectedSearchResult;
            }
            set
            {
                _selectedSearchResult = value;
                NotifyOfPropertyChange(() => SelectedSearchResult);
            }
        }

        public void UserInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            if (UserInput != String.Empty)
            {
                if (!listBox.IsVisible)
                    listBox.Visibility = Visibility.Visible;

                SearchResultModel.Category category;
                SearchResultModel[] searchResultModels;

                if (UserInput.IsMathExpression())
                {
                    category = SearchResultModel.Category.Math;
                    searchResultModels = new SearchResultModel[1]
                    {
                        MakeSearchResultModel("__math", category)
                    };
                }
                else if (UserInput.HasTopLevelDomain())
                {
                    category = SearchResultModel.Category.HTTP;
                    searchResultModels = new SearchResultModel[1]
                    {
                        MakeSearchResultModel("__http", category)
                    };
                }
                else
                {
                    category = SearchResultModel.Category.Keyword;
                    string[] inputs = UserInput.Partition(' ');
                    string attribute = inputs[0];
                    SearchResultModel searchResultModel = MakeSearchResultModel(attribute, category);
                    if (inputs.Length > 1 && !(searchResultModel.Name is null))
                    {
                        searchResultModels = new SearchResultModel[1]
                        {
                            MakeSearchResultModel(attribute, category)
                        };
                    }
                    else
                    {
                        category = SearchResultModel.Category.SearchEngine;
                        searchResultModels = new SearchResultModel[2]
                        {
                            MakeSearchResultModel("g", category),
                            MakeSearchResultModel("ddg", category)
                        };
                    }
                }

                UpdateSearchResults(category);
                if (SearchResults.Count == 0)
                {
                    for (int i = 0; i < searchResultModels.Length; i++)
                    {
                        SearchResultModel model = searchResultModels[i];
                        model.Count = "CTRL + " + (i + 1).ToString();
                        SearchResults.Add(searchResultModels[i]);
                    }
                    SelectedSearchResult = SearchResults[0];
                }
                SearchResults.Refresh();
            }
            else
            {
                if (listBox.IsVisible)
                    listBox.Visibility = Visibility.Hidden;

                SearchResults.Clear();
            }
        }

        private SearchResultModel MakeSearchResultModel(string attribute, SearchResultModel.Category category)
        {
            XmlDocument doc = GetXmlDocument("Search");
            XmlNode node = doc.GetNode(attribute);
            if (node is null)
                return new SearchResultModel();

            string name = node["Name"].InnerText;
            string icon = node["Icon"].InnerText;
            string keyword = node["Keyword"].InnerText;
            string separator = node["Separator"].InnerText;
            string url = node["URL"].InnerText;
            string format = node["Format"].InnerText;
            string description;
            if (category is SearchResultModel.Category.Keyword)
            {
                string[] inputs = UserInput.Partition(' ');
                if (inputs.Length > 1)
                    description = String.Format(format, inputs[1]);
                else
                    description = String.Empty;
            }
            else
                description = String.Format(format, UserInput);
            string alt = node["Alt"].InnerText;

            SearchResultModel searchResultModel = new()
            {
                Name = name,
                CategoryName = category,
                Icon = icon,
                Keyword = keyword,
                Separator = separator,
                URL = url,
                Format = format,
                Description = description,
                Alt = alt
            };
            return searchResultModel;
        }

        private void UpdateSearchResults(SearchResultModel.Category category)
        {
            if (SearchResults.Count != 0)
            {
                for (int i = SearchResults.Count - 1; i >= 0; i--)
                {
                    if (SearchResults[i].CategoryName != category)
                        SearchResults.RemoveAt(i);
                }
            }
        }

        public void UserInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    switch (SelectedSearchResult.CategoryName)
                    {
                        case SearchResultModel.Category.Math:
                            Clipboard.SetText(SelectedSearchResult.Description);
                            TryCloseAsync();
                            break;

                        case SearchResultModel.Category.Keyword:
                            {
                                string uri = String.Format(SelectedSearchResult.URL, SelectedSearchResult.Description);
                                GoToWebsite(uri);
                                break;
                            }

                        case SearchResultModel.Category.HTTP:
                            {
                                string uri = UserInput;
                                if (!Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                                    uri = uri.MakeValidScheme();
                                GoToWebsite(uri);
                                break;
                            }

                        case SearchResultModel.Category.SearchEngine:
                            {
                                string uri = String.Format(SelectedSearchResult.URL, UserInput.Quote(SelectedSearchResult.Separator));
                                GoToWebsite(uri);
                                break;
                            }

                        default:
                            break;
                    }
                    e.Handled = true;
                    break;

                case Key.Up:
                    try
                    {
                        SelectedSearchResult = SearchResults[SearchResults.IndexOf(SelectedSearchResult) - 1];
                    }
                    catch (ArgumentOutOfRangeException) { }
                    break;

                case Key.Down:
                    try
                    {
                        SelectedSearchResult = SearchResults[SearchResults.IndexOf(SelectedSearchResult) + 1];
                    }
                    catch (ArgumentOutOfRangeException) { }
                    break;

                default:
                    break;
            }
        }

        public void SearchResults_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                SelectedSearchResult = SearchResults[0];
            }
            catch (ArgumentOutOfRangeException) { }
        }

        private static string MakeValidScheme(string uri)
        {
            Regex rx = new Regex("https*://", RegexOptions.IgnoreCase);
            MatchCollection matches = rx.Matches(uri);
            if (matches.Count == 0)
                uri = "https://" + uri;
            return uri;
        }

        private static void GoToWebsite(string uri)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = uri,
                    UseShellExecute = true
                };
                Process.Start(startInfo);
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                if (ex.ErrorCode == -2147467259)
                    MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private static XmlDocument GetXmlDocument(string name)
        {
            string appDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string xmlLocation = $"Reginald/{name}.xml";
            XmlDocument doc = new();
            doc.Load(System.IO.Path.Combine(appDataDirectory, xmlLocation));
            return doc;
        }
    }
}