namespace Reginald.ViewModels
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Caliburn.Micro;
    using Reginald.Core.Helpers;
    using Reginald.Core.IO;
    using Reginald.Data.Base;
    using Reginald.Data.Keyphrases;
    using Reginald.Data.Keywords;
    using Reginald.Data.Representations;
    using Reginald.Data.Settings;
    using Reginald.Data.ShellItems;
    using Reginald.Data.Units;

    public class DataViewModelBase : Screen
    {
        private SettingsDataModel _settings;

        private Theme _theme;

        public DataViewModelBase(bool monitorChanges)
        {
            UpdateSettings();
            UpdateTheme();

            if (monitorChanges)
            {
                Applications = UpdateShellItems();

                DefaultKeywords = KeywordHelper.ToKeywords(UpdateData<GenericKeywordDataModel>(ApplicationPaths.KeywordsJsonFilename, true).Union(UpdateData<GenericKeywordDataModel>(ApplicationPaths.KeywordsJsonFilename, false)));
                UserKeywords = KeywordHelper.ToKeywords(UpdateData<GenericKeywordDataModel>(ApplicationPaths.UserKeywordsJsonFilename, false));
                DefaultResults = KeywordHelper.ToKeywords(UpdateData<GenericKeywordDataModel>(ApplicationPaths.DefaultResultsJsonFilename, true));
                Commands = KeywordHelper.ToKeywords(UpdateData<CommandKeywordDataModel>(ApplicationPaths.CommandsJsonFilename, true).Union(UpdateData<CommandKeywordDataModel>(ApplicationPaths.CommandsJsonFilename, false)));
                HttpKeywords = KeywordHelper.ToKeywords(UpdateData<HttpKeywordDataModel>(ApplicationPaths.HttpKeywordsJsonFilename, true));

                Utilities = KeyphraseHelper.ToKeyphrases(UpdateData<UtilityKeyphraseDataModel>(ApplicationPaths.UtilitiesJsonFilename, true));
                MicrosoftSettings = KeyphraseHelper.ToKeyphrases(UpdateData<MicrosoftSettingKeyphraseDataModel>(ApplicationPaths.MicrosoftSettingsJsonFilename, true));

                Calculator = RepresentationHelper.ToRepresentation(UpdateDatum<CalculatorDataModel>(ApplicationPaths.CalculatorJsonFilename, true));
                Link = RepresentationHelper.ToRepresentation(UpdateDatum<LinkDataModel>(ApplicationPaths.LinkJsonFilename, true));

                string appDataDirectoryPath = ApplicationPaths.AppDataDirectoryPath;
                string applicationName = ApplicationPaths.ApplicationName;
                string appDataApplicationDirectoryPath = Path.Combine(appDataDirectoryPath, applicationName);

                SettingsWatcher = FileSystemWatcherHelper.Initialize(appDataApplicationDirectoryPath, ApplicationPaths.SettingsFilename, OnSettingsChanged);
                DefaultKeywordsWatcher = FileSystemWatcherHelper.Initialize(appDataApplicationDirectoryPath, ApplicationPaths.KeywordsJsonFilename, OnDefaultKeywordsChanged);
                UserKeywordsWatcher = FileSystemWatcherHelper.Initialize(appDataApplicationDirectoryPath, ApplicationPaths.UserKeywordsJsonFilename, OnUserKeywordsChanged);
                CommandsWatcher = FileSystemWatcherHelper.Initialize(appDataApplicationDirectoryPath, ApplicationPaths.CommandsJsonFilename, OnCommandsChanged);
            }
        }

        public SearchTermFactory SearchTermFactory { get; set; } = new();

        public IEnumerable<Keyphrase> MicrosoftSettings { get; set; }

        public SettingsDataModel Settings
        {
            get => _settings;
            set
            {
                _settings = value;
                NotifyOfPropertyChange(() => Settings);
            }
        }

        public Theme Theme
        {
            get => _theme;
            set
            {
                _theme = value;
                NotifyOfPropertyChange(() => Theme);
            }
        }

        public IEnumerable<ShellItem> Applications { get; set; }

        public IEnumerable<Keyword> DefaultKeywords { get; set; }

        public IEnumerable<Keyword> UserKeywords { get; set; }

        public IEnumerable<Keyword> DefaultResults { get; set; }

        public IEnumerable<Keyword> Commands { get; set; }

        public IEnumerable<Keyword> HttpKeywords { get; set; }

        public IEnumerable<Keyphrase> Utilities { get; set; }

        public Representation Calculator { get; set; }

        public Representation Link { get; set; }

        private FileSystemWatcher SettingsWatcher { get; set; }

        private FileSystemWatcher DefaultKeywordsWatcher { get; set; }

        private FileSystemWatcher UserKeywordsWatcher { get; set; }

        private FileSystemWatcher CommandsWatcher { get; set; }

        public static IEnumerable<T> UpdateData<T>(string filename, bool isResource)
        {
            string filePath = FileOperations.GetFilePath(filename, isResource);
            return FileOperations.GetGenericData<T>(filePath);
        }

        public static T UpdateDatum<T>(string filename, bool isResource)
        {
            string filePath = FileOperations.GetFilePath(filename, isResource);
            return FileOperations.GetGenericDatum<T>(filePath);
        }

        public void UpdateTheme()
        {
            string filePath = FileOperations.GetFilePath(ApplicationPaths.ThemesJsonFilename, true);
            IEnumerable<ThemeDataModel> models = FileOperations.GetGenericData<ThemeDataModel>(filePath);
            AccessoryFactory factory = new();
            UnitClient client = new(factory, models.First(m => m.Guid == Settings.ThemeIdentifier));
            Theme = client.Unit as Theme;
        }

        protected void OnSettingsChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                UpdateSettings();
                UpdateTheme();
            }
        }

        private void UpdateSettings()
        {
            string filePath = FileOperations.GetFilePath(ApplicationPaths.SettingsFilename, false);
            Settings = new(filePath);
        }

        private IEnumerable<ShellItem> UpdateShellItems()
        {
            ShellItemClient client = new(SearchTermFactory, WindowsShell.GetApplications());
            return client.ShellItems;
        }

        private void OnDefaultKeywordsChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                DefaultKeywords = KeywordHelper.ToKeywords(UpdateData<GenericKeywordDataModel>(ApplicationPaths.KeywordsJsonFilename, true).Union(UpdateData<GenericKeywordDataModel>(ApplicationPaths.KeywordsJsonFilename, false)));
            }
        }

        private void OnUserKeywordsChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                UserKeywords = KeywordHelper.ToKeywords(UpdateData<GenericKeywordDataModel>(ApplicationPaths.UserKeywordsJsonFilename, false));
            }
        }

        private void OnCommandsChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                Commands = KeywordHelper.ToKeywords(UpdateData<CommandKeywordDataModel>(ApplicationPaths.CommandsJsonFilename, true).Union(UpdateData<CommandKeywordDataModel>(ApplicationPaths.CommandsJsonFilename, false)));
            }
        }
    }
}
