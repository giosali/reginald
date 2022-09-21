namespace Reginald.ViewModels
{
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using Caliburn.Micro;
    using Reginald.Core.Services;
    using Reginald.Messages;

    internal sealed class AboutViewModel : Screen
    {
        public void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessService.GoTo((sender as Button).Tag.ToString());
        }

        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            IEventAggregator eventAggregator = IoC.Get<IEventAggregator>();
            _ = eventAggregator.PublishOnUIThreadAsync(new UpdatePageMessage("About"), cancellationToken);
            return base.OnActivateAsync(cancellationToken);
        }
    }
}
