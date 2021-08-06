using Caliburn.Micro;
using Reginald.Core.Base;
using Reginald.Core.Enums;
using Reginald.Core.Helpers;
using Reginald.Core.IO;
using Reginald.Core.Notifications;
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
        private Dictionary<string, string> applicationsDict;

        private Indicator _indicator;
        public Indicator Indicator
        {
            get => _indicator;
            set
            {
                _indicator = value;
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
        }

        public async void UserInput_TextChangedAsync(object sender, TextChangedEventArgs e)
        {
            SearchResults.Clear();
            SpecialSearchResults.Clear();
            if (UserInput != string.Empty)
            {
                IEnumerable<SearchResultModel> models = Enumerable.Empty<SearchResultModel>();

                Task<IEnumerable<SearchResultModel>> applicationModelsTask = GetApplicationModels(UserInput);
                //Task<IEnumerable<SearchResultModel>> keywordModelsTask = GetKeywordModels(UserInput);
                Task<IEnumerable<SearchResultModel>> keywordModelsTask = GetModels(Properties.Settings.Default.IncludeDefaultKeywords, searchDoc, UserInput);
                //Task<IEnumerable<SearchResultModel>> userKeywordModelsTask = GetUserKeywordModels(UserInput);
                Task<IEnumerable<SearchResultModel>> userKeywordModelsTask = GetModels(true, userSearchDoc, UserInput);
                Task<IEnumerable<SearchResultModel>> commandKeywordModelsTask = GetCommandModelsAsync(commandsDoc, UserInput);
                Task<SearchResultModel[]> mathModelsTask = GetMathModels(UserInput);
                Task<SpecialSearchResultModel> specialKeywordModelTask = GetSpecialKeywordModelAsync(UserInput);

                IEnumerable<SearchResultModel> applicationModels = await applicationModelsTask;
                IEnumerable<SearchResultModel> keywordModels = await keywordModelsTask;
                IEnumerable<SearchResultModel> userKeywordModels = await userKeywordModelsTask;
                IEnumerable<SearchResultModel> commandModels = await commandKeywordModelsTask;
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
                            models = applicationModels.Concat(keywordModels).Concat(userKeywordModels).Concat(mathModels).Concat(commandModels);
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
                else
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

        //private Task<IEnumerable<SearchResultModel>> GetKeywordModels(string input)
        //{
        //    if (Properties.Settings.Default.IncludeDefaultKeywords)
        //    {
        //        (string Left, string Separator, string Right) partition = input.Partition(" ");

        //        List<string> attributes = searchDoc.GetNodesAttributes(Constants.NamespacesXpath);
        //        string format = @"((?<!\w){0}.*)";
        //        Regex rx = new(string.Format(format, partition.Left.Replace("[", "\\[")), RegexOptions.IgnoreCase);
        //        IEnumerable<string> matches = attributes.Where(x => rx.IsMatch(x))
        //                                                .Distinct();

        //        IEnumerable<SearchResultModel> keywordModels = Array.Empty<SearchResultModel>();
        //        foreach (string match in matches)
        //        {
        //            if (partition.Separator != string.Empty)
        //            {
        //                if (partition.Left != match)
        //                    continue;
        //            }
        //            keywordModels = keywordModels.Concat(SearchResultModel.MakeList(searchDoc, partition.Right, match, Category.Keyword));
        //        }
        //        return Task.FromResult(keywordModels);
        //    }
        //    return Task.FromResult(Enumerable.Empty<SearchResultModel>());
        //}

        //private Task<IEnumerable<SearchResultModel>> GetUserKeywordModels(string input)
        //{
        //    (string Left, string Separator, string Right) partition = input.Partition(" ");

        //    List<string> attributes = userSearchDoc.GetNodesAttributes(Constants.NamespacesXpath);
        //    string format = @"((?<!\w){0}.*)";
        //    Regex rx = new(string.Format(format, partition.Left.Replace("[", "\\[")), RegexOptions.IgnoreCase);
        //    IEnumerable<string> matches = attributes.Where(x => rx.IsMatch(x))
        //                                            .Distinct();

        //    IEnumerable<SearchResultModel> userKeywordModels = Array.Empty<SearchResultModel>();
        //    foreach (string match in matches)
        //    {
        //        if (partition.Separator != string.Empty)
        //        {
        //            if (partition.Left != match)
        //                continue;
        //        }
        //        userKeywordModels = userKeywordModels.Concat(SearchResultModel.MakeList(userSearchDoc, partition.Right, match, Category.Keyword));
        //    }
        //    return Task.FromResult(userKeywordModels);
        //}

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

        private async Task<IEnumerable<SearchResultModel>> GetCommandModelsAsync(XmlDocument doc, string input)
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

        //private async Task<SearchResultModel> GetCommandModelsAsync(string input)
        //{
        //    (string keyword, _, string statement) = input.Partition(" ");

        //    XmlNode node = commandsDoc.GetNode(keyword);
        //    if (node is not null)
        //    {
        //        Command command = (Command)Enum.Parse(typeof(Command), keyword.Capitalize());
        //        switch (command)
        //        {
        //            case Command.Timer:
        //                SearchResultModel model = new();
        //                int minSplit = int.Parse(node["MinSplit"].InnerText);
        //                string[] substrings = statement.Split(' ', minSplit);
        //                if (substrings.Length > 1)
        //                {
        //                    string time = substrings[0];
        //                    string remainder = substrings[^1];
        //                    if (double.TryParse(time, out double result))
        //                    {
        //                        string firstWord = remainder.FirstWord(out string rest);
        //                        double? seconds = await TimeUtils.GetTimeAsSecondsAsync(firstWord, result);
        //                        string unit;
        //                        string text;
        //                        if (seconds is null)
        //                        {
        //                            seconds = result;
        //                            unit = TimeUtils.GetTimeUnit("s", result);
        //                            text = remainder;
        //                        }
        //                        else
        //                        {
        //                            unit = TimeUtils.GetTimeUnit(firstWord, result);
        //                            text = rest;
        //                        }
        //                        string description = string.Format(node["Format"].InnerText, time, unit, text);
        //                        model = new(node, description, Category.Command);
        //                        return model;
        //                    }
        //                }
        //                else if (substrings.Length == 1)
        //                {
        //                    string time = substrings[0];
        //                    if (double.TryParse(time, out double seconds))
        //                    {
        //                        string unit = TimeUtils.GetTimeUnit("s", seconds);
        //                        string description = string.Format(node["Format"].InnerText, time, unit, node["DefaultText"].InnerText);
        //                        model = new(node, description, Category.Command);
        //                        return model;
        //                    }
        //                }

        //                break;

        //            default:
        //                break;
        //        }
        //    }
        //    return null;
        //}

        public void UserInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (SearchResults.Any())
            {
                switch (e.Key)
                {
                    case Key.Enter:
                        try
                        {
                            _ = HandleSelectedSearchResultBasedOnCategoryNameAsync(SelectedSearchResult.Category);
                        }
                        catch (NullReferenceException) { }
                        e.Handled = true;
                        TryCloseAsync();
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
                    //TryCloseAsync();
                    break;

                case Category.Math:
                    Clipboard.SetText(SelectedSearchResult.Text);
                    //TryCloseAsync();
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
                        await Task.Delay((int)SelectedSearchResult.Time * 1000);
                        ToastNotifications.SendSimpleToastNotification(SelectedSearchResult.Name, SelectedSearchResult.Text);
                    }
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