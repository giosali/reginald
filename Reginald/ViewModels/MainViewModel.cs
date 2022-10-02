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
    using Reginald.Models.Inputs;
    using Reginald.Models.ObjectModels;
    using Reginald.Models.Producers;
    using Reginald.Models.Products;
    using Reginald.Services;

    internal sealed class MainViewModel : SearchPopupViewModelScreen<SearchResult>
    {
        private readonly ObjectModelService _oms;

        private CancellationTokenSource _cts = new();

        public MainViewModel()
        {
            _oms = IoC.Get<ObjectModelService>();
        }

        public void Item_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            PressEnter(sender);
        }

        public void Items_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Items.Count == 0)
            {
                return;
            }

            SelectedItem = Items.Contains(LastSelectedItem) ? LastSelectedItem : Items[0];
        }

        public void UserInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key is Key.System && !(e.Key is Key.LeftAlt || e.Key is Key.RightAlt) ? e.SystemKey : e.Key)
            {
                case Key.Tab:
                {
                    if (Items.Count == 0 || SelectedItem is not SearchResult selectedItem)
                    {
                        e.Handled = true;
                        break;
                    }

                    InputProcessingEventArgs args = new();
                    selectedItem.PressTab(args);
                    if (args.IsInputIncomplete && sender is TextBox textBox)
                    {
                        textBox.SetText(args.CompleteInput);
                    }

                    e.Handled = true;
                    break;
                }

                case Key.Enter when Keyboard.Modifiers is ModifierKeys.Alt:
                {
                    if (SelectedItem is not SearchResult selectedItem)
                    {
                        return;
                    }

                    InputProcessingEventArgs args = new();
                    selectedItem.PressAltAndEnter(args);
                    if (args.Handled)
                    {
                        Hide();
                    }

                    if (args.Remove)
                    {
                        int index = Items.IndexOf(selectedItem);
                        Items.Remove(selectedItem);
                        if (Items.Count > 0)
                        {
                            SelectedItem = Items[index - 1];
                        }
                    }

                    e.Handled = true;
                    break;
                }

                case Key.Enter:
                {
                    PressEnter(sender);
                    e.Handled = true;
                    break;
                }

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
                case Key.D1 when Keyboard.Modifiers is ModifierKeys.Control:
                case Key.D2 when Keyboard.Modifiers is ModifierKeys.Control:
                case Key.D3 when Keyboard.Modifiers is ModifierKeys.Control:
                case Key.D4 when Keyboard.Modifiers is ModifierKeys.Control:
                case Key.D5 when Keyboard.Modifiers is ModifierKeys.Control:
                case Key.D6 when Keyboard.Modifiers is ModifierKeys.Control:
                case Key.D7 when Keyboard.Modifiers is ModifierKeys.Control:
                case Key.D8 when Keyboard.Modifiers is ModifierKeys.Control:
                case Key.D9 when Keyboard.Modifiers is ModifierKeys.Control:
                {
                    int index = e.Key - Key.D1;
                    if (index >= Items.Count)
                    {
                        break;
                    }

                    PressEnter(sender, index);
                    break;
                }

                case Key.T when Keyboard.Modifiers is ModifierKeys.Control && !e.IsRepeat:
                    BorderOpacity = BorderOpacity == 1.0 ? 0.25 : 1.0;
                    break;
                case Key.LeftAlt when !e.IsRepeat:
                case Key.RightAlt when !e.IsRepeat:
                    SelectedItem?.PressAlt(new InputProcessingEventArgs());
                    e.Handled = true;
                    break;
            }
        }

        public void UserInput_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (Items.Count == 0)
            {
                return;
            }

            switch (e.SystemKey)
            {
                case Key.LeftAlt:
                case Key.RightAlt:
                    SelectedItem?.ReleaseAlt(new());
                    break;
            }
        }

        public async void UserInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            _cts.Cancel();
            _cts = new();
            IsMouseOverChanged = false;
            MousePosition = default;

            string userInput = UserInput;
            if (userInput.Length == 0)
            {
                // Removes ListBox flickering when it's cleared at this point.
                Items.Clear();
                return;
            }

            if (DMS.FileSystemEntrySearch.IsEnabled && userInput == " " && sender is TextBox textBox)
            {
                textBox.SetText(DMS.FileSystemEntrySearch.Key);
                return;
            }

            List<SearchResult> items = new();
            CancellationToken token = _cts.Token;
            if (DMS.FileSystemEntrySearch.Check(userInput))
            {
                List<SearchResult> tempItems = await Task.Run(
                    () =>
                {
                    try
                    {
                        return SearchFileSystemEntries(userInput, token);
                    }
                    catch (OperationCanceledException)
                    {
                        return null;
                    }
                },
                    token);
                if (tempItems is null)
                {
                    return;
                }

                items.AddRange(tempItems);
            }
            else
            {
                // Filters through applications.
                LinkedList<SearchResult> applications = new();
                LinkedListNode<SearchResult> separator = null;
                LinkedListNode<SearchResult> matchSeparator = null;
                ISingleProducer<SearchResult>[] omsSp = _oms.SingleProducers;
                for (int i = 0; i < omsSp.Length; i++)
                {
                   ISingleProducer<SearchResult> sp = omsSp[i];
                   if (!sp.Check(userInput))
                   {
                       continue;
                   }

                   SearchResult application = sp.Produce();
                   if (separator is null)
                   {
                       separator = applications.AddLast(application);
                       continue;
                   }

                   if (char.ToUpper(application.Description[0]) != char.ToUpper(userInput[0]))
                   {
                       applications.AddLast(application);
                       continue;
                   }

                   if (application.Description.IndexOf(userInput, StringComparison.OrdinalIgnoreCase) == -1)
                   {
                       _ = applications.AddBefore(separator, application);
                       continue;
                   }

                   matchSeparator = matchSeparator is null ? applications.AddFirst(application) : applications.AddAfter(matchSeparator, application);
                }

                items.AddRange(applications);
                items.AddRange(await Task.Run(
                    () =>
                {
                    try
                    {
                        List<SearchResult> results = new();
                        results.AddRange(SearchDataModelServiceProducers(userInput, token));
                        results.AddRange(SearchCpuIntensiveModels(userInput, token));
                        return results;
                    }
                    catch (OperationCanceledException)
                    {
                        return new List<SearchResult>();
                    }
                },
                    token));
            }

            // Removes ListBox flickering when it's cleared at this point.
            Items.Clear();
            Items.AddRange(items.Count == 0 ? DMS.DefaultWebQueries.Select(wq => wq.Produce(userInput)) : items.Take(DMS.Settings.SearchResultsLimit));
            if (Items.Count == 0)
            {
                return;
            }

            for (int i = 0; i < Math.Min(9, Items.Count); i++)
            {
                Items[i].KeyboardShortcut = "CTRL + " + (i + 1);
            }

            // Selects the previously selected item and places it at the top of the
            // results if it's still in the new list of results.
            int index = Items.IndexOf(LastSelectedItem);
            if (index > 0)
            {
                Items.Move(index, 0);
            }

            SelectedItem = Items[0];
        }

        private void PressEnter(object sender, int index = -1)
        {
            if ((index == -1 ? SelectedItem : Items[index]) is not SearchResult selectedItem)
            {
                return;
            }

            InputProcessingEventArgs args = new();
            selectedItem.PressEnter(args);
            if (args.IsInputIncomplete && sender is TextBox textBox)
            {
                textBox.SetText(args.CompleteInput);
            }

            if (args.Handled)
            {
                Hide();
            }
        }

        private List<SearchResult> SearchDataModelServiceProducers(string input, CancellationToken token)
        {
            List<SearchResult> results = new();
            ISingleProducer<SearchResult>[] singleProducers = DMS.SingleProducers;
            for (int i = 0; i < singleProducers.Length; i++)
            {
                token.ThrowIfCancellationRequested();
                ISingleProducer<SearchResult> sp = singleProducers[i];
                if (sp.Check(input))
                {
                    results.Add(sp.Produce());
                }
            }

            IMultipleProducer<SearchResult>[] multipleProducers = DMS.MultipleProducers;
            for (int i = 0; i < multipleProducers.Length; i++)
            {
                token.ThrowIfCancellationRequested();
                IMultipleProducer<SearchResult> mp = multipleProducers[i];
                if (mp.Check(input))
                {
                    results.AddRange(mp.Produce());
                }
            }

            return results;
        }

        private List<SearchResult> SearchCpuIntensiveModels(string input, CancellationToken token)
        {
            List<SearchResult> results = new();
            for (int i = 0; i < DMS.CpuIntensiveMultipleProducers.Length; i++)
            {
                IMultipleProducer<SearchResult> mp = DMS.CpuIntensiveMultipleProducers[i];
                if (mp.Check(input))
                {
                    results.AddRange(mp.Produce(token));
                }
            }

            return results;
        }

        private List<SearchResult> SearchFileSystemEntries(string input, CancellationToken token)
        {
            int limit = DMS.Settings.SearchResultsLimit;
            List<SearchResult> items = new();
            do
            {
                if (input.Length == DMS.FileSystemEntrySearch.Key.Length)
                {
                    items.Add(DMS.FileSystemEntrySearch.Produce());
                    break;
                }

                string fsQuery = input[1..];
                if (fsQuery.Length == 0)
                {
                    break;
                }

                int count = 0;
                foreach (FileSystemEntry entry in _oms.FileSystemEntries.Values)
                {
                    token.ThrowIfCancellationRequested();
                    if (count == limit)
                    {
                        break;
                    }

                    if (entry.Check(fsQuery))
                    {
                        items.Add(entry.Produce());
                        count++;
                    }
                }
            }
            while (false);

            token.ThrowIfCancellationRequested();
            return items;
        }
    }
}
