using Caliburn.Micro;
using Reginald.Core.Base;
using Reginald.Core.Enums;
using Reginald.Core.Extensions;
using Reginald.Core.Helpers;
using Reginald.Core.IO;
using Reginald.Core.Notifications;
using Reginald.Core.Utilities;
using Reginald.Extensions;
using Reginald.Models;
using SourceChord.FluentWPF;
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
using System.Xml;

namespace Reginald.ViewModels
{
    public class SearchViewModel : Screen
    {
        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            Indicator.IsDeactivated = true;
            return base.OnDeactivateAsync(close, cancellationToken);
        }

        public SearchViewModel(Indicator indicator)
        {
            Indicator = indicator;
            SetUpViewModelAsync();
        }

        private string applicationImagesDirectoryPath;
        private string applicationsTxtFilePath;
        private XmlDocument searchDoc;
        private XmlDocument userSearchDoc;
        private XmlDocument specialKeywordDoc;
        private XmlDocument commandsDoc;
        private XmlDocument utilitiesDoc;
        private Dictionary<string, string> applicationsDict;

        public Indicator Indicator { get; set; }

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
            });
            Task<Dictionary<string, string>> applicationsDictTask = Task.Run(() =>
            {
                return Applications.MakeDictionary();
            });

            await assignmentTask;
            applicationsDict = await applicationsDictTask;

            Properties.Settings settings = Properties.Settings.Default;
            System.Drawing.Color searchBackgroundColor;
            System.Drawing.Color searchDescriptionTextColor;
            System.Drawing.Color searchAltTextColor;
            System.Drawing.Color searchInputTextColor;
            System.Drawing.Color searchInputCaretColor;
            System.Drawing.Color searchViewBorderColor;
            System.Drawing.Color specialSearchResultSubColor;
            System.Drawing.Color specialSearchResultBorderColor;
            System.Drawing.Color specialSearchResultSecondaryColor;
            System.Drawing.Color specialSearchResultMainColor;
            System.Drawing.Color searchResultHighlightColor;
            if (settings.IsDarkModeEnabled)
            {
                searchBackgroundColor = settings.SearchBackgroundColorDark;
                searchDescriptionTextColor = settings.SearchDescriptionTextColorDark;
                searchAltTextColor = settings.SearchAltTextColorDark;
                searchInputTextColor = settings.SearchInputTextColorDark;
                searchInputCaretColor = settings.SearchInputCaretColorDark;
                searchViewBorderColor = settings.SearchViewBorderColorDark;
                specialSearchResultSubColor = settings.SpecialSearchResultSubColorDark;
                specialSearchResultBorderColor = settings.SpecialSearchResultBorderColorDark;
                specialSearchResultSecondaryColor = settings.SpecialSearchResultSecondaryColorDark;
                specialSearchResultMainColor = settings.SpecialSearchResultMainColorDark;
            }
            else
            {
                searchBackgroundColor = settings.SearchBackgroundColorLight;
                searchDescriptionTextColor = settings.SearchDescriptionTextColorLight;
                searchAltTextColor = settings.SearchAltTextColorLight;
                searchInputTextColor = settings.SearchInputTextColorLight;
                searchInputCaretColor = settings.SearchInputCaretColorLight;
                searchViewBorderColor = settings.SearchViewBorderColorLight;
                specialSearchResultSubColor = settings.SpecialSearchResultSubColorLight;
                specialSearchResultBorderColor = settings.SpecialSearchResultBorderColorLight;
                specialSearchResultSecondaryColor = settings.SpecialSearchResultSecondaryColorLight;
                specialSearchResultMainColor = settings.SpecialSearchResultMainColorLight;
            }

            Settings.SearchBackgroundColor = Color.FromRgb(searchBackgroundColor.R, searchBackgroundColor.G, searchBackgroundColor.B);
            Settings.SearchDescriptionTextBrush = SolidColorBrushHelper.FromRgb(searchDescriptionTextColor);
            Settings.SearchAltTextBrush = SolidColorBrushHelper.FromRgb(searchAltTextColor);
            Settings.SearchInputTextBrush = SolidColorBrushHelper.FromRgb(searchInputTextColor);
            Settings.SearchInputCaretBrush = SolidColorBrushHelper.FromRgb(searchInputCaretColor);
            Settings.SearchViewBorderBrush = !settings.IsSearchBoxBorderEnabled ? Brushes.Transparent : SolidColorBrushHelper.FromRgb(searchViewBorderColor);
            Settings.SpecialSearchResultSubBrush = SolidColorBrushHelper.FromRgb(specialSearchResultSubColor);
            Settings.SpecialSearchResultBorderBrush = SolidColorBrushHelper.FromRgb(specialSearchResultBorderColor);
            Settings.SpecialSearchResultSecondaryBrush = SolidColorBrushHelper.FromRgb(specialSearchResultSecondaryColor);
            Settings.SpecialSearchResultMainBrush = SolidColorBrushHelper.FromRgb(specialSearchResultMainColor);

            string highlightHex = settings.IsSystemColorEnabled ? AccentColors.ImmersiveSystemAccentBrush.GetTransparentHex("#99") : "#99ff8d00";
            searchResultHighlightColor = System.Drawing.ColorTranslator.FromHtml(highlightHex);
            Settings.SearchResultHighlightColor = SolidColorBrushHelper.FromArgb(searchResultHighlightColor);
        }

        public async void UserInput_TextChangedAsync(object sender, TextChangedEventArgs e)
        {
            SearchResults.Clear();
            SpecialSearchResults.Clear();
            if (UserInput != string.Empty)
            {
                IEnumerable<SearchResultModel> models = Enumerable.Empty<SearchResultModel>();

                Task<IEnumerable<SearchResultModel>> applicationModelsTask = GetApplicationModels(UserInput);
                Task<IEnumerable<SearchResultModel>> keywordModelsTask = GetModels(Properties.Settings.Default.IncludeDefaultKeywords, searchDoc, UserInput);
                Task<IEnumerable<SearchResultModel>> userKeywordModelsTask = GetModels(true, userSearchDoc, UserInput);
                Task<IEnumerable<SearchResultModel>> commandKeywordModelsTask = GetCommandModelsAsync(Properties.Settings.Default.IncludeCommands, commandsDoc, UserInput);
                Task<IEnumerable<SearchResultModel>> utilityKeywordModelsTask = GetUtilityModels(true, utilitiesDoc, UserInput);
                Task<SearchResultModel[]> mathModelsTask = GetMathModels(UserInput);
                Task<SpecialSearchResultModel> specialKeywordModelTask = GetSpecialKeywordModelAsync(UserInput);

                IEnumerable<SearchResultModel> applicationModels = await applicationModelsTask;
                IEnumerable<SearchResultModel> keywordModels = await keywordModelsTask;
                IEnumerable<SearchResultModel> userKeywordModels = await userKeywordModelsTask;
                IEnumerable<SearchResultModel> commandModels = await commandKeywordModelsTask;
                IEnumerable<SearchResultModel> utilityModels = await utilityKeywordModelsTask;
                SearchResultModel[] mathModels = await mathModelsTask;
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
                        models = SearchResultModel.MakeArray(searchDoc, UserInput, "__http", Category.HTTP);
                    }
                    else
                    {
                        try
                        {
                            models = applicationModels.Concat(keywordModels).Concat(userKeywordModels).Concat(mathModels).Concat(commandModels).Concat(utilityModels);
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
                return Task.FromResult(Enumerable.Empty<SearchResultModel>());
            }
            catch (RegexParseException)
            {
                return Task.FromResult(Enumerable.Empty<SearchResultModel>());
            }
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
                    models = models.Concat(SearchResultModel.MakeList(doc, description, k, Category.Keyword));
                }
                return Task.FromResult(models);
            }
            return Task.FromResult(Enumerable.Empty<SearchResultModel>());
        }

        private IEnumerable<string> MatchKeywordsInDoc(XmlDocument doc, string keyword, string separator)
        {
            IEnumerable<string> attributes = doc.GetNodesAttributes(Constants.NamespacesXpath);
            string format = @"((?<!\w){0}.*)";
            Regex rx = new(string.Format(format, keyword.Replace("[", "\\[")), RegexOptions.IgnoreCase);
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

        private Task<SearchResultModel[]> GetMathModels(string input)
        {
            if (UserInput.IsMathExpression())
            {
                return Task.FromResult(SearchResultModel.MakeArray(searchDoc, input, "__math", Category.Math, input.Eval()));
            }
            return Task.FromResult(Array.Empty<SearchResultModel>());
        }

        private CancellationTokenSource tokenSource;
        private async Task<SpecialSearchResultModel> GetSpecialKeywordModelAsync(string input)
        {
            if (Properties.Settings.Default.IncludeSpecialKeywords)
            {
                if (tokenSource is not null)
                {
                    tokenSource.Cancel();
                }

                XmlNode node = specialKeywordDoc.GetNode(string.Format(Constants.NamespaceNameXpathFormat, input));
                if (node is not null)
                {
                    bool isEnabled = bool.Parse(node["IsEnabled"].InnerText);
                    if (isEnabled)
                    {
                        bool isCommand = bool.Parse(node["IsCommand"].InnerText);
                        if (isCommand)
                        {
                            tokenSource = new();
                            SpecialSearchResultModel model = new();

                            string apiText = node["API"].InnerText;
                            Api api = (Api)Enum.Parse(typeof(Api), apiText);
                            switch (api)
                            {
                                case Api.Cloudflare:
                                    model = await SpecialSearchResultModel.MakeCloudflareSpecialSearchResultModelAsync(node, tokenSource.Token);
                                    break;

                                default:
                                    break;
                            }
                            return model;
                        }
                    }
                }
                else
                {
                    (string Left, string Separator, string Right) partition = input.Partition(" ");
                    if (partition.Right != string.Empty)
                    {
                        node = specialKeywordDoc.GetNode(string.Format(Constants.NamespaceNameXpathFormat, partition.Left));
                        if (node is not null)
                        {
                            bool isEnabled = bool.Parse(node["IsEnabled"].InnerText);
                            if (isEnabled)
                            {
                                bool isCommand = bool.Parse(node["IsCommand"].InnerText);
                                if (!isCommand)
                                {
                                    bool canHaveSpaces = bool.Parse(node["CanHaveSpaces"].InnerText);
                                    if (!canHaveSpaces)
                                    {
                                        if (partition.Right.Contains(" "))
                                        {
                                            return null;
                                        }
                                    }

                                    tokenSource = new();
                                    SpecialSearchResultModel model = new();

                                    string apiText = node["API"].InnerText;
                                    Api api = (Api)Enum.Parse(typeof(Api), apiText);
                                    switch (api)
                                    {
                                        case Api.Styvio:
                                            if (partition.Right.Length > 5)
                                            {
                                                return null;
                                            }
                                            model = await SpecialSearchResultModel.MakeStyvioSpecialSearchResultModelAsync(node, partition.Right, tokenSource.Token);
                                            break;

                                        default:
                                            break;
                                    }
                                    return model;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        private async Task<IEnumerable<SearchResultModel>> GetCommandModelsAsync(bool isIncluded, XmlDocument doc, string input)
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

        private Task<IEnumerable<SearchResultModel>> GetUtilityModels(bool isIncluded, XmlDocument doc, string input)
        {
            if (isIncluded)
            {
                IEnumerable<string> keywords = MatchKeywordsInDoc(doc, input, string.Empty);

                IEnumerable<SearchResultModel> models = Array.Empty<SearchResultModel>();
                foreach (string k in keywords)
                {
                    models = models.Concat(SearchResultModel.MakeListForUtilities(doc, k, Category.Utility));
                }
                return Task.FromResult(models);
            }
            return Task.FromResult(Enumerable.Empty<SearchResultModel>());
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
                    Process.Start("explorer.exe", @"shell:appsfolder\" + SelectedSearchResult.ParsingName);
                    await TryCloseAsync();
                    break;

                case Category.Math:
                    Clipboard.SetText(SelectedSearchResult.Text);
                    await TryCloseAsync();
                    break;

                case Category.Keyword:
                    {
                        string uri = string.Format(SelectedSearchResult.URL, SelectedSearchResult.Text);
                        GoToWebsite(uri);
                        break;
                    }

                case Category.HTTP:
                    {
                        string uri = UserInput;
                        if (!Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                            uri = uri.PrependScheme();
                        GoToWebsite(uri);
                        break;
                    }

                case Category.SearchEngine:
                    {
                        string uri = string.Format(SelectedSearchResult.URL, UserInput.Quote(SelectedSearchResult.Separator));
                        GoToWebsite(uri);
                        break;
                    }

                case Category.Notifier:
                    if (!string.IsNullOrEmpty(SelectedSearchResult.Text))
                    {
                        await TryCloseAsync();
                        await Task.Delay((int)SelectedSearchResult.Time * 1000);
                        ToastNotifications.SendSimpleToastNotification(SelectedSearchResult.Name, SelectedSearchResult.Text);
                    }
                    break;

                case Category.Utility:
                    if (SelectedSearchResult.RequiresConfirmation)
                    {
                        string confirmationMessage = SelectedSearchResult.ConfirmationMessage;
                        Utility utility = SelectedSearchResult.Utility;
                        SearchResults.Clear();
                        SearchResults.Add(new SearchResultModel(confirmationMessage, utility));
                        SelectedSearchResult = SearchResults[0];
                    }
                    else
                    {
                        await TryCloseAsync();
                        await UtilityBase.HandleUtilityAsync(SelectedSearchResult.Utility);
                    }
                    break;

                case Category.Confirmation:
                    await TryCloseAsync();
                    await UtilityBase.HandleUtilityAsync(SelectedSearchResult.Utility);
                    break;

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
    }
}