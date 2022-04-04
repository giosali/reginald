namespace Reginald.ViewModels
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Caliburn.Micro;
    using Reginald.Core.IO;
    using Reginald.Data.Keyphrases;
    using Reginald.Messages;
    using Reginald.Services;

    public class KeyphrasesViewModel : ItemsViewModelBase<KeyphraseDataModel>, IHandle<UpdateItemsMessage>
    {
        private readonly IEventAggregator _eventAggregator;

        public KeyphrasesViewModel(IEventAggregator eventAggregator, ConfigurationService configurationService)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeOnUIThread(this);

            ConfigurationService = configurationService;
        }

        public ConfigurationService ConfigurationService { get; set; }

        public Task HandleAsync(UpdateItemsMessage message, CancellationToken cancellationToken)
        {
            Filename = message.Filename;
            IsResource = message.IsResource;
            UpdateItems();
            return Task.CompletedTask;
        }

        public void IncludeSettingsPagesToggleButton_Click(object sender, RoutedEventArgs e) => ConfigurationService.Settings.Save();

        public async void ListBoxItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Type type = (e.Source as ListBoxItem).Tag switch
            {
                "UtilityKeyphrases" => typeof(UtilityKeyphraseViewModel),
                _ => null,
            };
            object instance = IoC.GetInstance(type, null);
            if (instance is IScreen screen && !screen.IsActive)
            {
                await ActivateItemAsync(instance);
            }
        }

        protected override void UpdateItems()
        {
            if (Items.Count > 0)
            {
                Items.Clear();
            }

            Items.AddRange(FileOperations.GetGenericData<KeyphraseDataModel>(Filename, IsResource));
        }

        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            object instance = IoC.GetInstance(typeof(UtilityKeyphraseViewModel), null);
            _ = ActivateItemAsync(instance, cancellationToken);
            return base.OnActivateAsync(cancellationToken);
        }
    }
}
