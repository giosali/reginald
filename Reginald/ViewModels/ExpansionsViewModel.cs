namespace Reginald.ViewModels
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Caliburn.Micro;
    using Reginald.Core.Extensions;
    using Reginald.Core.IO;
    using Reginald.Data.Expansions;
    using Reginald.Messages;
    using Reginald.Services;

    public class ExpansionsViewModel : ItemsViewModelConductor<TextExpansion>
    {
        private string _trigger;

        private string _replacement;

        public ExpansionsViewModel(ConfigurationService configurationService)
        {
            Filename = TextExpansion.Filename;
            IsResource = false;
            UpdateItems();

            ConfigurationService = configurationService;
        }

        public ConfigurationService ConfigurationService { get; set; }

        public string Trigger
        {
            get => _trigger;
            set
            {
                _trigger = value;
                NotifyOfPropertyChange(() => Trigger);
            }
        }

        public string Replacement
        {
            get => _replacement;
            set
            {
                _replacement = value;
                NotifyOfPropertyChange(() => Replacement);
            }
        }

        protected bool IsBeingEdited { get; set; }

        public void ExpansionsToggleButton_Click(object sender, RoutedEventArgs e) => ConfigurationService.Settings.Save();

        public void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Items.Add(new TextExpansion(Trigger, Replacement));
            FileOperations.WriteFile(Filename, Items.Serialize());
            Trigger = Replacement = string.Empty;
        }

        public void DataGrid_LostFocus(object sender, RoutedEventArgs e)
        {
            if (IsBeingEdited)
            {
                FileOperations.WriteFile(Filename, Items.Serialize());
                IsBeingEdited = false;
            }
        }

        public void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as MenuItem).Tag)
            {
                case "Delete":
                    _ = Items.Remove(SelectedItem);
                    FileOperations.WriteFile(Filename, Items.Serialize());
                    UpdateItems();
                    break;
            }
        }

        public void DataGridTemplateColumnTextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) => IsBeingEdited = true;

        protected override void UpdateItems()
        {
            if (Items.Count > 0)
            {
                Items.Clear();
            }

            TextExpansion[] textExpansions = FileOperations.GetGenericData<TextExpansion>(Filename, false);
            Items.AddRange(textExpansions.OrderBy(e => e.Trigger));
        }

        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            IEventAggregator eventAggregator = IoC.Get<IEventAggregator>();
            _ = eventAggregator.PublishOnUIThreadAsync(new UpdatePageMessage("Expansions"), cancellationToken);
            return base.OnActivateAsync(cancellationToken);
        }
    }
}
