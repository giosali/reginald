using Caliburn.Micro;
using Reginald.Core.AbstractProducts;
using Reginald.Core.Extensions;
using Reginald.Core.Helpers;
using Reginald.Core.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Reginald.ViewModels
{
    public class SearchViewModel : DataViewModelBase
    {
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

        private BindableCollection<DisplayItem> _searchResults = new();
        public BindableCollection<DisplayItem> SearchResults
        {
            get => _searchResults;
            set
            {
                _searchResults = value;
                NotifyOfPropertyChange(() => SearchResults);
            }
        }

        private DisplayItem _selectedSearchResult;
        public DisplayItem SelectedSearchResult
        {
            get => _selectedSearchResult;
            set
            {
                LastSelectedSearchResult = SelectedSearchResult;
                _selectedSearchResult = value;
                NotifyOfPropertyChange(() => SelectedSearchResult);
            }
        }

        private DisplayItem _lastSelectedSearchResult;
        public DisplayItem LastSelectedSearchResult
        {
            get => _lastSelectedSearchResult;
            set
            {
                _lastSelectedSearchResult = value;
                NotifyOfPropertyChange(() => LastSelectedSearchResult);
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

        protected override Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            return base.OnInitializeAsync(cancellationToken);
        }

        public SearchViewModel() : base(true)
        {

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
                    LastSelectedSearchResult = null;
                }
                else
                {
                    window.Show();
                }
            }
        }

        public async Task UserInput_TextChangedAsync(object sender, TextChangedEventArgs e)
        {
            SearchResults.Clear();
            IsMouseOverChanged = false;
            MousePosition = new();
            if (UserInput != string.Empty)
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
                                                                           .Concat(microsoftSettingsResults);

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
                int index = SearchResults.IndexOf(LastSelectedSearchResult);
                if (index > 0)
                {
                    SearchResults.PrependFrom(index);
                }
                SelectedSearchResult = SearchResults[0];
            }
        }

        /// <summary>
        /// Disables activation of the application menu bar through the Alt key
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            try
            {
                if (SearchResults.Count > 0)
                {
                    switch (e.SystemKey)
                    {
                        // If the key isn't a system key (likely a modifier key)...
                        case Key.None:
                            switch (e.Key)
                            {
                                case Key.Enter:
                                    HandleSelectedDisplayItemOnEnterDownAsync(sender);
                                    e.Handled = true;
                                    break;

                                case Key.Up:
                                    SelectedSearchResult = SearchResults[SearchResults.IndexOf(SelectedSearchResult) - 1];
                                    break;

                                case Key.Down:
                                    SelectedSearchResult = SearchResults[SearchResults.IndexOf(SelectedSearchResult) + 1];
                                    break;

                                case Key.Tab:
                                    TextBox textBox = sender as TextBox;
                                    SearchResult result = SelectedSearchResult as SearchResult;
                                    // If the currently selected search result is actually
                                    // derived from a keyword...
                                    if (result.Keyword is not null)
                                    {
                                        // then get see if the user has already typed the
                                        // corresponding keyword. If they haven't, autocomplete the
                                        // textbox with the keyword
                                        if (!UserInput.StartsWith(result.Keyword.Word, StringComparison.InvariantCultureIgnoreCase))
                                        {
                                            UserInput = result.Keyword.Word;
                                            textBox.SelectionStart = UserInput.Length;
                                        }
                                    }
                                    // Otherwise, there is no keyword and we should simply
                                    // autocomplete with the name of the object
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
                            }
                            break;

                        case Key.LeftAlt:
                        case Key.RightAlt:
                            HandleSelectedDisplayItemOnAltDown();
                            e.Handled = true;
                            break;

                        // This is only active if a system key is pressed in conjunction
                        // with a non-system key
                        // For example: Alt + Enter will produce Key.Alt and then Key.Return
                        // rather than Key.Enter
                        case Key.Return:
                            HandleSelectedDisplayItemOnEnterDownAsync(sender);
                            e.Handled = true;
                            break;
                    }
                }
                else
                {
                    switch (e.Key)
                    {
                        // Prevent textbox from losing focus
                        case Key.Tab:
                            e.Handled = true;
                            break;
                    }
                }
            }
            catch (ArgumentOutOfRangeException) { }
        }

        public void UserInput_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (SearchResults.Count > 0)
            {
                switch (e.SystemKey)
                {
                    case Key.LeftAlt:
                    case Key.RightAlt:
                        HandleSelectedDisplayItemOnAltUp();
                        e.Handled = true;
                        break;
                }
            }
        }

        public void SearchResults_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            HandleSelectedDisplayItemOnEnterDownAsync(sender);
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

        private async void HandleSelectedDisplayItemOnEnterDownAsync(object sender)
        {
            SearchResult result = SelectedSearchResult as SearchResult;
            if (result.Keyword is not null)
            {
                result.Keyword.EnterDown(result.Keyword, result.IsAltDown, ShowOrHide);
            }
            else if (result.ShellItem is not null)
            {
                result.ShellItem.EnterDown(result.ShellItem, result.IsAltDown, ShowOrHide);
            }
            else if (result.Representation is not null)
            {
                result.Representation.EnterDown(result.Representation, result.IsAltDown, ShowOrHide, sender);
            }
            else if (result.Keyphrase is not null)
            {
                bool success = await result.Keyphrase.EnterDown(result.Keyphrase, result.IsAltDown, result.IsPrompted, ShowOrHide);
                if (!success)
                {
                    SelectedSearchResult = SearchResults.Spotlight(KeywordHelper.ToConfirmationResult(result.Keyphrase));
                }
            }
        }

        private void HandleSelectedDisplayItemOnAltDown()
        {
            SearchResult result = SelectedSearchResult as SearchResult;
            if (!result.IsAltDown)
            {
                result.IsAltDown = true;
                (string Description, string Caption) = result.Keyword?.AltDown(result.Keyword)
                                                    ?? result.ShellItem?.AltDown(result.ShellItem)
                                                    ?? result.Representation?.AltDown(result.Representation)
                                                    ?? (result.Description, result.Caption);
                result.Description = Description ?? result.Description;
                result.Caption = Caption ?? result.Caption;
            }
        }

        private void HandleSelectedDisplayItemOnAltUp()
        {
            SearchResult result = SelectedSearchResult as SearchResult;
            result.IsAltDown = false;
            (string Description, string Caption) = result.Keyword?.AltUp(result.Keyword)
                                                ?? result.ShellItem?.AltUp(result.ShellItem)
                                                ?? result.Representation?.AltUp(result.Representation)
                                                ?? (result.Description, result.Caption);
            result.Description = Description ?? result.Description;
            result.Caption = Caption ?? result.Caption;
        }
    }
}