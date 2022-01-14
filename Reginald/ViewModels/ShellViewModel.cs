namespace Reginald.ViewModels
{
    using System.Windows;
    using System.Windows.Input;
    using Caliburn.Micro;
    using Reginald.Commanding;
    using Reginald.Core.Extensions;
    using Reginald.Core.InputInjection;
    using Reginald.Core.IO;

    public class ShellViewModel : Conductor<object>
    {
        private bool _isEnabled = true;

        public ShellViewModel()
        {
            FileOperations.SetUp();
            if (SearchView.Settings.LaunchOnStartup)
            {
                _ = FileOperations.TryCreateShortcut();
            }

            OpenWindowCommand = new OpenWindowCommand(ExecuteMethod, CanExecuteMethod);

            KeyboardHook keyboardHook = new(Hook.Expansion);
            keyboardHook.Add();
        }

        public static SearchViewModel SearchView { get; set; } = new();

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                NotifyOfPropertyChange(() => IsEnabled);
            }
        }

        public ICommand OpenWindowCommand { get; set; }

        public void OpenSettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            IWindowManager manager = new WindowManager();
            _ = manager.ShowWindowAsync(new SettingsViewModel());
        }

        public void LaunchOnStartupMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!FileOperations.TryCreateShortcut())
            {
                FileOperations.DeleteShortcut();
            }

            FileOperations.WriteFile(ApplicationPaths.SettingsFilename, SearchView.Settings.Serialize());
        }

        private bool CanExecuteMethod(object parameter)
        {
            return true;
        }

        private async void ExecuteMethod(object parameter)
        {
            if (IsEnabled)
            {
                IWindowManager manager = new WindowManager();
                if (!SearchView.IsActive)
                {
                    await manager.ShowWindowAsync(SearchView);
                }
                else
                {
                    SearchView.UserInput = string.Empty;
                    SearchView.ShowOrHide();
                }
            }
        }
    }
}
