namespace Reginald.Core.IO
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Windows;
    using Newtonsoft.Json;
    using Reginald.Core.Extensions;

    public static class FileOperations
    {
        private const string ApplicationShortcutName = "Reginald.lnk";

        private const string ApplicationResourcesPackUriFormat = "pack://application:,,,/Reginald;component/Resources/{0}";

        private static readonly Guid WindowsScriptHostShellObjectGuid = new("72c24dd5-d70a-438b-8a42-98424b88afb8");

        public static string ApplicationName { get; private set; } = Assembly.GetExecutingAssembly().GetName().Name.Partition(".").Left;

        public static string ApplicationAppDataDirectoryPath { get; private set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationName);

        /// <summary>
        /// Deletes the shortcut file that points to the Reginald executable.
        /// </summary>
        public static void DeleteShortcut()
        {
            string shortcutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), ApplicationShortcutName);
            File.Delete(shortcutPath);
        }

        public static T DeserializeFile<T>(string filePath)
        {
            T type = default;

            if (!File.Exists(filePath))
            {
                return type;
            }

            for (int i = 0; i < 10; i++)
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

            return type;
        }

        public static T DeserializeFile<T>(Uri packUri)
        {
            T type = default;

            for (int i = 0; i < 10; i++)
            {
                try
                {
                    using StreamReader reader = new(Application.GetResourceStream(packUri).Stream);
                    string json = reader.ReadToEnd();
                    type = JsonConvert.DeserializeObject<T>(json);
                    break;
                }

                // If the file is already in use, try again until it's no longer in use
                catch (IOException)
                {
                }
            }

            return type;
        }

        public static string GetFilePath(string fileName)
        {
            return Path.Combine(ApplicationAppDataDirectoryPath, fileName);
        }

        public static T[] GetGenericData<T>(string fileName, bool isResource)
        {
            return (isResource ? DeserializeFile<T[]>(GetResourcePath(fileName)) : DeserializeFile<T[]>(GetFilePath(fileName))) ?? Array.Empty<T>();
        }

        public static T GetGenericDatum<T>(string fileName, bool isResource)
        {
            return isResource ? DeserializeFile<T>(GetResourcePath(fileName)) : (DeserializeFile<T>(GetFilePath(fileName)) ?? JsonConvert.DeserializeObject<T>("{}"));
        }

        public static Uri GetResourcePath(string fileName)
        {
            return new Uri(string.Format(ApplicationResourcesPackUriFormat, fileName));
        }

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

        public static void WriteFile(string fileName, string text = null)
        {
            string filePath = GetFilePath(fileName);
            while (true)
            {
                try
                {
                    File.WriteAllText(filePath, text ?? string.Empty);
                    break;
                }
                catch (IOException)
                {
                }
            }
        }
    }
}
