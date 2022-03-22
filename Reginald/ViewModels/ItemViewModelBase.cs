namespace Reginald.ViewModels
{
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using Caliburn.Micro;
    using Reginald.Core.Extensions;
    using Reginald.Core.IO;
    using Reginald.Services;

    public class ItemViewModelBase<T> : ScrollViewModelBase
    {
        private T _selectedItem;

        public ItemViewModelBase(string filename)
        {
            Filename = filename;
        }

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

        public virtual void Include_Click(object sender, RoutedEventArgs e)
        {
            ConfigurationService configurationService = IoC.Get<ConfigurationService>();
            configurationService.Settings.Save();
        }

        public virtual void IsEnabled_Click(object sender, RoutedEventArgs e)
        {
            PropertyInfo pi = typeof(T).GetProperty("IsEnabled");
            FileOperations.WriteFile(Filename, Items.Where(k => (bool)pi.GetValue(k)).Serialize());
        }
    }
}
