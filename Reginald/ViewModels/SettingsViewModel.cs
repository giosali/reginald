namespace Reginald.ViewModels
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Interop;
    using Caliburn.Micro;
    using Reginald.Messages;
    using Reginald.Visual;

    internal sealed class SettingsViewModel : Conductor<object>, IHandle<UpdatePageMessage>
    {
        private string _title;

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                NotifyOfPropertyChange(() => Title);
            }
        }

        public Task HandleAsync(UpdatePageMessage message, CancellationToken cancellationToken)
        {
            Title = message.Title;
            return Task.CompletedTask;
        }

        public async void ListBoxItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is not ListBoxItem listBoxItem || listBoxItem.Tag is not string tag)
            {
                return;
            }

            object obj = tag switch
            {
                "General" => new GeneralViewModel(),
                "Themes" => new ThemesViewModel(),
                "Features" => new FeaturesViewModel(),
                "About" => new AboutViewModel(),
                _ => null,
            };
            if (obj is IScreen screen && !screen.IsActive)
            {
                await ActivateItemAsync(screen);
            }
        }

        public async void ListBox_Loaded(object sender, RoutedEventArgs e)
        {
            await ActivateItemAsync(new GeneralViewModel());
        }

        public void ListBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (GetView() is Window window && PresentationSource.FromVisual(window) is HwndSource source)
            {
                DarkTitleBar.Enable(source.Handle);
            }
        }

        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            IoC.Get<IEventAggregator>().SubscribeOnPublishedThread(this);
            return base.OnActivateAsync(cancellationToken);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            IoC.Get<IEventAggregator>().Unsubscribe(this);
            return base.OnDeactivateAsync(close, cancellationToken);
        }
    }
}
