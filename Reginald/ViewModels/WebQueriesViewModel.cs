namespace Reginald.ViewModels
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using Reginald.Models.DataModels;
    using Reginald.Services;

    internal sealed class WebQueriesViewModel : ItemsScreen<WebQuery>
    {
        public WebQueriesViewModel(DataModelService dms)
            : base("Features > Web Queries")
        {
            DMS = dms;
        }

        public DataModelService DMS { get; set; }

        public override void IsEnabled_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem.IsEnabled)
            {
                _ = DMS.Settings.DisabledWebQueries.Remove(SelectedItem.Id);
            }
            else
            {
                DMS.Settings.DisabledWebQueries.Add(SelectedItem.Id);
            }

            DMS.Settings.Save();
        }

        public void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem menuItem || menuItem.Tag is not string tag)
            {
                return;
            }

            switch (tag)
            {
                case "1":
                case "2":
                case "3":
                    if (!int.TryParse(tag, out int index))
                    {
                        break;
                    }

                    DMS.Settings.DefaultWebQueries[index - 1] = SelectedItem.Id;
                    DMS.Settings.Save();
                    break;
            }
        }

        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            Items.AddRange(DMS.SingleProducers
                              .Where(sp => sp is WebQuery wq && !wq.IsCustom)
                              .Select(sp => sp as WebQuery));
            return base.OnActivateAsync(cancellationToken);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            Items.Clear();
            return base.OnDeactivateAsync(close, cancellationToken);
        }
    }
}
