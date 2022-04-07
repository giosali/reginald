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
    using Reginald.Data.Keywords;
    using Reginald.Messages;

    public class KeywordsViewModel : ItemsViewModelConductor<KeywordDataModel>, IHandle<UpdateItemsMessage>
    {
        private readonly IEventAggregator _eventAggregator;

        public KeywordsViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public Task HandleAsync(UpdateItemsMessage message, CancellationToken cancellationToken)
        {
            Filename = message.Filename;
            IsResource = message.IsResource;
            UpdateItems();
            return Task.CompletedTask;
        }

        public async void ListBoxItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Type type = (e.Source as ListBoxItem).Tag switch
            {
                "DefaultKeywords" => typeof(DefaultKeywordViewModel),
                "UserKeywords" => typeof(UserKeywordViewModel),
                "CommandKeywords" => typeof(CommandKeywordViewModel),
                "HttpKeywords" => typeof(HttpKeywordViewModel),
                _ => null,
            };
            object instance = IoC.GetInstance(type, null);
            if (instance is IScreen screen && !screen.IsActive)
            {
                await ActivateItemAsync(instance);
            }
        }

        public async void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ModificationType modificationType = (sender as MenuItem).Tag switch
            {
                "Edit" => ModificationType.Edit,
                "Delete" => ModificationType.Delete,
                _ => ModificationType.None,
            };
            await _eventAggregator.PublishOnUIThreadAsync(new ModifyItemMessage(SelectedItem.Guid, modificationType));
        }

        protected override void UpdateItems()
        {
            if (Items.Count > 0)
            {
                Items.Clear();
            }

            KeywordDataModel[] resourceModels = FileOperations.GetGenericData<KeywordDataModel>(Filename, true);
            KeywordDataModel[] localModels = FileOperations.GetGenericData<KeywordDataModel>(Filename, false);
            KeywordDataModel[] models;
            if (IsResource)
            {
                for (int i = 0; i < localModels.Length; i++)
                {
                    KeywordDataModel localModel = localModels[i];
                    for (int j = 0; j < resourceModels.Length; j++)
                    {
                        KeywordDataModel resourceModel = resourceModels[j];
                        if (resourceModel.Guid == localModel.Guid)
                        {
                            resourceModel.IsEnabled = localModel.IsEnabled;
                            break;
                        }
                    }
                }

                models = resourceModels;
            }
            else
            {
                models = localModels;
            }

            for (int i = 0; i < models.Length; i++)
            {
                KeywordDataModel model = models[i];
                if (string.IsNullOrEmpty(model.DisplayDescription))
                {
                    model.DisplayDescription = string.Format(model.Format, model.Placeholder);
                }

                if (model.Icon is null)
                {
                    model.Icon = model.PrimaryIcon;
                }
            }

            Items.AddRange(models);
        }

        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            _eventAggregator.SubscribeOnUIThread(this);

            object instance = IoC.GetInstance(typeof(DefaultKeywordViewModel), null);
            _ = ActivateItemAsync(instance, cancellationToken);
            return base.OnActivateAsync(cancellationToken);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            _eventAggregator.Unsubscribe(this);
            return base.OnDeactivateAsync(close, cancellationToken);
        }
    }
}
