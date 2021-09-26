using Caliburn.Micro;
using Reginald.Core.Base;
using Reginald.Core.Enums;
using Reginald.Core.Helpers;
using Reginald.Core.IO;
using Reginald.Core.Notifications;
using Reginald.Core.Utilities;
using Reginald.Core.Utils;
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
using System.Xml;

namespace Reginald.ViewModels
{
    public class SearchViewModel : Screen
    {
        public SearchViewModel()
        {
            SetUpViewModelAsync();
        }

        private string applicationImagesDirectoryPath;
        private string applicationsTxtFilePath;
        private XmlDocument searchDoc;
        private XmlDocument userSearchDoc;
        private XmlDocument specialKeywordDoc;
        private XmlDocument commandsDoc;
        private XmlDocument utilitiesDoc;
        private XmlDocument settingsDoc;
        private Dictionary<string, string> applicationsDict;

        private async void SetUpViewModelAsync()
        {
            Task assignmentTask = Task.Run(() =>
            {
                applicationImagesDirectoryPath = Path.Combine(ApplicationPaths.AppDataDirectoryPath, ApplicationPaths.ApplicationName, ApplicationPaths.IconsDirectoryName);
                applicationsTxtFilePath = Path.Combine(ApplicationPaths.AppDataDirectoryPath, ApplicationPaths.ApplicationName, ApplicationPaths.TxtFilename);
                searchDoc = XmlHelper.GetXmlDocument(ApplicationPaths.XmlKeywordFilename);
                userSearchDoc = XmlHelper.GetXmlDocument(ApplicationPaths.XmlUserKeywordFilename);
                specialKeywordDoc = XmlHelper.GetXmlDocument(ApplicationPaths.XmlSpecialKeywordFilename);
                commandsDoc = XmlHelper.GetXmlDocument(ApplicationPaths.XmlCommandsFilename);
                utilitiesDoc = XmlHelper.GetXmlDocument(ApplicationPaths.XmlUtilitiesFilename);
                settingsDoc = XmlHelper.GetXmlDocument("Resources/MSSettings.xml", true);
            });
            Task<Dictionary<string, string>> applicationsDictTask = Task.Run(() =>
            {
                return Applications.MakeDictionary();
            });
            Task stylesTask = Task.Run(() =>
            {
                StyleSearchView();
            });

            await assignmentTask;
            applicationsDict = await applicationsDictTask;
            await stylesTask;
        }

        public void StyleSearchView()
        {
            XmlDocument doc = XmlHelper.GetXmlDocument(ApplicationPaths.XmlThemesFileLocation, true);
            XmlNodeList nodes = doc.GetNodes(Constants.ThemesXpath);
            ThemeModel model = new();
            for (int i = 0; i < nodes.Count; i++)
            {
                XmlNode node = nodes[i];
                _ = Guid.TryParse(node["GUID"]?.InnerText, out Guid result);
                if (result == Properties.Settings.Default.ThemeIdentifier)
                {
                    model = new ThemeModel(node);
                    break;
                }
            }
            Theme = model;
        }

        public void ShowOrHide()
        {
            Window window = GetView() as Window;
            if (window is not null)
            {
                if (window.IsVisible)
                {
                    window.Hide();
                    MousePosition = new();
                    FileOperations.CacheApplications();
                    applicationsDict = Applications.MakeDictionary();
                }
                else
                {
                    window.Show();
                }
            }
        }

