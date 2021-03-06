namespace Reginald.ViewModels
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using Caliburn.Micro;
    using HotkeyUtility;
    using HotkeyUtility.Extensions;
    using Reginald.Services.Hooks;
    using Reginald.Services.Input;

    public class PopupViewModelScreen<T> : Screen
    {
        private T _selectedItem;

        private bool _isMouseOverChanged;

        public T SelectedItem
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

        public BindableCollection<T> Items { get; set; } = new();

        protected T LastSelectedItem { get; set; }

        protected Point MousePosition { get; set; }

        protected IntPtr ActiveHandle { get; set; }

        private MouseHook MouseHook { get; set; }

        private KeyboardHook KeyboardHook { get; set; }

        public void Items_Unloaded(object sender, RoutedEventArgs e)
        {
            BindingOperations.ClearBinding(sender as ListBox, Selector.SelectedItemProperty);
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

        public void Hide()
        {
            if (GetView() is Popup popup)
            {
                popup.IsOpen = false;
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
            return base.OnActivateAsync(cancellationToken);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            MouseHook.Remove();
            KeyboardHook.Remove();
            MousePosition = default;
            LastSelectedItem = default;
            return base.OnDeactivateAsync(close, cancellationToken);
        }

        private void OnLeftMouseClick(object sender, MouseClickEventArgs e)
        {
            if (e.Handle != ActiveHandle)
            {
                Hide();
            }
        }

        private void OnKeyPressed(object sender, KeyPressedEventArgs e)
        {
            ModifierKeys modifiers = Keyboard.Modifiers;
            if (modifiers != ModifierKeys.None)
            {
                HotkeyManager hotkeyManager = HotkeyManager.GetHotkeyManager();
                Hotkey hotkey = hotkeyManager.GetHotkeys().Find(e.Key, modifiers);
                if (hotkey is not null)
                {
                    e.IsHotkeyPressed = true;
                    return;
                }
            }

            if (e.IsDown)
            {
                _ = KeyboardInputInjector.SendKeyDown(ActiveHandle, e.VirtualKeyCode);
            }
            else
            {
                _ = KeyboardInputInjector.SendKeyUp(ActiveHandle, e.VirtualKeyCode);
            }
        }
    }
}
