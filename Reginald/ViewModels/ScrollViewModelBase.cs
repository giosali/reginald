namespace Reginald.ViewModels
{
    using System.Windows.Controls;
    using System.Windows.Input;
    using Caliburn.Micro;

    public abstract class ScrollViewModelBase : Conductor<object>
    {
        public void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = sender as ScrollViewer;
            if (scv is not null)
            {
                scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            }

            e.Handled = true;
        }

        public void ScrollViewer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ScrollViewer scv = sender as ScrollViewer;
            if (scv is not null)
            {
                _ = scv.Focus();
            }
        }
    }
}
