namespace Reginald.ViewModels
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Caliburn.Micro;
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
