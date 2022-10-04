namespace Reginald.ViewModels
{
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using Caliburn.Micro;

    internal abstract class PopupViewModelScreen<T> : Screen
    {
        private bool _isMouseOverChanged;

        private T _selectedItem;

        public bool IsMouseOverChanged
        {
            get => _isMouseOverChanged;
            set
            {
                _isMouseOverChanged = value;
                NotifyOfPropertyChange(() => IsMouseOverChanged);
            }
        }

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

        public BindableCollection<T> Items { get; set; } = new();

        protected T LastSelectedItem { get; set; }

        protected Point MousePosition { get; set; }

        public void Hide()
        {
            if (GetView() is Popup popup)
            {
                popup.IsOpen = false;
            }
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

        public void Items_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (sender as ListBox)?.ScrollIntoView(SelectedItem);
        }

        public void Items_Unloaded(object sender, RoutedEventArgs e)
        {
            BindingOperations.ClearBinding(sender as ListBox, Selector.SelectedItemProperty);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            MousePosition = default;
            LastSelectedItem = default;
            return base.OnDeactivateAsync(close, cancellationToken);
        }
    }
}
