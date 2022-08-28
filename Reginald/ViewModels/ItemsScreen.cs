﻿namespace Reginald.ViewModels
{
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using Caliburn.Micro;
    using Reginald.Messages;
    using Reginald.Services;

    internal abstract class ItemsScreen<T> : Screen
    {
        private readonly string _pageName;

        private T _selectedItem;

        public ItemsScreen(string pageName)
        {
            _pageName = pageName;
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

        public abstract void IsEnabled_Click(object sender, RoutedEventArgs e);

        public virtual void Include_Click(object sender, RoutedEventArgs e)
        {
            IoC.Get<ConfigurationService>().Settings.Save();
        }

        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            IEventAggregator eventAggregator = IoC.Get<IEventAggregator>();
            _ = eventAggregator.PublishOnUIThreadAsync(new UpdatePageMessage(_pageName), cancellationToken);
            return base.OnActivateAsync(cancellationToken);
        }
    }
}