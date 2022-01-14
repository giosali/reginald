namespace Reginald.Core.IO
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using Newtonsoft.Json;
    using Reginald.Core.DataModels;
    using Reginald.Core.Extensions;

    public static class FileOperations
    {
        private static readonly Guid WindowsScriptHostShellObjectGuid = new("72c24dd5-d70a-438b-8a42-98424b88afb8");

        /// <summary>
        /// Creates a shortcut file that points to the Reginald executable. A return value indicates whether the shortcut was created.
        /// </summary>
        /// <returns><see langword="true"/> if the shortcut was created; otherwise, <see langword="false"/>.</returns>
        public static bool TryCreateShortcut()
        {
            string executablePath = Process.GetCurrentProcess().MainModule.FileName;
            string shortcutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), ApplicationPaths.ApplicationShortcutName);

            if (File.Exists(shortcutPath))
            {
                return false;
            }
            else
            {
                Type t = Type.GetTypeFromCLSID(WindowsScriptHostShellObjectGuid);
                dynamic wshShell = Activator.CreateInstance(t);
                try
                {
                    dynamic iWshShortcut = wshShell.CreateShortcut(shortcutPath);
                    iWshShortcut.Arguments = string.Empty;
                    iWshShortcut.TargetPath = executablePath;
                    iWshShortcut.WindowStyle = 1;
                    iWshShortcut.WorkingDirectory = Directory.GetParent(executablePath).FullName;
                    iWshShortcut.Save();
                }
                finally
                {
                    Marshal.FinalReleaseComObject(wshShell);
                }

                return true;
            }
        }

        /// <summary>
        /// Deletes the shortcut file that points to the Reginald executable.
        /// </summary>
        public static void DeleteShortcut()
        {
            string shortcutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), ApplicationPaths.ApplicationShortcutName);
            File.Delete(shortcutPath);
        }

        public static void WriteFile(string filename, string json = null)
        {
            // If the file doesn't exist, create it
            string filePath = GetFilePath(filename, false);
            if (!File.Exists(filePath))
            {
                using FileStream stream = File.Create(filePath);
            }

            // Write contents to the file
            if (json is not null)
            {
                while (true)
                {
                    try
                    {
                        File.WriteAllText(filePath, json);
                        break;
                    }
                    catch (IOException)
                    {
                    }
                }
            }
        }

        public static IEnumerable<KeywordDataModelBase> GetKeywordData<T>(string filename, bool isResource)
        {
            string resourceFilePath = GetFilePath(filename, isResource);
            IEnumerable<KeywordDataModelBase> models = DeserializeFile<IEnumerable<T>>(resourceFilePath) as IEnumerable<KeywordDataModelBase> ?? Enumerable.Empty<KeywordDataModelBase>();

            // Disable models that aren't supposed to be enabled
            if (isResource)
            {
                string localFilePath = GetFilePath(filename, false);
                IEnumerable<KeywordDataModelBase> disabledModels = DeserializeFile<IEnumerable<T>>(localFilePath) as IEnumerable<KeywordDataModelBase> ?? Enumerable.Empty<KeywordDataModelBase>();

                foreach (KeywordDataModelBase disabledModel in disabledModels)
                {
                    foreach (KeywordDataModelBase model in models)
                    {
                        if (model == disabledModel)
                        {
                            model.IsEnabled = false;
                            break;
                        }
                    }
                }
            }

            return models;
        }

        public static IEnumerable<KeyphraseDataModelBase> GetKeyphraseData<T>(string filename, bool isResource)
        {
            string resourceFilePath = GetFilePath(filename, isResource);
            IEnumerable<KeyphraseDataModelBase> models = DeserializeFile<IEnumerable<T>>(resourceFilePath) as IEnumerable<KeyphraseDataModelBase> ?? Enumerable.Empty<KeyphraseDataModelBase>();

            // Disable models that aren't supposed to be enabled
            if (isResource)
            {
                string localFilePath = GetFilePath(filename, false);
                IEnumerable<KeyphraseDataModelBase> disabledModels = DeserializeFile<IEnumerable<T>>(localFilePath) as IEnumerable<KeyphraseDataModelBase> ?? Enumerable.Empty<KeyphraseDataModelBase>();

                foreach (KeyphraseDataModelBase disabledModel in disabledModels)
                {
                    foreach (KeyphraseDataModelBase model in models)
                    {
                        if (model == disabledModel)
                        {
                            model.IsEnabled = false;
                            break;
                        }
                    }
                }
            }

            return models;
        }

        public static IEnumerable<UnitDataModelBase> GetUnitData<T>(string filename, bool isResource)
        {
            string resourceFilePath = GetFilePath(filename, isResource);
            IEnumerable<UnitDataModelBase> models = DeserializeFile<IEnumerable<T>>(resourceFilePath) as IEnumerable<UnitDataModelBase> ?? Enumerable.Empty<UnitDataModelBase>();

            // Combine local models to resource models
            if (isResource)
            {
                string localFilePath = GetFilePath(filename, false);
                models = models.Concat(DeserializeFile<IEnumerable<T>>(localFilePath) as IEnumerable<UnitDataModelBase> ?? Enumerable.Empty<UnitDataModelBase>());
            }

            return models;
        }

        public static SettingsDataModel GetSettingsData(string filename)
        {
            string filePath = GetFilePath(filename, false);
            SettingsDataModel protoSettings = DeserializeFile<SettingsDataModel>(filePath);

            // Assign properties
            SettingsDataModel settings = new()
            {
                IncludeInstalledApplications = protoSettings?.IncludeInstalledApplications ?? true,
                IncludeDefaultKeywords = protoSettings?.IncludeDefaultKeywords ?? true,
                IncludeHttpKeywords = protoSettings?.IncludeHttpKeywords ?? true,
                IncludeCommands = protoSettings?.IncludeCommands ?? true,
                IncludeUtilities = protoSettings?.IncludeUtilities ?? true,
                IncludeSettingsPages = protoSettings?.IncludeSettingsPages ?? true,
                AreExpansionsEnabled = protoSettings?.AreExpansionsEnabled ?? true,
                LaunchOnStartup = protoSettings?.LaunchOnStartup ?? true,
                SearchBoxKey = protoSettings?.SearchBoxKey ?? "Space",
                SearchBoxModifierOne = protoSettings?.SearchBoxModifierOne ?? "Alt",
                SearchBoxModifierTwo = protoSettings?.SearchBoxModifierTwo ?? "None",
                ThemeIdentifier = protoSettings?.ThemeIdentifier ?? "553a4cdf-11c6-49ce-b634-7ce6945f6958",
            };
            return settings;
        }

        public static IEnumerable<T> GetGenericData<T>(string filename, bool isResource)
        {
            string filePath = GetFilePath(filename, isResource);
            IEnumerable<T> models = DeserializeFile<IEnumerable<T>>(filePath);
            return models;
        }

        public static T GetGenericDatum<T>(string filename, bool isResource)
        {
            string filePath = GetFilePath(filename, isResource);
            T model = DeserializeFile<T>(filePath);
            return model;
        }

        public static InputDataModelBase GetRepresentationDatum<T>(string filename, bool isResource)
        {
            string filePath = GetFilePath(filename, isResource);
            InputDataModelBase model = DeserializeFile<T>(filePath) as InputDataModelBase;
            return model;
        }

        public static string GetFilePath(string filename, bool isResource)
        {
            string appDataDirectoryPath = ApplicationPaths.AppDataDirectoryPath;
            string applicationName = ApplicationPaths.ApplicationName;
            string applicationAppDataDirectoryPath = Path.Combine(appDataDirectoryPath, applicationName);
            string resourcesDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
            return Path.Combine(isResource ? resourcesDirectoryPath : applicationAppDataDirectoryPath, filename);
        }

        public static void SetUp()
        {
            string appDataDirectoryPath = ApplicationPaths.AppDataDirectoryPath;
            string applicationName = ApplicationPaths.ApplicationName;
            string applicationAppDataDirectoryPath = Path.Combine(appDataDirectoryPath, applicationName);

            // Creates "Reginald" in %AppData%
            _ = Directory.CreateDirectory(applicationAppDataDirectoryPath);

            // Creates "Reginald\UserIcons" in %AppData%
            string userIconsDirectoryName = ApplicationPaths.UserIconsDirectoryName;
            string userIconsDirectoryPath = Path.Combine(appDataDirectoryPath, applicationName, userIconsDirectoryName);
            _ = Directory.CreateDirectory(userIconsDirectoryPath);

            // Creates and updates "Reginald\Settings.json" in %AppData%
            SettingsDataModel settings = GetSettingsData(ApplicationPaths.SettingsFilename);
            WriteFile(ApplicationPaths.SettingsFilename, settings.Serialize());

            // Creates "Reginald\Expansions.json" in %AppData%
            WriteFile(ApplicationPaths.ExpansionsJsonFilename);

            // Creates "Reginald\Keywords.json" in %AppData%
            WriteFile(ApplicationPaths.KeywordsJsonFilename);

            // Creates "Reginald\UserKeywords.json" in %AppData%
            WriteFile(ApplicationPaths.UserKeywordsJsonFilename);
        }

        private static T DeserializeFile<T>(string filePath)
        {
            T type = default;

            // Check if file exists
            if (File.Exists(filePath))
            {
                // If it does, extract its contents
                while (true)
                {
                    try
                    {
                        using StreamReader reader = new(filePath);
                        string json = reader.ReadToEnd();
                        type = JsonConvert.DeserializeObject<T>(json);
                        break;
                    }

                    // If the file is already in use, try again until it's no longer in use
                    catch (IOException)
                    {
                    }
                }
            }

            return type;
        }
    }
}
