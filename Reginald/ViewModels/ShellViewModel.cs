using Caliburn.Micro;
using Hardcodet.Wpf.TaskbarNotification;
using Reginald.Commands;
using Reginald.Core.IO;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Reginald.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {
        public ShellViewModel()
        {
            SetUpApplication();
            tb = (TaskbarIcon)Application.Current.FindResource("ReginaldNotifyIcon");
            OpenWindowCommand = new OpenWindowCommand(ExecuteMethod, CanExecuteMethod);
        }

        // NotifyIcon for System Tray
        private TaskbarIcon tb;

        private SearchViewModel _searchViewModel = new();
        public SearchViewModel SearchViewModel
        {
            get => _searchViewModel;
            set => _searchViewModel = value;
        }

        public ICommand OpenWindowCommand { get; set; }

        private bool CanExecuteMethod(object parameter)
        {
            return true;
        }

        private async void ExecuteMethod(object parameter)
        {
            if (SearchViewModel.IsActive)
            {
                await SearchViewModel.TryCloseAsync();
                SearchViewModel = new SearchViewModel();
            }
            else
            {
                IWindowManager manager = new WindowManager();
                await manager.ShowWindowAsync(SearchViewModel);
            }
        }

        /// <summary>
        /// Creates files necessary for the program.
        /// </summary>
        private static async void SetUpApplication()
        {
            // Creates "Reginald" in %AppData%
            Directory.CreateDirectory(Path.Combine(ApplicationPaths.AppDataDirectoryPath, ApplicationPaths.ApplicationName));
            // Creates "Reginald\UserIcons" in %AppData%
            await Task.Run(() =>
            {
                string path = Path.Combine(ApplicationPaths.AppDataDirectoryPath, ApplicationPaths.ApplicationName, ApplicationPaths.UserIconsDirectoryName);
                Directory.CreateDirectory(path);
            });
            // Creates "Reginald\Search.xml" in %AppData%
            await Task.Run(() =>
            {
                FileOperations.MakeDefaultKeywordXmlFile();
            });
            await Task.Run(() =>
            {
                FileOperations.CacheApplicationIcons();
            });
            // Creates "Reginald\Applications.txt" in %AppData%
            await Task.Run(() =>
            {
                FileOperations.MakeApplicationsTextFile();
            });
            // Creates "Reginald\UserSearch.xml" in %AppData%
            await Task.Run(() =>
            {
                FileOperations.MakeUserKeywordsXmlFile();
            });
        }
    }
}
