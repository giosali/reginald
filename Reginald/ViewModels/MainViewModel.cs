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
    using Reginald.Data.Products;
    using Reginald.Services;

    public class MainViewModel : SearchPopupViewModelScreen<Data.Products.SearchResult>
    {
        private readonly ObjectModelService _objectModelService;

        public MainViewModel(ConfigurationService configurationService)
            : base(configurationService)
        {
            _objectModelService = IoC.Get<ObjectModelService>();
        }

        public void UserInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            Items.Clear();
            IsMouseOverChanged = false;
            MousePosition = default;

            string userInput = UserInput;
            if (userInput.Length == 0)
            {
                return;
            }

            List<SearchResult> items = new();
            items.AddRange(_objectModelService.SingleProducers
                                              .Where(sp => sp.Check(userInput))
                                              .Select(sp => sp.Produce())
                                              .OrderBy(sp => sp.Description.StartsWith(userInput[0]))
                                              .ThenBy(sp => sp.Description));
            items.AddRange(_objectModelService.MultipleProducers
                                              .Where(mp => mp.Check(userInput))
                                              .SelectMany(mp => mp.Produce()));
            Items.AddRange(items.Take(25));

            // Items.AddRange(_objectModelService.SingleProducers.Where(sp => sp.Check(UserInput)).Select(sp => sp.Produce()).OrderBy(sp => sp.Description).ThenBy(sp => sp.Description.StartsWith(UserInput[0])));
            // Items.AddRange(_objectModelService.MultipleProducers.Where(sp => sp.Check(UserInput)).SelectMany(sp => sp.Produce()).OrderBy(sp => sp.Description).ThenBy(sp => sp.Description.StartsWith(UserInput[0])));

            if (Items.Count > 0)
            {
                SelectedItem = Items[0];
            }

            //// Selects the previously selected item and places it at the top of the
            //// results if it's still in the new list of results.
            //int index = Items.IndexOf(LastSelectedItem);
            //if (index > 0)
            //{
            //    Items.PrependFrom(index);
            //}

            //SelectedItem = Items[0];
        }

        public void UserInput_PreviewKeyDown(object sender, KeyEventArgs e)
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
                {
                    Data.Inputs.InputProcessingEventArgs args = new();
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
                    Data.Inputs.InputProcessingEventArgs args = new();
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

                case Key.LeftAlt when !e.IsRepeat:
                case Key.RightAlt when !e.IsRepeat:
                    SelectedItem?.PressAlt(new Data.Inputs.InputProcessingEventArgs());
                    e.Handled = true;
                    break;

                case Key.Tab:
                {
                    if (Items.Count == 0)
                    {
                        e.Handled = true;
                        break;
                    }

                    Data.Inputs.InputProcessingEventArgs args = new();
                    SelectedItem?.PressTab(args);
                    if (args.IsInputIncomplete)
                    {
                        (sender as TextBox).SetText(args.CompleteInput);
                    }

                    e.Handled = true;
                    break;
                }
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
                    SelectedItem?.ReleaseAlt(new Data.Inputs.InputProcessingEventArgs());
                    break;
            }
        }

        public void Items_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Items.Count == 0)
            {
                return;
            }

            SelectedItem = Items.Contains(LastSelectedItem) ? LastSelectedItem : Items[0];
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            UserInput = string.Empty;
            return base.OnDeactivateAsync(close, cancellationToken);
        }
    }
}
