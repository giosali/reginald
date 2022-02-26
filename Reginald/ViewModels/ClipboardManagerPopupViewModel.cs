namespace Reginald.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Interop;
    using Caliburn.Micro;
    using HotkeyUtility;
    using Reginald.Core.Extensions;
    using Reginald.Core.Helpers;
    using Reginald.Core.IO;
    using Reginald.Data.Comparers;
    using Reginald.Data.DisplayItems;
    using Reginald.Services.Clipboard;
    using Reginald.Services.Hooks;
    using Reginald.Services.Input;

    public class ClipboardManagerPopupViewModel : DataViewModelBase
    {
        private const string ClipboardJsonFilename = "Clipboard.json";

        private const int ClipboardManagerLimit = 25;

        private string _userInput;

        private ClipboardItem _selectedItem;

        private bool _isMouseOverChanged;

        public ClipboardManagerPopupViewModel()
            : base(false)
        {
            ClipboardUtility utility = ClipboardUtility.GetClipboardUtility();
            utility.ClipboardChanged += OnClipboardChanged;

            string filePath = FileOperations.GetFilePath(ClipboardJsonFilename, false);
            IEnumerable<ClipboardItemDataModel> models = FileOperations.GetGenericData<ClipboardItemDataModel>(filePath);
            Items = new(models.Select(model => new ClipboardItem(model)));
            Items.CollectionChanged += OnCollectionChanged;
        }

        public string UserInput
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

        public ClipboardItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                LastSelectedItem = SelectedItem;
                _selectedItem = value;
                NotifyOfPropertyChange(() => SelectedItem);
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

        public BindableCollection<ClipboardItem> DisplayItems => new(Items.Where(item =>
        {
            if (!string.IsNullOrEmpty(UserInput))
            {
                return item.Description.Contains(UserInput, StringComparison.OrdinalIgnoreCase);
            }

            return true;
        }));

        private BindableCollection<ClipboardItem> Items { get; set; }

        private ClipboardItem LastSelectedItem { get; set; }

        private MouseHook MouseHook { get; set; }

        private KeyboardHook KeyboardHook { get; set; }

        private Point MousePosition { get; set; }

        private IntPtr ActiveHandle { get; set; }

        public void UserInput_Loaded(object sender, RoutedEventArgs e)
        {
            if (GetView() is Popup popup && PresentationSource.FromVisual(popup.Child) is HwndSource source)
            {
                // Brings popup to front without stealing focus from the foreground window
                _ = Services.Devices.Keyboard.SetFocus(ActiveHandle = source.Handle);
            }
        }

        public void UserInput_LayoutUpdated(object sender, EventArgs e)
        {
            // Sets focus on the main textbox since, for some reason, the textbox loses focus
            // some time between the textbox being loaded and the texbox's layout being updated
            Keyboard.Focus(sender as TextBox);
        }

        public void UserInput_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

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
                    SelectedItem.EnterDown(false, ShowOrHide);
                    break;
            }
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
            ShowOrHide();
        }

        public void Items_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SelectedItem.EnterDown(false, ShowOrHide);
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

        public void Items_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (sender as ListBox)?.ScrollIntoView(SelectedItem);
        }

        public void Item_MouseMove(object sender, MouseEventArgs e)
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
            if (GetView() is Popup popup)
            {
                popup.IsOpen = !popup.IsOpen;
            }
        }

        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            // Creates a new mouse and keyboard hook each time the window is activated
            MouseHook = new();
            MouseHook.Add();
            MouseHook.MouseClick += OnLeftMouseClick;

            KeyboardHook = new(true);
            KeyboardHook.Add();
            KeyboardHook.KeyPressed += OnKeyPressed;

            // Resets index of selected item to 0 if there are any items
            if (DisplayItems?.Count > 0)
            {
                SelectedItem = DisplayItems[0];
            }

            return base.OnActivateAsync(cancellationToken);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            MouseHook.Remove();
            KeyboardHook.Remove();
            UserInput = string.Empty;
            MousePosition = default;
            LastSelectedItem = null;
            return base.OnDeactivateAsync(close, cancellationToken);
        }

        private void OnClipboardChanged(object sender, EventArgs e)
        {
            if (ClipboardHelper.TryGetText(out string text))
            {
                Items.Insert(0, new ClipboardItem(text));
                if (Items.Count > ClipboardManagerLimit)
                {
                    Items.RemoveAt(ClipboardManagerLimit - 1);
                }

                Items = new(Items.Distinct(new ClipboardItemComparer()));
                FileOperations.WriteFile(ClipboardJsonFilename, Items.Where(item => item.ClipboardItemType != ClipboardItemType.Image).Serialize());
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

        private void OnLeftMouseClick(object sender, MouseClickEventArgs e)
        {
            if (e.ThreadProcessId != Environment.ProcessId)
            {
                ShowOrHide();
            }
        }

        private void OnKeyPressed(object sender, KeyPressedEventArgs e)
        {
            e.Key = KeyInterop.KeyFromVirtualKey(e.VirtualKeyCode);
            _ = KeyboardInputInjector.SendKey(ActiveHandle, e.VirtualKeyCode);
            ModifierKeys modifiers = Keyboard.Modifiers;
            if (modifiers != ModifierKeys.None)
            {
                HotkeyUtility utility = HotkeyUtility.GetHotkeyUtility();
                foreach (Hotkey hotkey in utility.GetHotkeys())
                {
                    if (e.Key == hotkey.Key && modifiers == hotkey.Modifiers)
                    {
                        e.IsHotkeyPressed = true;
                        break;
                    }
                }
            }
        }
    }
}
