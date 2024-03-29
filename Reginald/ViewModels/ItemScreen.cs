﻿namespace Reginald.ViewModels
{
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using Caliburn.Micro;
    using Reginald.Core.Services;
    using Reginald.Messages;
    using Reginald.Services;

    internal abstract class ItemScreen : Screen
    {
        private readonly string _pageName;

        public ItemScreen(string pageName)
        {
            _pageName = pageName;
        }

        public void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessService.GoTo((sender as Button).Tag.ToString());
        }

        public void Include_Click(object sender, RoutedEventArgs e)
        {
            IoC.Get<DataModelService>().Settings.Save();
        }

        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            IEventAggregator eventAggregator = IoC.Get<IEventAggregator>();
            _ = eventAggregator.PublishOnUIThreadAsync(new UpdatePageMessage(_pageName), cancellationToken);
            return base.OnActivateAsync(cancellationToken);
        }
    }
}
