using Caliburn.Micro;
using Reginald.Core.AbstractProducts;
using Reginald.Core.Clients;
using Reginald.Core.DataModels;
using Reginald.Core.Factories;
using Reginald.Core.IO;
using Reginald.Core.Products;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Reginald.ViewModels
{
    public class DataViewModelBase : Screen
    {
        public SearchTermFactory SearchTermFactory { get; set; } = new();

        private SettingsDataModel _settings = new();
        public SettingsDataModel Settings
        {
            get => _settings;
            set
            {
                _settings = value;
                NotifyOfPropertyChange(() => Settings);
            }
        }

        private Theme _theme;
        public Theme Theme
        {
            get => _theme;
            set
            {
                _theme = value;
                NotifyOfPropertyChange(() => Theme);
            }
        }

        private IEnumerable<ShellItem> _applications;
        public IEnumerable<ShellItem> Applications
        {
            get => _applications;
            set
            {
                _applications = value;
                NotifyOfPropertyChange(() => Applications);
            }
        }

        private IEnumerable<Keyword> _defaultKeywords;
        public IEnumerable<Keyword> DefaultKeywords
        {
            get => _defaultKeywords;
            set
            {
                _defaultKeywords = value;
                NotifyOfPropertyChange(() => DefaultKeywords);
            }
        }

        private IEnumerable<Keyword> _userKeywords;
        public IEnumerable<Keyword> UserKeywords
        {
            get => _userKeywords;
            set
            {
                _userKeywords = value;
                NotifyOfPropertyChange(() => UserKeywords);
            }
        }

        private IEnumerable<Keyword> _defaultResults;
        public IEnumerable<Keyword> DefaultResults
        {
            get => _defaultResults;
            set
            {
                _defaultResults = value;
                NotifyOfPropertyChange(() => DefaultResults);
            }
        }

        private IEnumerable<Keyword> _commands;
        public IEnumerable<Keyword> Commands
        {
            get => _commands;
            set
            {
                _commands = value;
                NotifyOfPropertyChange(() => Commands);
            }
        }

        private IEnumerable<Keyword> _httpKeywords;
        public IEnumerable<Keyword> HttpKeywords
        {
            get => _httpKeywords;
            set
            {
                _httpKeywords = value;
                NotifyOfPropertyChange(() => HttpKeywords);
            }
        }

        private IEnumerable<Keyphrase> _utilities;
        public IEnumerable<Keyphrase> Utilities
        {
            get => _utilities;
            set
            {
                _utilities = value;
                NotifyOfPropertyChange(() => Utilities);
            }
        }

        public IEnumerable<Keyphrase> MicrosoftSettings { get; set; }

        private Representation _calculator;
        public Representation Calculator
        {
            get => _calculator;
            set
            {
                _calculator = value;
                NotifyOfPropertyChange(() => Calculator);
            }
        }

        private Representation _link;
        public Representation Link
        {
            get => _link;
            set
            {
                _link = value;
                NotifyOfPropertyChange(() => Link);
            }
        }

        private FileSystemWatcher SettingsWatcher { get; set; }

        private FileSystemWatcher DefaultKeywordsWatcher { get; set; }

        private FileSystemWatcher UserKeywordsWatcher { get; set; }

        private FileSystemWatcher CommandsWatcher { get; set; }

        public DataViewModelBase(bool monitorChanges)
        {
            UpdateSettings();
            Theme = UpdateUnit<ThemeDataModel>(ApplicationPaths.ThemesJsonFilename, true, Settings.ThemeIdentifier) as Theme;

            if (monitorChanges)
            {
                Applications = UpdateShellItems();
                DefaultKeywords = UpdateKeywords<GenericKeywordDataModel>(ApplicationPaths.KeywordsJsonFilename, true, true);
                UserKeywords = UpdateKeywords<GenericKeywordDataModel>(ApplicationPaths.UserKeywordsJsonFilename, false, true);
                DefaultResults = UpdateKeywords<GenericKeywordDataModel>(ApplicationPaths.DefaultResultsJsonFilename, true, true);
                Commands = UpdateKeywords<CommandDataModel>(ApplicationPaths.CommandsJsonFilename, true, true);
                HttpKeywords = UpdateKeywords<HttpKeywordDataModel>(ApplicationPaths.HttpKeywordsJsonFilename, true, true);
                Utilities = UpdateKeyphrases<UtilityDataModel>(ApplicationPaths.UtilitiesJsonFilename);
                MicrosoftSettings = UpdateKeyphrases<MicrosoftSettingDataModel>(ApplicationPaths.MicrosoftSettingsJsonFilename);
                Calculator = UpdateRepresentation<CalculatorDataModel>(ApplicationPaths.CalculatorJsonFilename);
                Link = UpdateRepresentation<LinkDataModel>(ApplicationPaths.LinkJsonFilename);

                string appDataDirectoryPath = ApplicationPaths.AppDataDirectoryPath;
                string applicationName = ApplicationPaths.ApplicationName;
                string appDataApplicationDirectoryPath = Path.Combine(appDataDirectoryPath, applicationName);

                SettingsWatcher = CreateWatcher(appDataApplicationDirectoryPath, ApplicationPaths.SettingsFilename, OnSettingsChanged);
                DefaultKeywordsWatcher = CreateWatcher(appDataApplicationDirectoryPath, ApplicationPaths.KeywordsJsonFilename, OnDefaultKeywordsChanged);
                UserKeywordsWatcher = CreateWatcher(appDataApplicationDirectoryPath, ApplicationPaths.UserKeywordsJsonFilename, OnUserKeywordsChanged);
                CommandsWatcher = CreateWatcher(appDataApplicationDirectoryPath, ApplicationPaths.CommandsJsonFilename, OnCommandsChanged);
            }
        }

        public static Unit UpdateUnit<T>(string filename, bool isResource, string parameter)
        {
            IEnumerable<UnitDataModelBase> models = FileOperations.GetUnitData<T>(filename, isResource);
            AccessoryFactory factory = new();
            UnitClient client = new(factory, models.First(m => m.Predicate(m, parameter)));
            return client.Unit;
        }

        public static IEnumerable<Unit> UpdateUnits<T>(string filename, bool isResource)
        {
            IEnumerable<UnitDataModelBase> models = FileOperations.GetUnitData<T>(filename, isResource);
            AccessoryFactory factory = new();
            UnitClient client = new(factory, models);
            return client.Units;
        }

        public void UpdateTheme(string filename, bool isResource)
        {
            IEnumerable<ThemeDataModel> models = FileOperations.GetGenericData<ThemeDataModel>(filename, isResource);
            AccessoryFactory factory = new();
            UnitClient client = new(factory, models.First(m => m.Guid == Settings.ThemeIdentifier));
            Theme = client.Unit as Theme;
        }

        public IEnumerable<Keyword> UpdateKeywords<T>(string filename, bool isResource, bool filter)
        {
            IEnumerable<KeywordDataModelBase> models = filter
                                                     ? FileOperations.GetKeywordData<T>(filename, isResource)
                                                                     .Where(k => k.IsEnabled)
                                                     : FileOperations.GetKeywordData<T>(filename, isResource);
            if (models is not null)
            {
                KeywordClient client = new(SearchTermFactory, models);
                return client.Keywords;
            }
            return Enumerable.Empty<Keyword>();
        }

        public IEnumerable<Keyphrase> UpdateKeyphrases<T>(string filename)
        {
            IEnumerable<KeyphraseDataModelBase> models = FileOperations.GetKeyphraseData<T>(filename, true);
            if (models is not null)
            {
                KeyphraseClient client = new(SearchTermFactory, models);
                return client.Keyphrases;
            }
            return Enumerable.Empty<Keyphrase>();
        }

        private static FileSystemWatcher CreateWatcher(string directoryPath, string filename, FileSystemEventHandler handler)
        {
            FileSystemWatcher watcher = new(directoryPath, filename);
            watcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Size;
            watcher.Changed += handler;
            watcher.EnableRaisingEvents = true;
            return watcher;
        }

        private static Representation UpdateRepresentation<T>(string filename)
        {
            InputDataModelBase model = FileOperations.GetRepresentationDatum<T>(filename, true);
            RepresentationFactory factory = new();
            RepresentationClient client = new(factory, model);
            return client.Representation;
        }

        private void UpdateSettings()
        {
            SettingsDataModel settings = FileOperations.GetSettingsData(ApplicationPaths.SettingsFilename);
            if (settings is not null)
            {
                Settings = settings;
            }
        }

        private IEnumerable<ShellItem> UpdateShellItems()
        {
            ShellItemClient client = new(SearchTermFactory, Core.Base.Applications.GetApplications());
            return client.ShellItems;
        }

        private void OnSettingsChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                UpdateSettings();
                Theme = UpdateUnit<ThemeDataModel>(ApplicationPaths.ThemesJsonFilename, true, Settings.ThemeIdentifier) as Theme;
            }
        }

        private void OnDefaultKeywordsChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                DefaultKeywords = UpdateKeywords<GenericKeywordDataModel>(ApplicationPaths.KeywordsJsonFilename, true, true);
            }
        }

        private void OnUserKeywordsChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                UserKeywords = UpdateKeywords<GenericKeywordDataModel>(ApplicationPaths.UserKeywordsJsonFilename, false, true);
            }
        }

        private void OnCommandsChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                Commands = UpdateKeywords<CommandDataModel>(ApplicationPaths.CommandsJsonFilename, true, true);
            }
        }
    }
}
