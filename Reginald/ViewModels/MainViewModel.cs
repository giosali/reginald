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
    using Reginald.Models.Products;
    using Reginald.Services;

    internal class MainViewModel : SearchPopupViewModelScreen<SearchResult>
    {
        private readonly ObjectModelService _oms;

        private CancellationTokenSource _cts = new();

        public MainViewModel()
        {
            _oms = IoC.Get<ObjectModelService>();
        }

        public void Items_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Items.Count == 0)
            {
                return;
            }

            SelectedItem = Items.Contains(LastSelectedItem) ? LastSelectedItem : Items[0];
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

            List<SearchResult> items = new();
            CancellationToken token = _cts.Token;
            if (DMS.FileSystemEntrySearch.Check(userInput))
            {
                List<SearchResult> __items = await Task.Run(() =>
                {
                    try
                    {
                        return SearchFileSystemEntries(userInput, token);
                    }
                    catch (OperationCanceledException)
                    {
                        return null;
                    }
                }, token);
                if (__items is null)
                {
                    return;
                }

                items.AddRange(__items);
            }
            else
            {
                items.AddRange(_oms.SingleProducers
                                   .Where(sp => sp.Check(userInput))
                                   .Select(sp => sp.Produce())
                                   .OrderBy(sp => !sp.Description.StartsWith(userInput, StringComparison.OrdinalIgnoreCase))
                                   .ThenBy(sp => sp.Description));
                items.AddRange(DMS.SingleProducers
                                  .Where(sp => sp.Check(userInput))
                                  .Select(sp => sp.Produce()));
                items.AddRange(DMS.MultipleProducers
                                  .Where(mp => mp.Check(userInput))
                                  .SelectMany(mp => mp.Produce()));
                items.AddRange(await Task.Run(() =>
                {
                    try
                    {
                        return SearchCpuIntensiveModels(userInput, token);
                    }
                    catch (OperationCanceledException)
                    {
                        return Array.Empty<SearchResult>();
                    }
                }, token));
            }

            // Removes ListBox flickering when it's cleared at this point.
            Items.Clear();
            Items.AddRange(items.Count == 0 ? DMS.DefaultWebQueries.Select(wq => wq.Produce(userInput)) : items.Take(20));
            for (int i = 0; i < Math.Min(6, Items.Count); i++)
            {
                Items[i].KeyboardShortcut = "CTRL + " + (i + 1);
            }

            int index = Items.IndexOf(LastSelectedItem);
            if (index == -1)
            {
                SelectedItem = Items[0];
                return;
            }

            // Selects the previously selected item and places it at the top of the
            // results if it's still in the new list of results.
            SearchResult item = Items[index];
            for (int i = index; i > 0; i--)
            {
                Items[i] = Items[i - 1];
            }

            Items[0] = SelectedItem = item;
        }

        public void UserInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Prevents items from remaining in ListBox when TextChanged
            // event can't keep up while user holds down key.
            if (UserInput.Length == 0 && Items.Count != 0)
            {
                Items.Clear();
            }

            switch (e.Key is Key.System && !(e.Key is Key.LeftAlt || e.Key is Key.RightAlt) ? e.SystemKey : e.Key)
            {
                case Key.Tab:
                {
                    if (Items.Count == 0)
                    {
                        e.Handled = true;
                        break;
                    }

                    InputProcessingEventArgs args = new();
                    SelectedItem?.PressTab(args);
                    if (args.IsInputIncomplete)
                    {
                        (sender as TextBox)?.SetText(args.CompleteInput);
                    }

                    e.Handled = true;
                    break;
                }

                case Key.Enter when Keyboard.Modifiers is ModifierKeys.Alt:
                {
                    InputProcessingEventArgs args = new();
                    SelectedItem?.PressAltAndEnter(args);
                    if (args.Handled)
                    {
                        Hide();
                    }

                    if (args.Remove)
                    {
                        int index = Items.IndexOf(SelectedItem);
                        Items.Remove(SelectedItem);
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
                    InputProcessingEventArgs args = new();
                    SelectedItem?.PressEnter(args);
                    if (args.IsInputIncomplete)
                    {
                        (sender as TextBox).SetText(args.CompleteInput);
                    }

                    if (args.Handled)
                    {
                        Hide();
                    }

                    e.Handled = true;
                    break;
                }

                case Key.Space:
                    if (UserInput.Length == 0)
                    {
                        (sender as TextBox)?.SetText(DMS.FileSystemEntrySearch.Key);
                        e.Handled = true;
                    }

                    break;

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
            // Prevents the keyboard input from doubling per keystroke.
            e.Handled = true;

            if (Items.Count == 0)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.LeftAlt:
                case Key.RightAlt:
                    SelectedItem?.ReleaseAlt(new InputProcessingEventArgs());
                    break;
            }
        }

        private SearchResult[] SearchCpuIntensiveModels(string input, CancellationToken token)
        {
            if (!DMS.Quit.Check(input))
            {
                return Array.Empty<SearchResult>();
            }

            token.ThrowIfCancellationRequested();
            SearchResult[] items = DMS.Quit.Produce();
            token.ThrowIfCancellationRequested();
            return items;
        }

        private List<SearchResult> SearchFileSystemEntries(string input, CancellationToken token)
        {
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
                    if (count == 10)
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
