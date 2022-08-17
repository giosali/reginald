namespace Reginald.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Caliburn.Micro;
    using Reginald.Core.Helpers;
    using Reginald.Core.IO;
    using Reginald.Data.Settings;
    using Reginald.Data.Units;

    public class ConfigurationService : PropertyChangedBase
    {
        private readonly FileSystemWatcher[] _watchers;

        private SettingsDataModel _settings;

        private Theme _theme;

        public ConfigurationService()
        {
            // Creates main directory in %AppData%.
            string directoryPath = FileOperations.ApplicationAppDataDirectoryPath;
            _ = Directory.CreateDirectory(directoryPath);

            // Sets settings and current theme.
            UpdateSettings();
            UpdateTheme();

            // Writes the settings file to %AppData%.
            Settings.Save();

            // Creates a watcher for the settings file.
            // Each time the settings file changes, both the SettingsDataModel object and
            // the Theme object will be updated.
            _watchers = new FileSystemWatcher[] { FileSystemWatcherHelper.Initialize(directoryPath, SettingsDataModel.Filename, OnChanged) };
        }

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

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                UpdateSettings();
                UpdateTheme();
            }
        }

        private void UpdateSettings()
        {
            string filePath = FileOperations.GetFilePath(SettingsDataModel.Filename);
            Settings = new(filePath);
        }

        private void UpdateTheme()
        {
            IEnumerable<ThemeDataModel> models = FileOperations.GetGenericData<ThemeDataModel>(Theme.Filename, true);
            Unit unit = UnitFactory.CreateUnit(models.FirstOrDefault(m => m.Guid == Settings.ThemeIdentifier, models.First()));
            Theme = unit as Theme;
        }
    }
}
