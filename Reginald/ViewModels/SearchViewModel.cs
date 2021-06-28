using Caliburn.Micro;
using Microsoft.WindowsAPICodePack.Shell;
using Reginald.Extensions;
using Reginald.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        private readonly string applicationImagesDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Reginald", "ApplicationIcons");
        private readonly string applicationsTxtFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Reginald", "Applications.txt");
        private readonly ShellObject applicationsFolder = (ShellObject)KnownFolderHelper.FromKnownFolderId(new Guid("{1e87508d-89c2-42f0-8a7e-645a0f50ca58}"));
        private readonly XmlDocument searchDoc = GetXmlDocument("Search");
        private Dictionary<string, string> applicationsDict = MakeApplicationsDictionary();

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
                foreach (SearchResultModel model in SearchResults)
                {
                    if (model.CategoryName == SearchResultModel.Category.Application)
                    {

                    }
                    else if (model.CategoryName == SearchResultModel.Category.Math)
                    {
                        model.Text = value.Eval();
                    }
                    else if (model.CategoryName == SearchResultModel.Category.Keyword)
                    {
                        (string left, _, string right) = value.Partition(" ");
                        model.Text = right == String.Empty ? model.DefaultText : right;
                    }
                    else
                        model.Text = value;

                    model.Description = String.Format(model.Format, model.Text);
                }
                SearchResults.Refresh();
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
                IsVisible = SearchResults.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
                SelectedSearchResult = SearchResults[0];
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

        private Visibility _isVisible;
        public Visibility IsVisible
        {
            get
            {
                return _isVisible;
            }
            set
            {
                _isVisible = value;
                NotifyOfPropertyChange(() => IsVisible);
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

                //if (!listBox.IsVisible)
                //    listBox.Visibility = Visibility.Visible;

                IEnumerable<SearchResultModel> models;
                Task<IEnumerable<SearchResultModel>> applicationModelsTask;
                Task<IEnumerable<SearchResultModel>> keywordModelsTask;
                Task<SearchResultModel[]> mathModelsTask = null;

                if (UserInput.HasTopLevelDomain())
                {
                    models = MakeSearchResultModels("__http", SearchResultModel.Category.HTTP);
                }
                else
                {
                    applicationModelsTask = Task.Run(() => GetApplications(UserInput, tokenSource.Token));

                    keywordModelsTask = Task.Run(() =>
                    {
                        (string Left, string Separator, string Right) partition = UserInput.Partition(" ");

                        List<string> attributes = searchDoc.GetNodesAttributes();
                        string format = @"((?<!\w){0}.*)";
                        Regex rx = new(String.Format(format, partition.Left), RegexOptions.IgnoreCase);
                        IEnumerable<string> matches = attributes.Where(x => rx.IsMatch(x))
                                                                .Distinct();

                        IEnumerable<SearchResultModel> keywordModels = Array.Empty<SearchResultModel>();
                        foreach (string match in matches)
                        {
                            keywordModels = keywordModels.Concat(MakeSearchResultModels(searchDoc, match, SearchResultModel.Category.Keyword, partition.Right));
                        }
                        return keywordModels;
                    });

                    mathModelsTask = Task.Run(() =>
                    {
                        if (UserInput.IsMathExpression())
                            return MakeSearchResultModels("__math", SearchResultModel.Category.Math, UserInput.Eval());
                        return Array.Empty<SearchResultModel>();
                    });

                    //stopwatch.Stop();
                    //Debug.WriteLine(stopwatch.ElapsedMilliseconds);

                    var applicationModels = await applicationModelsTask;
                    if (applicationModels is null)
                        return;
                    var keywordModels = await keywordModelsTask;
                    var mathModels = await mathModelsTask;

                    models = applicationModels.Concat(keywordModels).Concat(mathModels);

                    if (!models.Any())
                    {
                        models = new SearchResultModel[2]
                        {
                            MakeSearchResultModel("g", SearchResultModel.Category.SearchEngine),
                            MakeSearchResultModel("ddg", SearchResultModel.Category.SearchEngine)
                        };
                    }
                }

                UpdateSearchResults(models);
                SearchResults.Refresh();
                
                //if (SearchResults.Count != 0)
                //{
                //    Stopwatch stopwatch = new();
                //    stopwatch.Start();
                //    SelectedSearchResult = SearchResults[0];
                //    stopwatch.Stop();
                //    Debug.WriteLine($"Time: {stopwatch.ElapsedMilliseconds}");
                //}
            }
            else
            {
                //if (listBox.IsVisible)
                //    listBox.Visibility = Visibility.Hidden;

                SearchResults.Clear();
            }
        }

        private IEnumerable<SearchResultModel> GetApplications(string input, CancellationToken cancellationToken)
        {
            //Stopwatch stopwatch = new();
            //stopwatch.Start();

            List<SearchResultModel> applications = new();
            List<string> applicationNames = new();
            //string format = @"(?<![a-z]){0}";
            string format = @".*((?<![a-z]){0}.*)";
            Regex rx = new(String.Format(format, input), RegexOptions.IgnoreCase);

            try
            {
                using (var sr = new StreamReader(applicationsTxtFilePath))
                {
                    string fileContent = sr.ReadToEnd();
                    MatchCollection matches = rx.Matches(fileContent);
                    foreach (Match match in matches)
                    {
                        applicationNames.Add(match.Value.Trim());
                    }
                }
                foreach (string name in applicationNames)
                {
                    if (applicationsDict.TryGetValue(name, out string value))
                    {
                        applications.Add(MakeSearchResultModel(name, value, GetApplicationIcon(applicationImagesDirectoryPath, name)));
                    }
                }
                //foreach (ShellObject application in (IKnownFolder)applicationsFolder)
                //{
                //    cancellationToken.ThrowIfCancellationRequested();

                //    string name = application.Name;
                //    Match match = rx.Match(name);
                //    if (match.Success)
                //    {
                //        applications.Add(MakeSearchResultModel(name, application.ParsingName, GetApplicationIcon(applicationImagesDirectoryPath, name)));
                //    }
                //}
                return applications.OrderByDescending(x => x.Name.StartsWith(input[0].ToString(), StringComparison.InvariantCultureIgnoreCase))
                                   .ThenBy(x => x.Name);
            }
            catch (OperationCanceledException)
            {
                return null;
            }
            //finally
            //{
            //    stopwatch.Stop();
            //    Debug.WriteLine(stopwatch.ElapsedMilliseconds);
            //}

            //for (int i = applications.Count - 1; i >= 0; i--)
            //{
            //    if (applications[i].Name.EndsWith(".url") | applications[i].ID.EndsWith("url"))
            //        applications.RemoveAt(i);
            //}
            //return applications.OrderByDescending(x => x.Name.StartsWith(input[0].ToString(), StringComparison.InvariantCultureIgnoreCase))
            //                   .ThenBy(x => x.Name);
        }

        private BitmapImage GetApplicationIcon(string path, string name)
        {
            string iconPath = Path.Combine(path, name + ".png");
            if (!File.Exists(iconPath))
            {
                iconPath = "pack://application:,,,/Reginald;component/Images/help-light.png";
            }

            BitmapImage icon = new();
            icon.BeginInit();
            icon.UriSource = new Uri(iconPath);
            icon.CacheOption = BitmapCacheOption.OnLoad;
            icon.DecodePixelWidth = 75;
            icon.DecodePixelHeight = 75;
            icon.EndInit();
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

        private SearchResultModel MakeSearchResultModel(string attribute, SearchResultModel.Category category)
        {
            XmlDocument doc = GetXmlDocument("Search");
            XmlNode node = doc.GetNode(attribute);
            if (node is null)
                return new SearchResultModel();

            string name = node["Name"].InnerText;
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

        private SearchResultModel[] MakeSearchResultModels(XmlDocument doc, string attribute, SearchResultModel.Category category, string input)
        {
            XmlNodeList nodes = doc.GetNodes(attribute);
            SearchResultModel[] models = new SearchResultModel[nodes.Count];
            for (int i = 0; i < nodes.Count; i++)
            {
                XmlNode node = nodes[i];
                string name = node["Name"].InnerText;

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
                string text = input == String.Empty ? defaultText : input;
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
                    DefaultText = defaultText,
                    Description = description,
                    Alt = alt
                };
                models[i] = model;
            }
            return models;
        }

        private SearchResultModel[] MakeSearchResultModels(string attribute, SearchResultModel.Category category, string text = null)
        {
            XmlDocument doc = GetXmlDocument("Search");
            XmlNodeList nodes = doc.GetNodes(attribute);
            if (nodes is null)
                return Array.Empty<SearchResultModel>();

            SearchResultModel[] models = new SearchResultModel[nodes.Count];
            for (int i = 0; i < nodes.Count; i++)
            {
                XmlNode node = nodes[i];
                string name = node["Name"].InnerText;

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
                    try
                    {
                        HandleSelectedSearchResultBasedOnCategoryName(SelectedSearchResult.CategoryName);
                    }
                    catch (NullReferenceException) { }
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

                case Key.Tab:
                    TextBox textBox = (TextBox)sender;
                    if (SelectedSearchResult.Keyword is not null)
                    {
                        if (!UserInput.StartsWith(SelectedSearchResult.Keyword, StringComparison.InvariantCultureIgnoreCase))
                        {
                            UserInput = SelectedSearchResult.Keyword;
                            textBox.SelectionStart = UserInput.Length;
                        }
                    }
                    else
                    {
                        if (!UserInput.StartsWith(SelectedSearchResult.Name, StringComparison.InvariantCultureIgnoreCase))
                        {
                            UserInput = SelectedSearchResult.Name;
                            textBox.SelectionStart = UserInput.Length;
                        }
                    }
                    e.Handled = true;
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

        public void SearchResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

        private static Dictionary<string, string> MakeApplicationsDictionary()
        {
            var FOLDERID_AppsFolder = new Guid("{1e87508d-89c2-42f0-8a7e-645a0f50ca58}");
            ShellObject appsFolder = (ShellObject)KnownFolderHelper.FromKnownFolderId(FOLDERID_AppsFolder);

            Dictionary<string, string> applications = new();

            foreach (ShellObject app in (IKnownFolder)appsFolder)
            {
                if (!app.Name.EndsWith("url") && !app.ParsingName.EndsWith("url"))
                {
                    applications.TryAdd(app.Name, app.ParsingName);
                }
            }
            return applications;
        }
    }
}