using Caliburn.Micro;
using Reginald.Commanding;
using Reginald.Core.Enums;
using Reginald.Core.IO;
using Reginald.Core.InputInjection;
using System.Windows;
using System.Windows.Input;
using Reginald.Core.Extensions;

namespace Reginald.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {
        public static SearchViewModel SearchView { get; set; } = new();

        public ICommand OpenWindowCommand { get; set; }

        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                NotifyOfPropertyChange(() => IsEnabled);
            }
        }

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
