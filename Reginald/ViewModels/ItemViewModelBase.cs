namespace Reginald.ViewModels
{
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using Caliburn.Micro;
    using Reginald.Messages;
    using Reginald.Services;

    public class ItemViewModelBase : Screen
    {
        private readonly string _pageName;

        public ItemViewModelBase(string filename, bool isResource, string pageName)
        {
            Filename = filename;
            IsResource = isResource;
            _pageName = pageName;
        }

        protected string Filename { get; set; }

        protected bool IsResource { get; set; }

        public virtual void Include_Click(object sender, RoutedEventArgs e)
        {
            ConfigurationService configurationService = IoC.Get<ConfigurationService>();
            configurationService.Settings.Save();
        }

        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            IEventAggregator eventAggregator = IoC.Get<IEventAggregator>();
            _ = eventAggregator.PublishOnUIThreadAsync(new UpdatePageMessage(_pageName), cancellationToken);
            _ = eventAggregator.PublishOnUIThreadAsync(new UpdateItemsMessage(Filename, IsResource), cancellationToken);
            return base.OnActivateAsync(cancellationToken);
        }
    }
}
