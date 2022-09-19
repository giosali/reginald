namespace Reginald.ViewModels
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
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
