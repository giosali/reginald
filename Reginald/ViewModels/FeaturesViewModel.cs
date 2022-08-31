namespace Reginald.ViewModels
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Caliburn.Micro;

    internal class FeaturesViewModel : Conductor<object>
    {
        public async void ListBoxItem_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Type type = (e.Source as ListBoxItem).Tag switch
            {
                "WebQueries" => typeof(WebQueriesViewModel),
                "YourWebQueries" => typeof(YourWebQueriesViewModel),
                "Applications" => typeof(ApplicationsViewModel),
                "Calculator" => typeof(CalculatorViewModel),
                "Link" => typeof(LinkViewModel),
                "MicrosoftSettings" => typeof(MicrosoftSettingsViewModel),
                "Recycle" => typeof(RecycleViewModel),
                "Timer" => typeof(TimerViewModel),
                "Quit" => typeof(QuitViewModel),
                _ => null,
            };
            if (IoC.GetInstance(type, null) is IScreen screen && !screen.IsActive)
            {
                await ActivateItemAsync(screen);
            }
        }

        public void ScrollViewer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer)
            {
                _ = scrollViewer.Focus();
            }
        }

        public void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer)
            {
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
            }

            e.Handled = true;
        }

        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            _ = ActivateItemAsync(IoC.GetInstance(typeof(WebQueriesViewModel), null), cancellationToken);
            return base.OnActivateAsync(cancellationToken);
        }
    }
}