        private ThemeModel _theme = new();
        public ThemeModel Theme
        {
            get => _theme;
            set
            {
                _theme = value;
                NotifyOfPropertyChange(() => Theme);
            }
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

        private BindableCollection<SpecialSearchResultModel> _specialSearchResults = new();
        public BindableCollection<SpecialSearchResultModel> SpecialSearchResults
        {
            get => _specialSearchResults;
            set
            {
                _specialSearchResults = value;
                NotifyOfPropertyChange(() => SpecialSearchResults);
            }
        }

        private SpecialSearchResultModel _specialSearchResult;
        public SpecialSearchResultModel SpecialSearchResult
        {
            get => _specialSearchResult;
            set
            {
                _specialSearchResult = value;
                NotifyOfPropertyChange(() => SpecialSearchResult);
                SpecialSearchResults.Add(SpecialSearchResult);
            }
        }

        private bool _isMouseOverChanged;
        public bool IsMouseOverChanged
        {
            get => _isMouseOverChanged;
            set
            {
                _isMouseOverChanged = value;
                NotifyOfPropertyChange(() => IsMouseOverChanged);
            }
        }

        private Point _mousePosition;
        public Point MousePosition
        {
            get => _mousePosition;
            set
            {
                _mousePosition = value;
                NotifyOfPropertyChange(() => MousePosition);
            }
        }

        public async void UserInput_TextChangedAsync(object sender, TextChangedEventArgs e)
        {
            SearchResults.Clear();
            SpecialSearchResults.Clear();
            IsMouseOverChanged = false;
            MousePosition = new();
            if (UserInput != string.Empty)
            {
                IEnumerable<SearchResultModel> models = Enumerable.Empty<SearchResultModel>();

                Task<IEnumerable<SearchResultModel>> applicationModelsTask = GetApplicationModels(UserInput);
                Task<IEnumerable<SearchResultModel>> keywordModelsTask = GetModels(Properties.Settings.Default.IncludeDefaultKeywords, searchDoc, UserInput);
                Task<IEnumerable<SearchResultModel>> userKeywordModelsTask = GetModels(true, userSearchDoc, UserInput);
                Task<IEnumerable<SearchResultModel>> commandKeywordModelsTask = GetCommandModelsAsync(Properties.Settings.Default.IncludeCommands, commandsDoc, UserInput);
                Task<IEnumerable<SearchResultModel>> utilityKeywordModelsTask = GetUtilityModels(Properties.Settings.Default.IncludeUtilities, utilitiesDoc, UserInput);
                Task<List<SearchResultModel>> mathModelsTask = GetMathModels(UserInput);
                Task<List<SearchResultModel>> settingsModelsTask = GetSettingsModels(Properties.Settings.Default.IncludeSettingsPages, settingsDoc, UserInput);
                Task<SpecialSearchResultModel> specialKeywordModelTask = GetSpecialKeywordModelAsync(UserInput);

                IEnumerable<SearchResultModel> applicationModels = await applicationModelsTask;
                IEnumerable<SearchResultModel> keywordModels = await keywordModelsTask;
                IEnumerable<SearchResultModel> userKeywordModels = await userKeywordModelsTask;
                IEnumerable<SearchResultModel> commandModels = await commandKeywordModelsTask;
                IEnumerable<SearchResultModel> utilityModels = await utilityKeywordModelsTask;
                List<SearchResultModel> mathModels = await mathModelsTask;
                List<SearchResultModel> settingsModels = await settingsModelsTask;
                SpecialSearchResultModel specialSearchResult = await specialKeywordModelTask;

                if (specialSearchResult is not null)
                {
                    if (specialSearchResult.IsCancelled)
                        return;
                    SpecialSearchResult = specialSearchResult;
                }
                else
                {
                    if (UserInput.HasScheme() || UserInput.HasTopLevelDomain())
                    {
                        models = SearchResultModel.MakeList(searchDoc, "__http", Category.HTTP, UserInput);
                    }
                    else
                    {
                        try
                        {
                            models = applicationModels.Concat(keywordModels).Concat(userKeywordModels).Concat(mathModels).Concat(commandModels).Concat(utilityModels).Concat(settingsModels);
                        }
                        catch (ArgumentNullException)
                        {
                            return;
                        }

                        if (!models.Any())
                        {
                            models = new SearchResultModel[3]
                            {
                                    new SearchResultModel(searchDoc, UserInput, "g", Category.SearchEngine),
                                    new SearchResultModel(searchDoc, UserInput, "ddg", Category.SearchEngine),
                                    new SearchResultModel(searchDoc, UserInput, "amazon", Category.SearchEngine),
                            };
                        }
                    }

                    SearchResults.AddRange(models);
                    SelectedSearchResult = SearchResults[0];
                }
            }
        }

        private Task<IEnumerable<SearchResultModel>> GetApplicationModels(string input)
        {
            try
            {
                if (Properties.Settings.Default.IncludeInstalledApplications)
                {
                    List<SearchResultModel> applications = new();
                    List<string> applicationNames = new();
                    string format = @".*((?<![a-z]){0}.*)";
                    Regex rx = new(string.Format(format, input), RegexOptions.IgnoreCase);

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
                            applications.Add(new SearchResultModel(name, value, BitmapImageHelper.GetIcon(applicationImagesDirectoryPath, name)));
                        }
                    }
                    IEnumerable<SearchResultModel> models = applications.OrderByDescending(x => x.Name.StartsWith(input[0].ToString(), StringComparison.InvariantCultureIgnoreCase))
                                                                        .ThenBy(x => x.Name);
                    return Task.FromResult(models);
                }
            }
            catch (RegexParseException) { }
            return Task.FromResult(Enumerable.Empty<SearchResultModel>());
        }

        private Task<IEnumerable<SearchResultModel>> GetModels(bool isIncluded, XmlDocument doc, string input)
        {
            if (isIncluded)
            {
                (string keyword, string separator, string description) = input.Partition(" ");
                IEnumerable<string> keywords = MatchKeywordsInDoc(doc, keyword, separator);

                IEnumerable<SearchResultModel> models = Array.Empty<SearchResultModel>();
                foreach (string k in keywords)
                {
                    models = models.Concat(SearchResultModel.MakeList(doc, k, Category.Keyword, description));
                }
                return Task.FromResult(models);
            }
            return Task.FromResult(Enumerable.Empty<SearchResultModel>());
        }

