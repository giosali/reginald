using Caliburn.Micro;
using Reginald.ViewModels;
using System.Windows;

namespace Reginald
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void OpenSettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            IWindowManager manager = new WindowManager();
            manager.ShowWindowAsync(new SettingsViewModel());
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Current.Shutdown();
        }
    }
}
