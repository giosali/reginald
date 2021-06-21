using Caliburn.Micro;
using Microsoft.WindowsAPICodePack.Shell;
using Reginald.Extensions;
using Reginald.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
                        result.Text = value.Eval();
                    else if (result.CategoryName == SearchResultModel.Category.Keyword)
                    {
                        //string[] inputs = value.Partition(' ');
                        //if (inputs.Length > 1)
                        //    result.Text = inputs[1];
                        //else
                        //    result.Text = String.Empty;
                        (string Left, string Separator, string Right) partition = value.Partition(" ");
                        result.Text = partition.Right;
                    }
                    else
                        result.Text = value;

                    result.Description = String.Format(result.Format, result.Text);
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

        public async void UserInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            if (UserInput != String.Empty)
            {
                if (!listBox.IsVisible)
                    listBox.Visibility = Visibility.Visible;

                SearchResultModel.Category category;

                IEnumerable<SearchResultModel> models;
                Task<IEnumerable<SearchResultModel>> applicationModelsTask;
                Task<SearchResultModel[]> keywordModelsTask;
                Task<SearchResultModel[]> mathModelsTask = null;

                SearchResultModel[] mathModels = Array.Empty<SearchResultModel>();

                if (UserInput.HasTopLevelDomain())
                {
                    models = MakeSearchResultModels("__http", SearchResultModel.Category.HTTP);
                }
                else
                {
                    applicationModelsTask = Task.Run(() => GetApplications(UserInput));

                    (string Left, string Separator, string Right) partition = UserInput.Partition(" ");
                    keywordModelsTask = Task.Run(() => MakeSearchResultModels(partition.Left, SearchResultModel.Category.Keyword, partition.Right));

                    if (UserInput.IsMathExpression())
                        mathModelsTask = Task.Run(() => MakeSearchResultModels("__math", SearchResultModel.Category.Math));

                    await Task.WhenAll(new Task[] { applicationModelsTask, keywordModelsTask, mathModelsTask }.Where(t => t is not null));

                    models = applicationModelsTask.Result.Concat(keywordModelsTask.Result).Concat(mathModelsTask is null ? mathModels : mathModelsTask.Result);
                    if (!models.Any())
                    {
                        category = SearchResultModel.Category.SearchEngine;
                        models = new SearchResultModel[2]
                        {
                            MakeSearchResultModel("g", category),
                            MakeSearchResultModel("ddg", category)
                        };
                    }
                }

                UpdateSearchResults(models);
                SelectedSearchResult = SearchResults[0];
                SearchResults.Refresh();
            }
            else
            {
                if (listBox.IsVisible)
                    listBox.Visibility = Visibility.Hidden;

                SearchResults.Clear();
            }
        }

        private IEnumerable<SearchResultModel> GetApplications(string input)
        {
            Guid applicationsFolderGuid = new Guid("{1e87508d-89c2-42f0-8a7e-645a0f50ca58}");
            ShellObject applicationsFolder = (ShellObject)KnownFolderHelper.FromKnownFolderId(applicationsFolderGuid);

            List<SearchResultModel> relevantResults = new();
            List<SearchResultModel> semiRelevantResults = new();
            List<SearchResultModel> irrelevantResults = new();

            foreach (var application in (IKnownFolder)applicationsFolder)
            {
                string name = application.Name;
                string id = application.ParsingName;
                ImageSource icon = application.Thumbnail.MediumBitmapSource;
                icon.Freeze();

                if (id.EndsWith("url") || name.EndsWith(".url"))
                {
                    continue;
                }
                else if (name.StartsWith(input, StringComparison.InvariantCultureIgnoreCase))
                {
                    relevantResults.Add(MakeSearchResultModel(name, id, icon));
                }
                else if (name.Contains(" "))
                {
                    string[] nameSplit = name.Split(' ');
                    for (int j = 0; j < nameSplit.Length; j++)
                    {
                        if (nameSplit[j].StartsWith(input, StringComparison.InvariantCultureIgnoreCase))
                        {
                            semiRelevantResults.Add(MakeSearchResultModel(name, id, icon));
                            break;
                        }
                    }
                }
                else
                {
                    (string Left, string Separator, string Right) partition = input.Partition(input[0]);
                    string format = @"[{0}]({1}|{2})";
                    string pattern = String.Format(format, partition.Separator.ToUpper(), partition.Right.ToLower(), partition.Right.ToUpper());
                    Regex rx = new Regex(pattern);
                    MatchCollection matches = rx.Matches(name);
                    if (matches.Count != 0)
                        irrelevantResults.Add(MakeSearchResultModel(name, id, icon));
                }
            }
            return relevantResults.Concat(semiRelevantResults).Concat(irrelevantResults);
        }

        private SearchResultModel MakeSearchResultModel(string name, string id, ImageSource icon)
        {
            SearchResultModel model = new()
            {
                Name = name,
                CategoryName = SearchResultModel.Category.Application,
                ID = id,
                Icon = icon,
                Text = name,
                Format = "{0}",
                Description = name,
                Alt = "Application"
            };
            return model;
        }

        private SearchResultModel MakeSearchResultModel(string attribute, SearchResultModel.Category category)
        {
            XmlDocument doc = GetXmlDocument("Search");
            XmlNode node = doc.GetNode(attribute);
            if (node is null)
                return new SearchResultModel();

            string name = node["Name"].InnerText;
            BitmapImage icon = new BitmapImage(new Uri(node["Icon"].InnerText));
            icon.Freeze();
            string keyword = node["Keyword"].InnerText;
            string separator = node["Separator"].InnerText;
            string url = node["URL"].InnerText;
            string text = UserInput;
            string format = node["Format"].InnerText;
            string description = String.Format(format, text);
            string alt = node["Alt"].InnerText;

            SearchResultModel model = new()
            {
                Name = name,
                CategoryName = category,
                Icon = icon,
                Keyword = keyword,
                Separator = separator,
                URL = url,
                Text = text,
                Format = format,
                Description = description,
                Alt = alt
            };
            return model;
        }

        private SearchResultModel[] MakeSearchResultModels(string attribute, SearchResultModel.Category category, string text = null)
        {
            XmlDocument doc = GetXmlDocument("Search");
            XmlNodeList nodes = doc.GetNodes(attribute);
            if (nodes is null)
                return new SearchResultModel[] { };

            SearchResultModel[] models = new SearchResultModel[nodes.Count];
            for (int i = 0; i < nodes.Count; i++)
            {
                XmlNode node = nodes[i];
                string name = node["Name"].InnerText;
                BitmapImage icon = new BitmapImage(new Uri(node["Icon"].InnerText));
                icon.Freeze();
                string keyword = node["Keyword"].InnerText;
                string separator = node["Separator"].InnerText;
                string url = node["URL"].InnerText;
                string format = node["Format"].InnerText;
                text = text is not null ? text : UserInput;
                string description = String.Format(format, text);
                string alt = node["Alt"].InnerText;

                SearchResultModel model = new()
                {
                    Name = name,
                    CategoryName = category,
                    Icon = icon,
                    Keyword = keyword,
                    Separator = separator,
                    URL = url,
                    Text = text,
                    Format = format,
                    Description = description,
                    Alt = alt
                };
                models[i] = model;
            }
            return models;
        }

        private void UpdateSearchResults(IEnumerable<SearchResultModel> models)
        {
            for (int i = SearchResults.Count - 1; i >= 0; i--)
            {
                if (!models.Contains(SearchResults[i]))
                {
                    SearchResults.RemoveAt(i);
                }
            }

            int j = 0;
            foreach (SearchResultModel model in models)
            {
                if (SearchResults.Contains(model))
                {
                    continue;
                }
                else
                {
                    SearchResults.Insert(j, model);
                }
                j++;
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
                            Clipboard.SetText(SelectedSearchResult.Text);
                            TryCloseAsync();
                            break;

                        case SearchResultModel.Category.Keyword:
                            {
                                string uri = String.Format(SelectedSearchResult.URL, SelectedSearchResult.Text);
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

        public void SearchResults_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            (sender as ListBox).ScrollIntoView(SelectedSearchResult);
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
            doc.Load(Path.Combine(appDataDirectory, xmlLocation));
            return doc;
        }
    }
}