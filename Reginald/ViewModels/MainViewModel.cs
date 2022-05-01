namespace Reginald.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Caliburn.Micro;
    using Reginald.Core.Extensions;
    using Reginald.Data.DisplayItems;
    using Reginald.Data.Keyphrases;
    using Reginald.Data.Keywords;
    using Reginald.Data.ShellItems;
    using Reginald.Services;
    using Reginald.Services.Utilities;

    public class MainViewModel : SearchPopupViewModelScreen<DisplayItem>
    {
        private readonly DataFileService _dataFileService;

        private readonly UserResourceService _userResourceService;

        public MainViewModel(ConfigurationService configurationService)
            : base(configurationService)
        {
            _dataFileService = IoC.Get<DataFileService>();
            _userResourceService = IoC.Get<UserResourceService>();
        }

        public async void UserInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            Items.Clear();
            IsMouseOverChanged = false;
            MousePosition = default;
            if (UserInput.Length > 0)
            {
                IEnumerable<DisplayItem> applicationResults = SearchResult.FromItems(await Application.FilterByNames(_userResourceService.Applications, ConfigurationService.Settings.IncludeInstalledApplications, UserInput));
                IEnumerable<DisplayItem> applicationResultsUppercase = SearchResult.FromItems(await Application.FilterByUppercaseCharacters(_userResourceService.Applications, ConfigurationService.Settings.IncludeInstalledApplications, UserInput));

                IEnumerable<DisplayItem> defaultKeywordResults = SearchResult.FromItems(await Keyword.Filter(_dataFileService.DefaultKeywords, ConfigurationService.Settings.IncludeDefaultKeywords, UserInput));
                IEnumerable<DisplayItem> userKeywordResults = SearchResult.FromItems(await Keyword.Filter(_dataFileService.UserKeywords, true, UserInput));
                IEnumerable<DisplayItem> commandResults = SearchResult.FromItems(await CommandKeyword.Process(_dataFileService.Commands, ConfigurationService.Settings.IncludeCommands, UserInput));
                IEnumerable<DisplayItem> httpKeywordResults = SearchResult.FromItems(await HttpKeyword.FilterAsync(_dataFileService.HttpKeywords, ConfigurationService.Settings.IncludeHttpKeywords, UserInput));
                IEnumerable<DisplayItem> defaultResults = SearchResult.FromItems(await Keyword.Set(_dataFileService.DefaultResults, UserInput));

                IEnumerable<DisplayItem> utilityResults = SearchResult.FromItems(await Keyphrase.Filter(_dataFileService.Utilities, ConfigurationService.Settings.IncludeUtilities, UserInput));
                IEnumerable<DisplayItem> microsoftSettingsResults = SearchResult.FromItems(await Keyphrase.Filter(_dataFileService.MicrosoftSettings, ConfigurationService.Settings.IncludeSettingsPages, UserInput));

                bool calculationSuccess = await _dataFileService.Calculator.IsExpression(UserInput);
                bool isLink = await _dataFileService.Link.IsLink(UserInput);

                IEnumerable<DisplayItem> results = applicationResults.Concat(applicationResultsUppercase)
                                                                     .Distinct()
                                                                     .Concat(defaultKeywordResults)
                                                                     .Concat(userKeywordResults)
                                                                     .Concat(commandResults)
                                                                     .Concat(httpKeywordResults)
                                                                     .Concat(utilityResults)
                                                                     .Concat(microsoftSettingsResults)
                                                                     .Concat(TimerResult.GetTimers(UserInput));

                if (results.Any())
                {
                    if (httpKeywordResults.Any())
                    {
                        // Clear default keyword results for late arriving HTTP keywords.
                        Items.Clear();
                    }

                    Items.AddRange(results);
                }

                if (calculationSuccess)
                {
                    Items.Add(new SearchResult(_dataFileService.Calculator));
                }

                if (isLink)
                {
                    Items.Add(new SearchResult(_dataFileService.Link));
                }

                if (Items.Count == 0)
                {
                    Items.AddRange(defaultResults);
                }

                // Selects the previously selected item and places it at the top of the
                // results if it's still in the new list of results.
                int index = Items.IndexOf(LastSelectedItem);
                if (index > 0)
                {
                    Items.PrependFrom(index);
                }

                SelectedItem = Items[0];
            }
        }

        public void UserInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Items.Count > 0)
            {
                switch (e.Key is Key.System && !(e.Key is Key.LeftAlt || e.Key is Key.RightAlt) ? e.SystemKey : e.Key)
                {
                    case Key.Up:
                        SelectedItem = Items[Math.Max(Items.IndexOf(SelectedItem) - 1, 0)];

                        // Prevents ListBoxItem from not getting selected after switching the
                        // selected item through arrow keys and moving mouse over it.
                        IsMouseOverChanged = false;
                        break;

                    case Key.Down:
                        SelectedItem = Items[Math.Min(Items.IndexOf(SelectedItem) + 1, Items.Count - 1)];

                        // Prevents ListBoxItem from not getting selected after switching the
                        // selected item through arrow keys and moving mouse over it.
                        IsMouseOverChanged = false;
                        break;

                    case Key.Enter when Keyboard.Modifiers is ModifierKeys.Alt:
                        OnSelectedItemEnterDown();
                        e.Handled = true;
                        break;

                    case Key.Enter:
                        OnSelectedItemEnterDown();
                        e.Handled = true;
                        break;

                    case Key.LeftAlt when !e.IsRepeat:
                    case Key.RightAlt when !e.IsRepeat:
                        SelectedItem?.AltKeyDown();
                        e.Handled = true;
                        break;

                    case Key.Tab:
                        // If the currently selected search result is actually
                        // derived from a keyword...
                        if (SelectedItem is SearchResult result && result.Keyword is not null)
                        {
                            // then get see if the user has already typed the
                            // corresponding keyword. If they haven't, autocomplete the
                            // textbox with the keyword.
                            if (!UserInput.StartsWith(result.Keyword, StringComparison.InvariantCultureIgnoreCase))
                            {
                                (sender as TextBox).SetText(result.Keyword + " ");
                            }
                        }

                        // Otherwise, there is no keyword and we should simply
                        // autocomplete with the name of the object.
                        else
                        {
                            if (!UserInput.StartsWith(SelectedItem.Name, StringComparison.InvariantCultureIgnoreCase))
                            {
                                (sender as TextBox).SetText(SelectedItem.Name);
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
                    case Key.Tab:
                        e.Handled = true;
                        break;
                }
            }
        }

        public void UserInput_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            // Prevents the keyboard input from doubling per keystroke.
            e.Handled = true;

            if (Items.Count > 0)
            {
                switch (e.Key)
                {
                    case Key.LeftAlt:
                    case Key.RightAlt:
                        SelectedItem?.AltKeyUp();
                        break;
                }
            }
        }

        public void Item_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OnSelectedItemEnterDown();
        }

        public void Items_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                SelectedItem = Items.Contains(LastSelectedItem) ? LastSelectedItem : Items[0];
            }
            catch (ArgumentOutOfRangeException)
            {
            }
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            UserInput = string.Empty;
            return base.OnDeactivateAsync(close, cancellationToken);
        }

        private void OnSelectedItemEnterDown()
        {
            DisplayItem selectedDisplayItem = SelectedItem;
            if (selectedDisplayItem is not null)
            {
                // Prevents the popup from closing by checking the selected item's
                // 'CanReceiveKeyboardInput' property. If the property is false,
                // then that means pressing Enter does nothing and the popup should stay visible.
                if (!selectedDisplayItem.RequiresPrompt && selectedDisplayItem.CanReceiveKeyboardInput)
                {
                    Hide();
                }

                selectedDisplayItem.EnterKeyDown();

                // Ensures browser doesn't lose focus
                if (selectedDisplayItem.LosesFocus)
                {
                    WindowUtility.SetTopWindow();
                }
            }
        }
    }
}
