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

        public SettingsViewModel()
        {
            IoC.Get<IEventAggregator>().SubscribeOnPublishedThread(this);
        }

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
            Type type = (e.Source as ListBoxItem).Tag switch
            {
                "General" => typeof(GeneralViewModel),
                "Themes" => typeof(ThemesViewModel),
                "Features" => typeof(FeaturesViewModel),
                "About" => typeof(AboutViewModel),
                _ => null,
            };
            if (IoC.GetInstance(type, null) is IScreen screen && !screen.IsActive)
            {
                await ActivateItemAsync(screen);
            }
        }

        public async void ListBox_Loaded(object sender, RoutedEventArgs e)
        {
            object instance = IoC.GetInstance(typeof(GeneralViewModel), null);
            await ActivateItemAsync(instance);
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
    }
}
