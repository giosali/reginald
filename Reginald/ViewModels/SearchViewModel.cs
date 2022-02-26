namespace Reginald.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Caliburn.Micro;
    using Reginald.Core.Base;
    using Reginald.Core.Collections;
    using Reginald.Core.Extensions;
    using Reginald.Data.DisplayItems;
    using Reginald.Data.Keyphrases;
    using Reginald.Data.Keywords;
    using Reginald.Data.Representations;
    using Reginald.Data.ShellItems;

    public class SearchViewModel : DataViewModelBase
    {
        private string _userInput;

        private BindableCollection<DisplayItem> _searchResults = new();

        private DisplayItem _selectedDisplayItem;

        private bool _isMouseOverChanged;

        private bool _isBrowsingRecentSearches;

        public SearchViewModel()
            : base(true)
        {
        }

        public string UserInput
        {
            get => _userInput;
            set
            {
                _userInput = value;
                NotifyOfPropertyChange(() => UserInput);
            }
        }

        public BindableCollection<DisplayItem> SearchResults
        {
            get => _searchResults;
            set
            {
                _searchResults = value;
                NotifyOfPropertyChange(() => SearchResults);
            }
        }

        public DisplayItem SelectedDisplayItem
        {
            get => _selectedDisplayItem;
            set
            {
                LastSelectedDisplayItem = SelectedDisplayItem;
                _selectedDisplayItem = value;
                NotifyOfPropertyChange(() => SelectedDisplayItem);
            }
        }

        public bool IsMouseOverChanged
        {
            get => _isMouseOverChanged;
            set
            {
                _isMouseOverChanged = value;
                NotifyOfPropertyChange(() => IsMouseOverChanged);
            }
        }

        public bool IsBrowsingRecentSearches
        {
            get => _isBrowsingRecentSearches;
            set
            {
                _isBrowsingRecentSearches = value;
                RecentSearchesIndex = RecentSearches.Count;
                NotifyOfPropertyChange(() => IsBrowsingRecentSearches);
            }
        }

        private DisplayItem LastSelectedDisplayItem { get; set; }

        private Point MousePosition { get; set; }

        private List<DisplayItem> Timers { get; set; } = new();

        private Deque<string> RecentSearches { get; set; } = new(20);

        private int RecentSearchesIndex { get; set; } = -1;

        private int KeyUpTimestamp { get; set; }

        public async void UserInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchResults.Clear();
            IsMouseOverChanged = false;
            MousePosition = default;

            if (UserInput.Length > 0 && !IsBrowsingRecentSearches)
            {
                IEnumerable<DisplayItem> applicationResultsStrict = ShellItemHelper.ToSearchResults(await ShellItemHelper.FilterByStrictNames(Applications, Settings.IncludeInstalledApplications, UserInput));
                IEnumerable<DisplayItem> applicationResultsUppercase = ShellItemHelper.ToSearchResults(await ShellItemHelper.FilterByUppercaseCharacters(Applications, Settings.IncludeInstalledApplications, UserInput));
                IEnumerable<DisplayItem> defaultKeywordResults = KeywordHelper.ToSearchResults(await KeywordHelper.Filter(DefaultKeywords, Settings.IncludeDefaultKeywords, UserInput));
                IEnumerable<DisplayItem> userKeywordResults = KeywordHelper.ToSearchResults(await KeywordHelper.Filter(UserKeywords, true, UserInput));
                IEnumerable<DisplayItem> commandResults = KeywordHelper.ToSearchResults(await KeywordHelper.Process(Commands, Settings.IncludeCommands, UserInput, Applications));
                IEnumerable<DisplayItem> httpKeywordResults = KeywordHelper.ToSearchResults(await KeywordHelper.FilterAsync(HttpKeywords, Settings.IncludeHttpKeywords, UserInput));
                IEnumerable<DisplayItem> utilityResults = KeyphraseHelper.ToSearchResults(await KeyphraseHelper.Filter(Utilities, Settings.IncludeUtilities, UserInput));
                IEnumerable<DisplayItem> microsoftSettingsResults = KeyphraseHelper.ToSearchResults(await KeyphraseHelper.Filter(MicrosoftSettings, Settings.IncludeSettingsPages, UserInput));
                IEnumerable<DisplayItem> defaultResults = KeywordHelper.ToSearchResults(await KeywordHelper.Set(DefaultResults, UserInput));
                IEnumerable<DisplayItem> timers = await DisplayItemHelper.Filter(Timers, UserInput, Constants.TimersPreciseTerm);
                bool calculationSuccess = await (Calculator as Calculator).IsExpression(UserInput);
                bool isLink = (Link as Link).IsLink(UserInput);
                IEnumerable<DisplayItem> results = applicationResultsStrict.Concat(applicationResultsUppercase)
                                                                           .Distinct()
                                                                           .OrderBy(item => item.Name)
                                                                           .Concat(defaultKeywordResults)
                                                                           .Concat(userKeywordResults)
                                                                           .Concat(commandResults)
                                                                           .Concat(httpKeywordResults)
                                                                           .Concat(utilityResults)
                                                                           .Concat(microsoftSettingsResults)
                                                                           .Concat(timers);

                if (results.Any())
                {
                    if (httpKeywordResults.Any())
                    {
                        // Clear default keyword results for late arriving HTTP keywords
                        SearchResults.Clear();
                    }

                    SearchResults.AddRange(results);
                }

                if (calculationSuccess)
                {
                    DisplayItem calculation = RepresentationHelper.ToSearchResult(Calculator);
                    SearchResults.Add(calculation);
                }

                if (isLink)
                {
                    DisplayItem link = RepresentationHelper.ToSearchResult(Link);
                    SearchResults.Add(link);
                }

                if (SearchResults.Count == 0)
                {
                    SearchResults.AddRange(defaultResults);
                }

                // The result needs to be in the collection and can't be the first item
                int index = SearchResults.IndexOf(LastSelectedDisplayItem);
                if (index > 0)
                {
                    SearchResults.PrependFrom(index);
                }

                SelectedDisplayItem = SearchResults[0];
            }
        }

        /// <summary>
        /// Disables activation of the application menu bar through the Alt key.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The key event data.</param>
        public void SearchWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.SystemKey)
            {
                case Key.LeftAlt:
                case Key.RightAlt:
                    e.Handled = true;
                    break;
            }
        }

        public void UserInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (SearchResults.Count > 0)
            {
                switch (e.Key is Key.System && !(e.Key is Key.LeftAlt || e.Key is Key.RightAlt) ? e.SystemKey : e.Key)
                {
                    case Key.Up:
                        SelectedDisplayItem = SearchResults[Math.Max(SearchResults.IndexOf(SelectedDisplayItem) - 1, 0)];

                        // Prevents ListBoxItem from not getting selected after switching the
                        // selected item through arrow keys and moving mouse over it
                        IsMouseOverChanged = false;
                        break;

                    case Key.Down:
                        SelectedDisplayItem = SearchResults[Math.Min(SearchResults.IndexOf(SelectedDisplayItem) + 1, SearchResults.Count - 1)];

                        // Prevents ListBoxItem from not getting selected after switching the
                        // selected item through arrow keys and moving mouse over it
                        IsMouseOverChanged = false;
                        break;

                    case Key.Enter when Keyboard.Modifiers is ModifierKeys.Alt:
                        OnSelectedDisplayItemEnterDown(sender, true);
                        e.Handled = true;
                        break;

                    case Key.Enter:
                        OnSelectedDisplayItemEnterDown(sender, false);
                        e.Handled = true;
                        break;

                    case Key.LeftAlt when !e.IsRepeat:
                    case Key.RightAlt when !e.IsRepeat:
                        (string description, string caption) = SelectedDisplayItem.AltDown();
                        SelectedDisplayItem.Description = description ?? SelectedDisplayItem.Description;
                        SelectedDisplayItem.Caption = caption ?? SelectedDisplayItem.Caption;
                        e.Handled = true;
                        break;

                    case Key.Tab:
                        // If the currently selected search result is actually
                        // derived from a keyword...
                        if (SelectedDisplayItem is SearchResult result && result.Keyword is not null)
                        {
                            // then get see if the user has already typed the
                            // corresponding keyword. If they haven't, autocomplete the
                            // textbox with the keyword
                            if (!UserInput.StartsWith(result.Keyword.Word, StringComparison.InvariantCultureIgnoreCase))
                            {
                                (sender as TextBox).SetText(result.Keyword.Word + " ");
                            }
                        }

                        // Otherwise, there is no keyword and we should simply
                        // autocomplete with the name of the object
                        else
                        {
                            if (!UserInput.StartsWith(SelectedDisplayItem.Name, StringComparison.InvariantCultureIgnoreCase))
                            {
                                (sender as TextBox).SetText(SelectedDisplayItem.Name);
                            }
                        }

                        e.Handled = true;
                        break;
                }
            }
            else
            {
                switch (e.Key)
                {
                    case Key.Up:
                        if (IsBrowsingRecentSearches)
                        {
                            BrowseRecentSearches(true);
                        }

                        break;

                    case Key.Down:
                        if (IsBrowsingRecentSearches)
                        {
                            // Breaks user out of browsing recent searches
                            if (RecentSearchesIndex + 1 == RecentSearches.Count)
                            {
                                IsBrowsingRecentSearches = false;
                                UserInput = string.Empty;
                            }
                            else
                            {
                                BrowseRecentSearches(false);
                            }
                        }

                        break;

                    case Key.Enter:
                        // Breaks user out of browsing recent searches
                        if (IsBrowsingRecentSearches)
                        {
                            (sender as TextBox).SetText(RecentSearches[RecentSearchesIndex]);
                            IsBrowsingRecentSearches = false;
                        }

                        break;

                    case Key.Tab:
                        e.Handled = true;
                        break;

                    default:
                        // Breaks user out of browsing recent searches
                        if (IsBrowsingRecentSearches)
                        {
                            IsBrowsingRecentSearches = false;
                            UserInput = string.Empty;
                        }

                        break;
                }
            }
        }

        public void UserInput_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (SearchResults.Count > 0)
            {
                switch (e.SystemKey)
                {
                    case Key.LeftAlt:
                    case Key.RightAlt:
                        (string description, string caption) = SelectedDisplayItem.AltUp();
                        SelectedDisplayItem.Description = description ?? SelectedDisplayItem.Description;
                        SelectedDisplayItem.Caption = caption ?? SelectedDisplayItem.Caption;
                        e.Handled = true;
                        break;
                }
            }
            else
            {
                switch (e.Key)
                {
                    case Key.Up:
                        if (RecentSearches.Count > 0)
                        {
                            int value = Math.Abs(e.Timestamp - KeyUpTimestamp);
                            if (value < 250 && !IsBrowsingRecentSearches)
                            {
                                IsBrowsingRecentSearches = true;
                                BrowseRecentSearches(true);
                            }
                        }

                        KeyUpTimestamp = e.Timestamp;
                        break;
                }
            }
        }

        public void SearchResults_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OnSelectedDisplayItemEnterDown(sender, false);
        }

        public void SearchResults_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                SelectedDisplayItem = SearchResults.Contains(LastSelectedDisplayItem) ? LastSelectedDisplayItem : SearchResults[0];
            }
            catch (ArgumentOutOfRangeException)
            {
            }
        }

        public void SearchResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (sender as ListBox)?.ScrollIntoView(SelectedDisplayItem);
        }

        public void SearchResults_MouseMove(object sender, MouseEventArgs e)
        {
            Point position = e.GetPosition((IInputElement)sender);
            if (position != MousePosition && MousePosition != default)
            {
                IsMouseOverChanged = true;
            }

            MousePosition = position;
        }

        public void ShowOrHide()
        {
            if (GetView() is Window window)
            {
                if (window.IsVisible)
                {
                    window.Hide();
                    MousePosition = default;
                    LastSelectedDisplayItem = null;
                }
                else
                {
                    IsBrowsingRecentSearches = false;
                    window.Show();
                }
            }
        }

        private async void OnSelectedDisplayItemEnterDown(object sender, bool isAltDown)
        {
            bool success = await SelectedDisplayItem.EnterDownAsync(isAltDown, ShowOrHide, sender);
            SearchResult result = SelectedDisplayItem as SearchResult;
            if (!success)
            {
                // Remove the selected timer from Timers
                if (SelectedDisplayItem is TimerResult)
                {
                    _ = Timers.Remove(SelectedDisplayItem);
                    UserInput_TextChanged(null, null);
                }
                else
                {
                    SelectedDisplayItem = SearchResults.Spotlight(KeywordHelper.ToConfirmationResult(result.Keyphrase));
                }
            }

            if (result.Keyword is TimerKeyword)
            {
                // Add a timer to Timers
                DisplayItem item = KeywordHelper.ToDisplayItem(result.Keyword);
                if (item is not null)
                {
                    Timers.Add(item);
                }
            }

            // Adds recent search query
            RecentSearches.Append(UserInput);
        }

        private void BrowseRecentSearches(bool isUpKey)
        {
            RecentSearchesIndex = isUpKey
                                ? Math.Max(RecentSearchesIndex - 1, 0)
                                : Math.Min(RecentSearchesIndex + 1, RecentSearches.Count - 1);
            UserInput = RecentSearches[RecentSearchesIndex];
        }
    }
}
