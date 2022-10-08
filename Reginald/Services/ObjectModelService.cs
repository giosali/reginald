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
            localMachine64Bit.Start();
            RegistryKeyWatcher localMachine = new(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            localMachine.RegistryKeyChanged += OnRegistryKeyChanged;
            localMachine.Start();
            RegistryKeyWatcher currentUser = new(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            currentUser.RegistryKeyChanged += OnRegistryKeyChanged;
            currentUser.Start();
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
                    string[] filters = IoC.Get<DataModelService>().Settings.FileSearchFilters.ToArray();
                    if (SkipEntry(fullPath, filters))
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
                        if (SkipEntry(file, filters))
                        {
                            continue;
                        }

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
                    if (SkipEntry(newPath, IoC.Get<DataModelService>().Settings.FileSearchFilters.ToArray()))
                    {
                        // Removes the old path and ignores the new one
                        // if the new path contains an invalid path name.
                        _ = FileSystemEntries.TryRemove(oldPath.GetCrc32HashCode(), out _);

                        if (!Directory.Exists(newPath))
                        {
                            break;
                        }

                        // Removes all sub-entries that also contain the invalid path name.
                        try
                        {
                            string[] subEntries = Directory.GetFiles(newPath, "*", SearchOption.AllDirectories);
                            for (int i = 0; i < subEntries.Length; i++)
                            {
                                string newFile = subEntries[i];
                                string oldFile = newFile.Replace(newPath, oldPath);
                                _ = FileSystemEntries.TryRemove(oldFile.GetCrc32HashCode(), out _);
                            }
                        }
                        catch (SystemException)
                        {
                            break;
                        }
                    }

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
            foreach (string entry in Directory.EnumerateFileSystemEntries(FileSystemEntry.UserProfile, "*", options))
            {
                if (SkipEntry(entry, filters))
                {
                    continue;
                }

                FileSystemEntries[entry.GetCrc32HashCode()] = new FileSystemEntry(entry.Split(FileSystemEntry.UserProfile)[^1]);
            }
        }

        private bool SkipEntry(string entry, string[] filters)
        {
            // Skips file system entry if it's located in the user's %APPDATA%
            string fileName = Path.GetFileName(entry);
            if (entry.StartsWith(_appDataPath))
            {
                return true;
            }

            // Ignores files and directories that begin with a period.
            int index = 0;
            while ((index = entry.IndexOf('.', index)) != -1)
            {
                if (entry[index - 1] == Path.DirectorySeparatorChar)
                {
                    return true;
                }

                index++;
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
                    if (isDir || fileName != filter)
                    {
                        continue;
                    }

                    return true;
                }

                // Handles filters directed at directories.
                else
                {
                    string dirName = filter[..^1];
                    index = 0;
                    while ((index = entry.IndexOf(dirName, index)) != -1 && index != 0)
                    {
                        int dirNameLength = dirName.Length;
                        if (entry[index - 1] == Path.DirectorySeparatorChar && (index + dirNameLength == entry.Length || entry[index + dirNameLength] == Path.DirectorySeparatorChar))
                        {
                            return true;
                        }

                        index++;
                    }
                }
            }

            return false;
        }
    }
}
