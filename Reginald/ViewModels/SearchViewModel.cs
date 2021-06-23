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

        CancellationTokenSource tokenSource = null;

        public async void UserInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tokenSource is not null)
                tokenSource.Cancel();

            ListBox listBox = sender as ListBox;
            if (UserInput != String.Empty)
            {
                tokenSource = new CancellationTokenSource();

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
                    applicationModelsTask = Task.Run(() => GetApplications(UserInput, tokenSource.Token));

                    (string Left, string Separator, string Right) partition = UserInput.Partition(" ");
                    keywordModelsTask = Task.Run(() => MakeSearchResultModels(partition.Left, SearchResultModel.Category.Keyword, partition.Right, tokenSource.Token));

                    if (UserInput.IsMathExpression())
                        mathModelsTask = Task.Run(() => MakeSearchResultModels("__math", SearchResultModel.Category.Math, UserInput.Eval(), tokenSource.Token));

                    await Task.WhenAll(new Task[] { applicationModelsTask, keywordModelsTask, mathModelsTask }.Where(t => t is not null));

                    if (tokenSource.IsCancellationRequested)
                    {
                        models = Array.Empty<SearchResultModel>();
                    }
                    else
                    {
                        models = applicationModelsTask.Result.Concat(keywordModelsTask.Result).Concat(mathModelsTask is null ? mathModels : mathModelsTask.Result);
                    }

                    if (!models.Any() && !tokenSource.IsCancellationRequested)
                    {
                        category = SearchResultModel.Category.SearchEngine;
                        models = new SearchResultModel[2]
                        {
                            MakeSearchResultModel("g", category, tokenSource.Token),
                            MakeSearchResultModel("ddg", category, tokenSource.Token)
                        };
                    }
                }

                UpdateSearchResults(models);
                if (SearchResults.Any())
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

        private IEnumerable<SearchResultModel> GetApplications(string input, CancellationToken cancellationToken)
        {
            string appDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string applicationName = "Reginald";
            string applicationImagesDirectory = "ApplicationIcons";
            string path = Path.Combine(appDataDirectory, applicationName, applicationImagesDirectory);

            Guid applicationsFolderGuid = new Guid("{1e87508d-89c2-42f0-8a7e-645a0f50ca58}");
            ShellObject applicationsFolder = (ShellObject)KnownFolderHelper.FromKnownFolderId(applicationsFolderGuid);

            List<SearchResultModel> applications = new();

            try
            {
                foreach (ShellObject application in (IKnownFolder)applicationsFolder)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    string name = application.Name;
                    string id = application.ParsingName;

                    if (id.EndsWith("url") || name.EndsWith(".url"))
                    {
                        continue;
                    }
                    else if (name.StartsWith(input, StringComparison.InvariantCultureIgnoreCase))
                    {
                        applications.Insert(0, MakeSearchResultModel(name, id, GetApplicationIcon(path, name)));
                    }
                    else if (name.Contains(" "))
                    {
                        string[] nameSplit = name.Split(' ');
                        for (int j = 0; j < nameSplit.Length; j++)
                        {
                            if (nameSplit[j].StartsWith(input, StringComparison.InvariantCultureIgnoreCase))
                            {
                                applications.Add(MakeSearchResultModel(name, id, GetApplicationIcon(path, name)));
                                break;
                            }
                        }
                    }
                    else
                    {
                        (string Left, string Separator, string Right) partition = input.Partition(input[0]);
                        string format = @"[{0}]({1}|{2})";
                        string pattern = String.Format(format, partition.Separator.ToUpper(), partition.Right.ToLower(), partition.Right.ToUpper());
                        try
                        {
                            Regex rx = new Regex(pattern);
                            MatchCollection matches = rx.Matches(name);
                            if (matches.Count != 0)
                            {
                                applications.Add(MakeSearchResultModel(name, id, GetApplicationIcon(path, name)));
                            }
                        }
                        catch (RegexParseException)
                        {
                            continue;
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                return Array.Empty<SearchResultModel>();
            }
            return applications;
        }

        private BitmapImage GetApplicationIcon(string path, string name)
        {
            string iconPath = Path.Combine(path, name + ".png");
            if (!File.Exists(iconPath))
            {
                iconPath = "pack://application:,,,/Reginald;component/Images/help-light.png";
            }

            BitmapImage icon = new BitmapImage(new Uri(iconPath));
            icon.DecodePixelWidth = 50;
            icon.DecodePixelHeight = 50;
            icon.Freeze();
            return icon;
        }

        private SearchResultModel MakeSearchResultModel(string name, string id, BitmapImage icon)
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

        private SearchResultModel MakeSearchResultModel(string attribute, SearchResultModel.Category category, CancellationToken cancellationToken = default(CancellationToken))
        {
            XmlDocument doc = GetXmlDocument("Search");
            XmlNode node = doc.GetNode(attribute);
            if (node is null)
                return new SearchResultModel();

            string name = node["Name"].InnerText;
            BitmapImage icon = new BitmapImage(new Uri(node["Icon"].InnerText));
            icon.DecodePixelWidth = 50;
            icon.DecodePixelHeight = 50;
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

            if (cancellationToken.IsCancellationRequested)
                model = new SearchResultModel();
            return model;
        }

        private SearchResultModel[] MakeSearchResultModels(string attribute, SearchResultModel.Category category, string text = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            XmlDocument doc = GetXmlDocument("Search");
            XmlNodeList nodes = doc.GetNodes(attribute);
            if (nodes is null)
                return Array.Empty<SearchResultModel>();

            SearchResultModel[] models = new SearchResultModel[nodes.Count];
            try
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    XmlNode node = nodes[i];
                    string name = node["Name"].InnerText;
                    BitmapImage icon = new BitmapImage(new Uri(node["Icon"].InnerText));
                    icon.DecodePixelWidth = 50;
                    icon.DecodePixelHeight = 50;
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
            }
            catch (OperationCanceledException)
            {
                models = Array.Empty<SearchResultModel>();
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

            foreach (SearchResultModel model in models)
            {
                if (SearchResults.Contains(model))
                {
                    continue;
                }
                else
                {
                    SearchResults.Add(model);
                }
            }
        }

        public void UserInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    HandleSelectedSearchResultBasedOnCategoryName(SelectedSearchResult.CategoryName);
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

        public void SearchResults_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            HandleSelectedSearchResultBasedOnCategoryName(SelectedSearchResult.CategoryName);
        }

        private void HandleSelectedSearchResultBasedOnCategoryName(SearchResultModel.Category category)
        {
            switch (category)
            {
                case SearchResultModel.Category.Application:
                    Process.Start("explorer.exe", @"shell:appsfolder\" + SelectedSearchResult.ID);
                    TryCloseAsync();
                    break;

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