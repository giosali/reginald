namespace Reginald.ViewModels
{
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Input;
    using Caliburn.Micro;
    using Reginald.Core.Extensions;
    using Reginald.Core.IO;
    using Reginald.Messages;

    public abstract class ItemsViewModelBase<T> : ScrollViewModelBase
    {
        private T _selectedItem;

        public BindableCollection<T> Items { get; set; } = new();

        public T SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                NotifyOfPropertyChange(() => SelectedItem);
            }
        }

        protected string Filename { get; set; }

        protected bool IsResource { get; set; }

        public void ListBox_PreviewKeyDown(object sender, KeyEventArgs e) => e.Handled = true;

        public virtual async void IsEnabled_Click(object sender, RoutedEventArgs e)
        {
            if (IsResource)
            {
                PropertyInfo pi = typeof(T).GetProperty("IsEnabled");
                FileOperations.WriteFile(Filename, Items.Where(k => !(bool)pi.GetValue(k)).Serialize());
            }
            else
            {
                PropertyInfo pi = typeof(T).GetProperty("Guid");
                string guid = pi.GetValue(SelectedItem).ToString();
                IEventAggregator eventAggregator = IoC.Get<IEventAggregator>();
                await eventAggregator.PublishOnUIThreadAsync(new ModifyItemMessage(guid, ModificationType.ToggleIsEnabled));
            }
        }

        public virtual void DataGrid_ContextMenuOpening(object sender, RoutedEventArgs e)
        {
            // Prevents ContextMenu from opening if the items primarily come from a resource file.
            if (IsResource)
            {
                e.Handled = true;
            }
        }

        protected abstract void UpdateItems();
    }
}