        private static IEnumerable<string> MatchKeywordsInDoc(XmlDocument doc, string keyword, string separator)
        {
            IEnumerable<string> attributes = doc.GetNodesAttributes(Constants.NamespacesXpath);
            string format = @"((?<!\w){0}.*)";

            keyword = StringHelper.RegexClean(keyword);

            Regex rx = new(string.Format(format, keyword), RegexOptions.IgnoreCase);
            IEnumerable<string> matches = attributes.Where(x => rx.IsMatch(x))
                                                    .Distinct()
                                                    .Where(x =>
                                                    {
                                                        if (separator != string.Empty)
                                                        {
                                                            if (keyword != x)
                                                            {
                                                                return false;
                                                            }
                                                        }
                                                        return true;
                                                    });
            return matches;
        }

        private Task<List<SearchResultModel>> GetMathModels(string input)
        {
            if (UserInput.IsMathExpression())
            {
                return Task.FromResult(SearchResultModel.MakeList(searchDoc, "__math", Category.Math, input, input.Eval()));
            }
            return Task.FromResult(new List<SearchResultModel>());
        }

        private CancellationTokenSource tokenSource;
        private async Task<SpecialSearchResultModel> GetSpecialKeywordModelAsync(string input)
        {
            try
            {
                if (Properties.Settings.Default.IncludeSpecialKeywords)
                {
                    if (tokenSource is not null)
                    {
                        tokenSource.Cancel();
                    }

                    (string keyword, string separator, string after) = input.Partition(" ");
                    XmlNode node = specialKeywordDoc.GetNode(string.Format(Constants.NamespaceNameXpathFormat, keyword));
                    if (node is not null)
                    {
                        bool isEnabled = bool.Parse(node["IsEnabled"].InnerText);
                        if (isEnabled)
                        {
                            bool isCommand = bool.Parse(node["IsCommand"].InnerText);
                            bool canHaveSpaces = bool.Parse(node["CanHaveSpaces"].InnerText);
                            if ((isCommand && separator != string.Empty) || (!canHaveSpaces && after.Contains(" ")))
                            {
                                return null;
                            }

                            tokenSource = new();
                            SpecialSearchResultModel model = new();

                            string apiText = node["API"].InnerText;
                            Api api = (Api)Enum.Parse(typeof(Api), apiText);
                            switch (api)
                            {
                                case Api.Cloudflare:
                                    model = await SpecialSearchResultModel.CloudflareAsync(node, tokenSource.Token);
                                    break;

                                case Api.Styvio:
                                    if (after == string.Empty || after.Length > 5)
                                    {
                                        return null;
                                    }
                                    model = await SpecialSearchResultModel.StyvioAsync(node, after, tokenSource.Token);
                                    break;

                                default:
                                    break;
                            }

                            return model;
                        }
                    }
                }
            }
            catch (System.Xml.XPath.XPathException) { }
            return null;
        }

        private static async Task<IEnumerable<SearchResultModel>> GetCommandModelsAsync(bool isIncluded, XmlDocument doc, string input)
        {
            if (isIncluded)
            {
                (string keyword, string separator, string description) = input.Partition(" ");
                IEnumerable<string> keywords = MatchKeywordsInDoc(doc, keyword, separator);

                IEnumerable<SearchResultModel> models = Array.Empty<SearchResultModel>();
                foreach (string k in keywords)
                {
                    models = models.Concat(await SearchResultModel.MakeListForCommandsAsync(doc, description, k, Category.Notifier));
                }
                return models;
            }
            return Array.Empty<SearchResultModel>();
        }

        private static Task<IEnumerable<SearchResultModel>> GetUtilityModels(bool isIncluded, XmlDocument doc, string input)
        {
            if (isIncluded)
            {
                IEnumerable<string> keywords = MatchKeywordsInDoc(doc, input, string.Empty);

                IEnumerable<SearchResultModel> models = Array.Empty<SearchResultModel>();
                foreach (string k in keywords)
                {
                    models = models.Concat(SearchResultModel.MakeList(doc, k, Category.Utility));
                }
                return Task.FromResult(models);
            }
            return Task.FromResult(Enumerable.Empty<SearchResultModel>());
        }

