namespace Reginald.Core.IO
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using Newtonsoft.Json;

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

        public static IEnumerable<T> GetGenericData<T>(string filePath)
        {
            return DeserializeFile<IEnumerable<T>>(filePath) ?? Enumerable.Empty<T>();
        }

        public static T GetGenericDatum<T>(string filePath)
        {
            return DeserializeFile<T>(filePath);
        }

        public static string GetFilePath(string filename, bool isResource)
        {
            string appDataDirectoryPath = ApplicationPaths.AppDataDirectoryPath;
            string applicationName = ApplicationPaths.ApplicationName;
            string applicationAppDataDirectoryPath = Path.Combine(appDataDirectoryPath, applicationName);
            string resourcesDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
            return Path.Combine(isResource ? resourcesDirectoryPath : applicationAppDataDirectoryPath, filename);
        }

        public static T DeserializeFile<T>(string filePath)
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
        }
    }
}
