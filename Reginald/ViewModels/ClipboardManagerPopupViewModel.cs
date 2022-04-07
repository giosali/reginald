namespace Reginald.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using Caliburn.Micro;
    using Reginald.Core.Extensions;
    using Reginald.Core.Helpers;
    using Reginald.Core.IO;
    using Reginald.Data.Comparers;
    using Reginald.Data.DisplayItems;
    using Reginald.Services;
    using Reginald.Services.Utilities;

    public class ClipboardManagerPopupViewModel : SearchPopupViewModelScreen<ClipboardItem>
    {
        private const string ClipboardFilename = "Clipboard.json";

        private const int ClipboardLimit = 25;

        private string _userInput;

        public ClipboardManagerPopupViewModel(ConfigurationService configurationService)
            : base(configurationService)
        {
            ClipboardUtility utility = ClipboardUtility.GetClipboardUtility();
            utility.ClipboardChanged += OnClipboardChanged;

            IEnumerable<ClipboardItemDataModel> models = FileOperations.GetGenericData<ClipboardItemDataModel>(ClipboardFilename, false);
            Items = new(models.Select(model => new ClipboardItem(model)));
            Items.CollectionChanged += OnCollectionChanged;
        }

        public override string UserInput
        {
            get => _userInput;
            set
            {
                _userInput = value;
                NotifyOfPropertyChange(() => UserInput);
                NotifyOfPropertyChange(() => DisplayItems);
                OnCollectionChanged(null, null);
            }
        }

        public BindableCollection<ClipboardItem> DisplayItems => new(Items.Where(item =>
        {
            if (!string.IsNullOrEmpty(UserInput))
            {
                return item.Description.Contains(UserInput, StringComparison.OrdinalIgnoreCase);
            }

            return true;
        }));

        public void UserInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    SelectedItem = DisplayItems[Math.Max(DisplayItems.IndexOf(SelectedItem) - 1, 0)];
                    IsMouseOverChanged = false;
                    break;

                case Key.Down:
                    SelectedItem = DisplayItems[Math.Min(DisplayItems.IndexOf(SelectedItem) + 1, DisplayItems.Count - 1)];
                    IsMouseOverChanged = false;
                    break;

                case Key.Enter:
                    SelectedItem.EnterKeyDown();
                    Hide();
                    break;
            }
        }

        public void UserInput_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            // Prevents premature input from appearing. For example, if the user
            // binds the clipboard manager to Control + Space and we didn't set e.Handled
            // to true, there will be a starting space character in the text each time
            // the user activates the clipboard manager by pressing those keys.
            e.Handled = true;
        }

        /// <summary>
        /// Drags the view.
        /// </summary>
        /// <param name="sender">The control that raised the event.</param>
        /// <param name="e">The event data.</param>
        public void Menu_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Services.Devices.Mouse.Drag();
        }

        public void PopupCloseBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Hide();
        }

        public void Item_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (SelectedItem is not null)
            {
                SelectedItem.EnterKeyDown();
                Hide();
            }
        }

        public void Items_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                SelectedItem = DisplayItems.Contains(LastSelectedItem) ? LastSelectedItem : DisplayItems[0];
            }
            catch (ArgumentOutOfRangeException)
            {
            }
        }

        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            // Resets index of selected item to 0 if there are any items
            if (DisplayItems?.Count > 0)
            {
                SelectedItem = DisplayItems[0];
            }

            return base.OnActivateAsync(cancellationToken);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            UserInput = string.Empty;
            return base.OnDeactivateAsync(close, cancellationToken);
        }

        private void OnClipboardChanged(object sender, EventArgs e)
        {
            if (ClipboardHelper.TryGetText(out string text))
            {
                Items.Insert(0, new ClipboardItem(text));
                if (Items.Count > ClipboardLimit)
                {
                    Items.RemoveAt(ClipboardLimit - 1);
                }

                Items = new(Items.Distinct(new ClipboardItemComparer()));
                FileOperations.WriteFile(ClipboardFilename, Items.Where(item => item.ClipboardItemType != ClipboardItemType.Image).Serialize());
            }
            else if (Clipboard.ContainsImage())
            {
                Items.Insert(0, new ClipboardItem(Clipboard.GetImage()));
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (DisplayItems.Count > 0)
            {
                SelectedItem = DisplayItems[0];
            }
        }
    }
}