        private static Task<List<SearchResultModel>> GetSettingsModels(bool isIncluded, XmlDocument doc, string input)
        {
            if (isIncluded)
            {
                if (input.Length > 2)
                {
                    string format = @"{0}";
                    string formatInput = StringHelper.RegexOrBoundarySplit(StringHelper.RegexClean(input), out int count);
                    Regex rx = new(string.Format(format, formatInput), RegexOptions.IgnoreCase);
                    IEnumerable<string> attributes = doc.GetNodesAttributes(Constants.NamespacesXpath)
                                                        .Where(x =>
                                                         {
                                                             MatchCollection matches = rx.Matches(x);
                                                             return matches.Count == count;
                                                         });
                    List<SearchResultModel> models = new();
                    foreach (string attribute in attributes)
                    {
                        XmlNode node = doc.GetNode(string.Format(Constants.NamespaceNameXpathFormat, attribute));
                        models.Add(new SearchResultModel(node, Category.URI));
                    }

                    return Task.FromResult(models);
                }
            }
            return Task.FromResult(new List<SearchResultModel>());
        }

        public void UserInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (SearchResults.Any())
                {
                    switch (e.Key)
                    {
                        case Key.Enter:
                            _ = HandleSelectedSearchResultBasedOnCategoryNameAsync(SelectedSearchResult.Category);
                            e.Handled = true;
                            break;

                        case Key.Up:
                            SelectedSearchResult = SearchResults[SearchResults.IndexOf(SelectedSearchResult) - 1];
                            break;

                        case Key.Down:
                            SelectedSearchResult = SearchResults[SearchResults.IndexOf(SelectedSearchResult) + 1];
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
            }
            catch (NullReferenceException)
            {
                e.Handled = true;
            }
            catch (ArgumentOutOfRangeException) { }
        }

        public void SearchResults_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _ = HandleSelectedSearchResultBasedOnCategoryNameAsync(SelectedSearchResult.Category);
        }

        private async Task HandleSelectedSearchResultBasedOnCategoryNameAsync(Category category)
        {
            switch (category)
            {
                case Category.Application:
                    _ = Process.Start("explorer.exe", @"shell:appsfolder\" + SelectedSearchResult.ParsingName);
                    ShowOrHide();
                    break;

                case Category.Math:
                    Clipboard.SetText(SelectedSearchResult.Text);
                    ShowOrHide();
                    break;

                case Category.Keyword:
                    {
                        string uri = string.Format(SelectedSearchResult.URL, SelectedSearchResult.Text);
                        UriUtils.GoTo(uri);
                        break;
                    }

                case Category.HTTP:
                    {
                        string uri = UserInput;
                        if (!Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                        {
                            uri = uri.PrependScheme();
                        }
                        UriUtils.GoTo(uri);
                        break;
                    }

                case Category.SearchEngine:
                    {
                        string uri = string.Format(SelectedSearchResult.URL, UserInput.Quote(SelectedSearchResult.Separator));
                        UriUtils.GoTo(uri);
                        break;
                    }

                case Category.Notifier:
                    if (!string.IsNullOrEmpty(SelectedSearchResult.Text))
                    {
                        ShowOrHide();
                        string name = SelectedSearchResult.Name;
                        string text = SelectedSearchResult.Text;
                        await Task.Delay((int)SelectedSearchResult.Time * 1000);
                        ToastNotifications.SendSimpleToastNotification(name, text);
                    }
                    break;

                case Category.Utility:
                    if (SelectedSearchResult.RequiresConfirmation)
                    {
                        string confirmationMessage = SelectedSearchResult.ConfirmationMessage;
                        Utility? utility = SelectedSearchResult.Utility;
                        SearchResults.Clear();
                        SearchResults.Add(new SearchResultModel(confirmationMessage, utility));
                        SelectedSearchResult = SearchResults[0];
                    }
                    else
                    {
                        ShowOrHide();
                        await UtilityBase.HandleUtilityAsync(SelectedSearchResult.Utility);
                    }
                    break;

                case Category.Confirmation:
                    ShowOrHide();
                    await UtilityBase.HandleUtilityAsync(SelectedSearchResult.Utility);
                    break;

                case Category.URI:
                    UriUtils.GoTo(SelectedSearchResult.URI);
                    break;

                default:
                    break;
            }
        }

        public void SearchResults_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                bool lastSelectedSearchResultRemains = false;
                for (int i = 0; i < SearchResults.Count; i++)
                {
                    if (LastSelectedSearchResult == SearchResults[i])
                    {
                        SelectedSearchResult = LastSelectedSearchResult;
                        lastSelectedSearchResultRemains = true;
                        break;
                    }
                }
                if (!lastSelectedSearchResultRemains)
                {
                    SelectedSearchResult = SearchResults[0];
                }
            }
            catch (ArgumentOutOfRangeException) { }
        }

        public void SearchResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (sender as ListBox).ScrollIntoView(SelectedSearchResult);
        }

        public void SearchResults_MouseMove(object sender, MouseEventArgs e)
        {
            Point position = e.GetPosition((IInputElement)sender);
            Point defaultPosition = new();
            if (position != MousePosition && MousePosition != defaultPosition)
            {
                IsMouseOverChanged = true;
            }
            MousePosition = position;
        }
    }
}