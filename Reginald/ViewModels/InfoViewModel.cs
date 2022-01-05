using Reginald.Commanding;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;

namespace Reginald.ViewModels
{
    public class InfoViewModel
    {
        public ICommand HyperlinkCommand { get; set; }

        public InfoViewModel()
        {
            HyperlinkCommand = new HyperlinkCommand(ExecuteMethod, CanExecuteMethod);
        }

        public void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private bool CanExecuteMethod(object parameter)
        {
            return true;
        }

        private void ExecuteMethod(object parameter)
        {
            string uri = (string)parameter;
            ProcessStartInfo startInfo = new()
            {
                FileName = uri,
                UseShellExecute = true
            };
            Process.Start(startInfo);
        }
    }
}
