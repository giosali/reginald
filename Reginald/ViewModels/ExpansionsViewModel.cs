namespace Reginald.ViewModels
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Reginald.Core.Extensions;
    using Reginald.Core.IO;
    using Reginald.Models.DataModels;
    using Reginald.Services;

    internal sealed class ExpansionsViewModel : ItemsScreen<TextExpansion>
    {
        private bool _isBeingEdited;

        private string _replacement;

        private string _trigger;

        public ExpansionsViewModel(DataModelService dms)
            : base("Features > Expansions")
        {
            DMS = dms;
        }

        public DataModelService DMS { get; set; }

        public string Replacement
        {
            get => _replacement;
            set
            {
                _replacement = value;
                NotifyOfPropertyChange(() => Replacement);
            }
        }

        public string Trigger
        {
            get => _trigger;
            set
            {
                _trigger = value;
                NotifyOfPropertyChange(() => Trigger);
            }
        }

        public void DataGridTemplateColumnTextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            _isBeingEdited = true;
        }

        public void DataGrid_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!_isBeingEdited)
            {
                return;
            }

            FileOperations.WriteFile(TextExpansion.FileName, Items.Serialize());
            _isBeingEdited = false;
        }

        public override void IsEnabled_Click(object sender, RoutedEventArgs e)
        {
            DMS.Settings.Save();
        }

        public void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as MenuItem).Tag)
            {
                case "Delete":
                    _ = Items.Remove(SelectedItem);
                    FileOperations.WriteFile(TextExpansion.FileName, Items.Serialize());
                    break;
            }
        }

        public void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Items.Add(new TextExpansion(Trigger, Replacement));
            Items.OrderBy(te => te.Trigger);
            FileOperations.WriteFile(TextExpansion.FileName, Items.Serialize());
            Trigger = Replacement = string.Empty;
        }

        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            Items.AddRange(DMS.TextExpansions.OrderBy(te => te.Trigger));
            return base.OnActivateAsync(cancellationToken);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            Items.Clear();
            return base.OnDeactivateAsync(close, cancellationToken);
        }
    }
}
