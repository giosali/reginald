namespace Reginald.ViewModels
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using Caliburn.Micro;
    using Reginald.Core.Extensions;
    using Reginald.Core.IO;
    using Reginald.Data.DataModels;
    using Reginald.Services;

    internal class WebQueriesViewModel : ItemsScreen<WebQuery>
    {
        public WebQueriesViewModel(ConfigurationService configurationService)
            : base("Features > Web Queries")
        {
            ConfigurationService = configurationService;
        }

        public ConfigurationService ConfigurationService { get; set; }

        public override void IsEnabled_Click(object sender, RoutedEventArgs e)
        {
            FileOperations.WriteFile(WebQuery.FileName, Items.Where(wq => !wq.IsEnabled).Serialize());
        }

        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            DataModelService dataModelService = IoC.Get<DataModelService>();
            Items.AddRange(dataModelService.SingleProducers
                                           .Where(sp => sp is WebQuery wq && !wq.IsCustom)
                                           .Select(sp =>
                                           {
                                                WebQuery wq = sp as WebQuery;
                                                wq.Description = string.Format(wq.DescriptionFormat, wq.Placeholder);
                                                return wq;
                                            }));
            return base.OnActivateAsync(cancellationToken);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            Items.Clear();
            return base.OnDeactivateAsync(close, cancellationToken);
        }
    }
}
