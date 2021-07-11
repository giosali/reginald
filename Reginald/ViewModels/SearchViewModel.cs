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
        private string applicationImagesDirectoryPath;
        private string applicationsTxtFilePath;
        private XmlDocument searchDoc;
        private XmlDocument userSearchDoc;
        private Dictionary<string, string> applicationsDict;

        public SearchViewModel()
        {
            SetUpViewModel();
        }

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

        private string _userInput;
        public string UserInput
        {
            get => _userInput;
            set
            {
                _userInput = value;
                NotifyOfPropertyChange(() => UserInput);
                foreach (SearchResultModel model in SearchResults)
                {
                    if (model.CategoryName == SearchResultModel.Category.Application)
                    {
                        continue;
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
            }
        }

        private BindableCollection<SearchResultModel> _searchResults = new();
        public BindableCollection<SearchResultModel> SearchResults
        {
            get => _searchResults;
            set
            {
                _searchResults = value;
                NotifyOfPropertyChange(() => SearchResults);
                IsVisible = SearchResults.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private SearchResultModel _selectedSearchResult = new();
        public SearchResultModel SelectedSearchResult
        {
            get => _selectedSearchResult;
            set
            {
                LastSelectedSearchResult = SelectedSearchResult;
                _selectedSearchResult = value;
                NotifyOfPropertyChange(() => SelectedSearchResult);
            }
        }

        private SearchResultModel _lastSelectedSearchResult;
        public SearchResultModel LastSelectedSearchResult
        {
            get => _lastSelectedSearchResult;
            set
            {
                _lastSelectedSearchResult = value;
                NotifyOfPropertyChange(() => LastSelectedSearchResult);
            }
        }

        private Visibility _isVisible;
        public Visibility IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                NotifyOfPropertyChange(() => IsVisible);
            }
        }

        private async void SetUpViewModel()
        {
            applicationImagesDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Reginald", "ApplicationIcons");
            applicationsTxtFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Reginald", "Applications.txt");
            searchDoc = GetXmlDocument("Search");
            userSearchDoc = GetXmlDocument("UserSearch");
            applicationsDict = await Task.Run(() =>
            {
                return MakeApplicationsDictionary();
            });

            Properties.Settings settings = Properties.Settings.Default;
            System.Drawing.Color searchBackgroundColor;
            System.Drawing.Color searchDescriptionTextColor;
            System.Drawing.Color searchAltTextColor;
            System.Drawing.Color searchInputTextColor;
            System.Drawing.Color searchInputCaretColor;
            if (settings.IsDarkModeEnabled)
            {
                searchBackgroundColor = settings.SearchBackgroundColorDark;
                searchDescriptionTextColor = settings.SearchDescriptionTextColorDark;
                searchAltTextColor = settings.SearchAltTextColorDark;
                searchInputTextColor = settings.SearchInputTextColorDark;
                searchInputCaretColor = settings.SearchInputCaretColorDark;
            }
            else
            {
                searchBackgroundColor = settings.SearchBackgroundColorLight;
                searchDescriptionTextColor = settings.SearchDescriptionTextColorLight;
                searchAltTextColor = settings.SearchAltTextColorLight;
                searchInputTextColor = settings.SearchInputTextColorLight;
                searchInputCaretColor = settings.SearchInputCaretColorLight;
            }
            Settings.SearchBackgroundColor = Color.FromRgb(searchBackgroundColor.R, searchBackgroundColor.G, searchBackgroundColor.B);
            Settings.SearchDescriptionTextBrush = new SolidColorBrush(Color.FromRgb(searchDescriptionTextColor.R, searchDescriptionTextColor.G, searchDescriptionTextColor.B));
            Settings.SearchAltTextBrush = new SolidColorBrush(Color.FromRgb(searchAltTextColor.R, searchAltTextColor.G, searchAltTextColor.B));
            Settings.SearchInputTextBrush = new SolidColorBrush(Color.FromRgb(searchInputTextColor.R, searchInputTextColor.G, searchInputTextColor.B));
            Settings.SearchInputCaretBrush = new SolidColorBrush(Color.FromRgb(searchInputCaretColor.R, searchInputCaretColor.G, searchInputCaretColor.B));
        }

        public async void UserInput_TextChangedAsync(object sender, TextChangedEventArgs e)
        {
            if (UserInput != String.Empty)
            {
                IEnumerable<SearchResultModel> models;
                Task<IEnumerable<SearchResultModel>> applicationModelsTask;
                Task<IEnumerable<SearchResultModel>> keywordModelsTask;
                Task<IEnumerable<SearchResultModel>> userKeywordModelsTask;
                Task<SearchResultModel[]> mathModelsTask = null;

                if (UserInput.HasTopLevelDomain())
                {
                    models = MakeSearchResultModels("__http", SearchResultModel.Category.HTTP);
                }
                else
                {
                    applicationModelsTask = Task.Run(() =>
                    {
                        if (Properties.Settings.Default.IncludeInstalledApplications)
                            return GetApplications(UserInput);
                        else
                            return Enumerable.Empty<SearchResultModel>();
                    });

                    keywordModelsTask = Task.Run(() =>
                    {
                        if (Properties.Settings.Default.IncludeDefaultKeywords)
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
                        }
                        else
                            return Enumerable.Empty<SearchResultModel>();
                    });

                    userKeywordModelsTask = Task.Run(() =>
                    {
                        (string Left, string Separator, string Right) partition = UserInput.Partition(" ");

                        List<string> attributes = userSearchDoc.GetNodesAttributes();
                        string format = @"((?<!\w){0}.*)";
                        Regex rx = new(String.Format(format, partition.Left), RegexOptions.IgnoreCase);
                        IEnumerable<string> matches = attributes.Where(x => rx.IsMatch(x))
                                                                .Distinct();

                        IEnumerable<SearchResultModel> userKeywordModels = Array.Empty<SearchResultModel>();
                        foreach (string match in matches)
                        {
                            userKeywordModels = userKeywordModels.Concat(MakeSearchResultModels(userSearchDoc, match, SearchResultModel.Category.Keyword, partition.Right));
                        }
                        return userKeywordModels;
                    });

                    mathModelsTask = Task.Run(() =>
                    {
                        if (UserInput.IsMathExpression())
                            return MakeSearchResultModels("__math", SearchResultModel.Category.Math, UserInput.Eval());
                        return Array.Empty<SearchResultModel>();
                    });

                    IEnumerable<SearchResultModel> applicationModels = await applicationModelsTask;
                    IEnumerable<SearchResultModel> keywordModels = await keywordModelsTask;
                    IEnumerable<SearchResultModel> userKeywordModels = await userKeywordModelsTask;
                    SearchResultModel[] mathModels = await mathModelsTask;

                    try
                    {
                        models = applicationModels.Concat(keywordModels).Concat(userKeywordModels).Concat(mathModels);
                    }
                    catch (ArgumentNullException)
                    {
                        return;
                    }

                    if (!models.Any())
                    {
                        models = new SearchResultModel[3]
                        {
                            MakeSearchResultModel("g", SearchResultModel.Category.SearchEngine),
                            MakeSearchResultModel("ddg", SearchResultModel.Category.SearchEngine),
                            MakeSearchResultModel("amazon", SearchResultModel.Category.SearchEngine)
                        };
                    }
                }

                SearchResults.Clear();
                SearchResults.AddRange(models);
                SelectedSearchResult = SearchResults[0];
            }
            else
            {
                SearchResults.Clear();
            }
        }

        private IEnumerable<SearchResultModel> GetApplications(string input)
        {
            List<SearchResultModel> applications = new();
            List<string> applicationNames = new();
            string format = @".*((?<![a-z]){0}.*)";
            Regex rx = new(String.Format(format, input), RegexOptions.IgnoreCase);

            using (StreamReader sr = new(applicationsTxtFilePath))
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
            return applications.OrderByDescending(x => x.Name.StartsWith(input[0].ToString(), StringComparison.InvariantCultureIgnoreCase))
                               .ThenBy(x => x.Name);
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

        private SearchResultModel MakeSearchResultModel(string name, string parsingName, BitmapImage icon)
        {
            SearchResultModel model = new()
            {
                Name = name,
                CategoryName = SearchResultModel.Category.Application,
                ParsingName = parsingName,
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

        private List<SearchResultModel> MakeSearchResultModels(XmlDocument doc, string attribute, SearchResultModel.Category category, string input)
        {
            XmlNodeList nodes = doc.GetNodes(attribute);
            if (nodes is null)
                return new List<SearchResultModel>();
            List<SearchResultModel> models = new();
            for (int i = 0; i < nodes.Count; i++)
            {
                XmlNode node = nodes[i];
                string name = node["Name"].InnerText;

                XmlNode isEnabledNode = node["IsEnabled"];
                if (isEnabledNode is not null)
                {
                    bool isEnabled = Boolean.Parse(isEnabledNode.InnerText);
                    if (!isEnabled)
                        continue;
                }
                else
                    continue;

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

                models.Add(new SearchResultModel
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
                });
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
                    Process.Start("explorer.exe", @"shell:appsfolder\" + SelectedSearchResult.ParsingName);
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
                SelectedSearchResult = LastSelectedSearchResult;
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
                ProcessStartInfo startInfo = new()
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
            ShellObject applicationsFolder = (ShellObject)KnownFolderHelper.FromKnownFolderId(new Guid("{1e87508d-89c2-42f0-8a7e-645a0f50ca58}"));
            Dictionary<string, string> applications = new();

            foreach (ShellObject app in (IKnownFolder)applicationsFolder)
            {
                if (!app.Name.EndsWith(".url") && !app.ParsingName.EndsWith("url"))
                {
                    applications.TryAdd(app.Name, app.ParsingName);
                }
            }
            return applications;
        }
    }
}