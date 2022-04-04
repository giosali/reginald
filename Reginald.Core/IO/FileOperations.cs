namespace Reginald.Core.IO
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using Newtonsoft.Json;
    using Reginald.Core.Extensions;

    public static class FileOperations
    {
        private const string ApplicationShortcutName = "Reginald.lnk";

        private static readonly Guid WindowsScriptHostShellObjectGuid = new("72c24dd5-d70a-438b-8a42-98424b88afb8");

        public static string ApplicationName { get; private set; } = Assembly.GetExecutingAssembly().GetName().Name.Partition(".").Left;

        public static string ApplicationAppDataDirectoryPath { get; private set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationName);

        /// <summary>
        /// Creates a shortcut file that points to the Reginald executable. A return value indicates whether the shortcut was created.
        /// </summary>
        /// <returns><see langword="true"/> if the shortcut was created; otherwise, <see langword="false"/>.</returns>
        public static bool TryCreateShortcut()
        {
            string executablePath = Environment.ProcessPath;
            string shortcutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), ApplicationShortcutName);

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
            string shortcutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), ApplicationShortcutName);
            File.Delete(shortcutPath);
        }

        public static void WriteFile(string filename, string json = null)
        {
            string filePath = GetFilePath(filename, false);

            // If the file doesn't exist, create it
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

        public static T[] GetGenericData<T>(string filePath)
        {
            return DeserializeFile<T[]>(filePath) ?? Array.Empty<T>();
        }

        public static T[] GetGenericData<T>(string filename, bool isResource)
        {
            string filePath = GetFilePath(filename, isResource);
            return DeserializeFile<T[]>(filePath) ?? Array.Empty<T>();
        }

        public static T GetGenericDatum<T>(string filename, bool isResource)
        {
            string filePath = GetFilePath(filename, isResource);
            return DeserializeFile<T>(filePath);
        }

        public static string GetFilePath(string filename, bool isResource)
        {
            string resourcesDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
            return Path.Combine(isResource ? resourcesDirectoryPath : ApplicationAppDataDirectoryPath, filename);
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
    }
}
