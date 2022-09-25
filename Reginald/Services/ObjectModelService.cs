namespace Reginald.Services
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using Caliburn.Micro;
    using Reginald.Core.Extensions;
    using Reginald.Core.IO;
    using Reginald.Models.ObjectModels;
    using Reginald.Models.Producers;
    using Reginald.Models.Products;

    internal sealed class ObjectModelService
    {
        private readonly string _appDataPath = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;

#pragma warning disable IDE0052
        private readonly RegistryKeyWatcher[] _registryKeyWatchers;
#pragma warning restore IDE0052

        private readonly FileSystemWatcher _userProfileWatcher;

        public ObjectModelService()
        {
            SetSingleProducers();

            RegistryKeyWatcher localMachine64Bit = new(RegistryHive.LocalMachine, @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall");
            localMachine64Bit.RegistryKeyChanged += OnRegistryKeyChanged;
            RegistryKeyWatcher localMachine = new(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            localMachine.RegistryKeyChanged += OnRegistryKeyChanged;
            RegistryKeyWatcher currentUser = new(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            currentUser.RegistryKeyChanged += OnRegistryKeyChanged;
            _registryKeyWatchers = new RegistryKeyWatcher[] { localMachine64Bit, localMachine, currentUser };

            _userProfileWatcher = new(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            _userProfileWatcher.NotifyFilter = NotifyFilters.Attributes
                                   | NotifyFilters.CreationTime
                                   | NotifyFilters.DirectoryName
                                   | NotifyFilters.FileName
                                   | NotifyFilters.LastAccess
                                   | NotifyFilters.LastWrite
                                   | NotifyFilters.Security
                                   | NotifyFilters.Size;
            _userProfileWatcher.Created += OnCreated;
            _userProfileWatcher.Deleted += OnDeleted;
            _userProfileWatcher.Renamed += OnRenamed;
            _userProfileWatcher.IncludeSubdirectories = true;
            ManageFileSystemEntries(IoC.Get<DataModelService>().Settings.IsFileSearchEnabled);
        }

        public ConcurrentDictionary<uint, FileSystemEntry> FileSystemEntries { get; private set; } = new();

        public ISingleProducer<SearchResult>[] SingleProducers { get; private set; }

        public void ManageFileSystemEntries(bool enable)
        {
            if (enable)
            {
                Thread th = new(SetFileSystemEntries);
                th.IsBackground = true;
                th.Start();
                _userProfileWatcher.EnableRaisingEvents = true;
            }
            else
            {
                _userProfileWatcher.EnableRaisingEvents = false;
                FileSystemEntries.Clear();
            }
        }

        public void SetSingleProducers()
        {
            List<ISingleProducer<SearchResult>> singleProducers = new();
            if (IoC.Get<DataModelService>().Settings.AreApplicationsEnabled)
            {
                singleProducers.AddRange(Application.GetApplications());
            }

            SingleProducers = singleProducers.ToArray();
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Created:
                    string fullPath = e.FullPath;
                    if (fullPath.Contains(_appDataPath) || Path.GetFileName(fullPath).StartsWith('.'))
                    {
                        break;
                    }

                    FileSystemEntries[fullPath.GetCrc32HashCode()] = new FileSystemEntry(fullPath.Split(FileSystemEntry.UserProfile, 2)[^1]);

                    if (!Directory.Exists(fullPath))
                    {
                        break;
                    }

                    // Re-adds file system entries if a directory that contains files
                    // and/or subdirectories is restored from the Recycle Bin.
                    string[] files = new string[] { };
                    try
                    {
                        files = Directory.GetFiles(fullPath, "*", SearchOption.AllDirectories);
                    }
                    catch (SystemException)
                    {
                        break;
                    }

                    for (int i = 0; i < files.Length; i++)
                    {
                        string file = files[i];
                        FileSystemEntries[file.GetCrc32HashCode()] = new FileSystemEntry(file.Split(FileSystemEntry.UserProfile, 2)[^1]);
                    }

                    break;
            }
        }

        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Deleted:
                    string fullPath = e.FullPath;
                    if (fullPath.Contains(_appDataPath))
                    {
                        break;
                    }

                    // Breaks if the path doesn't exist in the dictionary
                    // or if the removed file system entry is not a directory.
                    if (!FileSystemEntries.TryRemove(fullPath.GetCrc32HashCode(), out FileSystemEntry removedEntry) || removedEntry.Type == EntryType.File)
                    {
                        break;
                    }

                    // Removes entries from deleted directories.
                    string match = fullPath.Split(FileSystemEntry.UserProfile, 2)[^1];
                    foreach (FileSystemEntry entry in FileSystemEntries.Values)
                    {
                        string caption = entry.Caption;
                        if (caption.StartsWith(match))
                        {
                            _ = FileSystemEntries.TryRemove((FileSystemEntry.UserProfile + caption).GetCrc32HashCode(), out _);
                        }
                    }

                    break;
            }
        }

        private void OnRegistryKeyChanged(object sender, EventArgs e)
        {
            SetSingleProducers();
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Renamed:
                    string oldPath = e.OldFullPath;
                    if (oldPath.Contains(_appDataPath))
                    {
                        break;
                    }

                    string newPath = e.FullPath;
                    if (FileSystemEntries.TryRemove(oldPath.GetCrc32HashCode(), out FileSystemEntry entry))
                    {
                        entry.UpdatePath(newPath.Split(FileSystemEntry.UserProfile)[^1]);
                        FileSystemEntries[newPath.GetCrc32HashCode()] = entry;
                    }

                    if (!Directory.Exists(newPath))
                    {
                        break;
                    }

                    string[] files = new string[] { };
                    try
                    {
                        files = Directory.GetFiles(newPath, "*", SearchOption.AllDirectories);
                    }
                    catch (SystemException)
                    {
                        break;
                    }

                    // Changes the file path for all file system entries in
                    // renamed directories.
                    for (int i = 0; i < files.Length; i++)
                    {
                        string newFile = files[i];
                        string oldFile = newFile.Replace(newPath, oldPath);
                        if (FileSystemEntries.TryRemove(oldFile.GetCrc32HashCode(), out entry))
                        {
                            entry.UpdatePath(newFile.Split(FileSystemEntry.UserProfile)[^1]);
                            FileSystemEntries[newFile.GetCrc32HashCode()] = entry;
                        }
                    }

                    break;
            }
        }

        private void SetFileSystemEntries()
        {
            string[] filters = IoC.Get<DataModelService>().Settings.FileSearchFilters.ToArray();
            EnumerationOptions options = new()
            {
                IgnoreInaccessible = true,
                RecurseSubdirectories = true,
            };
            bool isFiltersEmpty = filters.Length == 0;
            char sep = Path.DirectorySeparatorChar;
            foreach (string entry in Directory.EnumerateFileSystemEntries(FileSystemEntry.UserProfile, "*", options))
            {
                // Skips file system entry if it's located in the user's %APPDATA%
                string fileName = Path.GetFileName(entry);
                if (entry.StartsWith(_appDataPath))
                {
                    continue;
                }

                bool skip = false;

                // Ignores files and directories that begin with a period.
                int index = 0;
                while (index != -1)
                {
                    index = entry.IndexOf('.', index);
                    if (index == -1)
                    {
                        break;
                    }

                    if (entry[index - 1] == sep)
                    {
                        skip = true;
                        break;
                    }

                    index++;
                }

                if (skip)
                {
                    continue;
                }

                bool isDir = Directory.Exists(entry);
                for (int i = 0; i < filters.Length; i++)
                {
                    string filter = filters[i];

                    // Handles filters directed at files.
                    if (filter[^1] != '/')
                    {
                        // Skips if the entry is a directory.
                        // or if the file name doesn't match the filter.
                        if (Directory.Exists(entry) || fileName != filter)
                        {
                            continue;
                        }

                        skip = true;
                        break;
                    }

                    // Handles filters directed at directories.
                    else
                    {
                        string dirName = filter[..^1];
                        index = 0;
                        while (index != -1)
                        {
                            index = entry.IndexOf(dirName, index);
                            if (index == -1 || index == 0)
                            {
                                break;
                            }

                            int dirNameLength = dirName.Length;
                            if (entry[index - 1] == sep && (index + dirNameLength == entry.Length || entry[index + dirNameLength] == sep))
                            {
                                skip = true;
                                break;
                            }

                            index++;
                        }

                        if (skip)
                        {
                            break;
                        }
                    }
                }

                if (skip)
                {
                    continue;
                }

                FileSystemEntries[entry.GetCrc32HashCode()] = new FileSystemEntry(entry.Split(FileSystemEntry.UserProfile)[^1]);
            }
        }
    }
}
