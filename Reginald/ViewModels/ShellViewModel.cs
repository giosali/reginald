using Caliburn.Micro;
using Hardcodet.Wpf.TaskbarNotification;
using Reginald.Commands;
using Reginald.Core.IO;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Reginald.ViewModels
{
    public class Indicator
    {
        private bool _isDeactivated;
        public bool IsDeactivated
        {
            get => _isDeactivated;
            set
            {
                _isDeactivated = value;
                ShellViewModel.SearchViewModel.TryCloseAsync();
                ShellViewModel.SearchViewModel = new SearchViewModel(new Indicator());
            }
        }
    }

    public class ShellViewModel : Conductor<object>
    {
        public ShellViewModel()
        {
            tb = (TaskbarIcon)Application.Current.FindResource("ReginaldNotifyIcon");
            OpenWindowCommand = new OpenWindowCommand(ExecuteMethod, CanExecuteMethod);

            // Creates "Reginald" in %AppData%
            Directory.CreateDirectory(Path.Combine(ApplicationPaths.AppDataDirectoryPath, ApplicationPaths.ApplicationName));

            // Creates "Reginald\UserIcons" in %AppData%
            string path = Path.Combine(ApplicationPaths.AppDataDirectoryPath, ApplicationPaths.ApplicationName, ApplicationPaths.UserIconsDirectoryName);
            Directory.CreateDirectory(path);

            // Creates "Reginald\Search.xml" in %AppData%
            FileOperations.MakeDefaultKeywordXmlFile();

            FileOperations.CacheApplicationIcons();

            // Creates "Reginald\Applications.txt" in %AppData%
            FileOperations.MakeApplicationsTextFile();

            // Creates "Reginald\UserSearch.xml" in %AppData%
            FileOperations.MakeUserKeywordsXmlFile();

            SearchViewModel = new(new Indicator());
        }

        // NotifyIcon for System Tray
        private TaskbarIcon tb;

        private static SearchViewModel _searchViewModel;
        public static SearchViewModel SearchViewModel
        {
            get => _searchViewModel;
            set => _searchViewModel = value;
        }

        private static Indicator _indicator;
        public static Indicator Indicator
        {
            get => _indicator;
            set => _indicator = value;
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
            }
            else
            {
                IWindowManager manager = new WindowManager();
                await manager.ShowWindowAsync(SearchViewModel);
            }
        }
    }
}
