using Caliburn.Micro;
using Reginald.Commands;
using Reginald.Core.IO;
using Reginald.Core.InputInjection;
using System.IO;
using System.Windows;
using System.Windows.Input;
using static Reginald.Core.Enums.KeyboardHookEnums;

namespace Reginald.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {
        public ShellViewModel()
        {
            SetUpIO();
            OpenWindowCommand = new OpenWindowCommand(ExecuteMethod, CanExecuteMethod);

            KeyboardHook keyboardHook = new(Hook.Expansion);
            keyboardHook.Add();
        }

        private static void SetUpIO()
        {
            // Creates "Reginald" in %AppData%
            _ = Directory.CreateDirectory(Path.Combine(ApplicationPaths.AppDataDirectoryPath, ApplicationPaths.ApplicationName));

            // Creates "Reginald\UserIcons" in %AppData%
            string path = Path.Combine(ApplicationPaths.AppDataDirectoryPath, ApplicationPaths.ApplicationName, ApplicationPaths.UserIconsDirectoryName);
            _ = Directory.CreateDirectory(path);

            // Creates and updates "Reginald\Settings.xml" in %AppData%
            string settingsXml = FileOperations.GetSettingsXml();
            FileOperations.MakeAndUpdateSettingsXmlFile(settingsXml, ApplicationPaths.XmlSettingsFilename);

            // Creates and updates "Reginald\Search.xml" in %AppData%
            //FileOperations.MakeDefaultKeywordXmlFile();
            string defaultKeywordsXml = FileOperations.GetDefaultKeywordsXml();
            FileOperations.MakeXmlFile(defaultKeywordsXml, ApplicationPaths.XmlKeywordFilename);
            FileOperations.UpdateXmlFile(defaultKeywordsXml, ApplicationPaths.XmlKeywordFilename);

            // Creates and updates "Reginald\SpecialKeywords.xml" in %AppData%
            string specialKeywordsXml = FileOperations.GetSpecialKeywordsXml();
            FileOperations.MakeXmlFile(specialKeywordsXml, ApplicationPaths.XmlSpecialKeywordFilename);
            FileOperations.UpdateXmlFile(specialKeywordsXml, ApplicationPaths.XmlSpecialKeywordFilename);

            // Creates and updates "Reginald\Commands.xml" in %AppData%
            string commandsXml = FileOperations.GetCommandsXml();
            FileOperations.MakeXmlFile(commandsXml, ApplicationPaths.XmlCommandsFilename);
            FileOperations.UpdateXmlFile(commandsXml, ApplicationPaths.XmlCommandsFilename);

            // Creates and updates "Reginald\Utilities.xml" in %AppData%
            string utilitiesXml = FileOperations.GetUtilitiesXml();
            FileOperations.MakeXmlFile(utilitiesXml, ApplicationPaths.XmlUtilitiesFilename);
            FileOperations.UpdateXmlFile(utilitiesXml, ApplicationPaths.XmlUtilitiesFilename);

            // Creates "Reginald\ApplicationIcons", caches icons, and creates "Reginald\Applications.txt" in %AppData%
            FileOperations.CacheApplications();

            // Creates "Reginald\UserSearch.xml" in %AppData%
            FileOperations.MakeUserKeywordsXmlFile();

            // Creates "Reginald\Expansions.json" in %AppData%
            FileOperations.WriteFile(ApplicationPaths.ExpansionsJsonFilename);
        }

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

        public static SearchViewModel SearchViewModel { get; set; } = new();

        public ICommand OpenWindowCommand { get; set; }

        private bool CanExecuteMethod(object parameter)
        {
            return true;
        }

        private async void ExecuteMethod(object parameter)
        {
            if (IsEnabled)
            {
                IWindowManager manager = new WindowManager();
                if (!SearchViewModel.IsActive)
                {
                    await manager.ShowWindowAsync(SearchViewModel);
                }
                else
                {
                    SearchViewModel.UserInput = string.Empty;
                    SearchViewModel.ShowOrHide();
                }
            }
        }

        public void OpenSettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            IWindowManager manager = new WindowManager();
            _ = manager.ShowWindowAsync(new SettingsViewModel(SearchViewModel.StyleSearchView));
        }
    }
}
