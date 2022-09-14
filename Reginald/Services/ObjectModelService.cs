namespace Reginald.Services
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using Caliburn.Micro;
    using Reginald.Models.ObjectModels;
    using Reginald.Models.Producers;
    using Reginald.Models.Products;
    using Reginald.Services.Watchers;

    internal class ObjectModelService
    {
        private readonly string _appDataPath = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;

#pragma warning disable IDE0052
        private readonly FileSystemWatcher[] _fileSystemWatchers;

        private readonly RegistryKeyWatcher[] _registryKeyWatchers;
#pragma warning restore IDE0052

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

            FileSystemWatcher fsWatcher = new(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            fsWatcher.NotifyFilter = NotifyFilters.Attributes
                                   | NotifyFilters.CreationTime
                                   | NotifyFilters.DirectoryName
                                   | NotifyFilters.FileName
                                   | NotifyFilters.LastAccess
                                   | NotifyFilters.LastWrite
                                   | NotifyFilters.Security
                                   | NotifyFilters.Size;
            fsWatcher.Created += OnCreated;
            fsWatcher.Deleted += OnDeleted;
            fsWatcher.Renamed += OnRenamed;
            fsWatcher.IncludeSubdirectories = true;
            fsWatcher.EnableRaisingEvents = true;
            _fileSystemWatchers = new FileSystemWatcher[] { fsWatcher };
            Thread th = new(SetFileSystemEntries);
            th.Start();
        }

        public ConcurrentDictionary<string, FileSystemEntry> FileSystemEntries { get; set; } = new();

        public ISingleProducer<SearchResult>[] SingleProducers { get; private set; }

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

                    FileSystemEntries[fullPath] = new FileSystemEntry(fullPath);
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

                    _ = FileSystemEntries.TryRemove(fullPath, out _);
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
                    string oldFullPath = e.OldFullPath;
                    if (FileSystemEntries.TryRemove(oldFullPath, out FileSystemEntry entry))
                    {
                        FileSystemEntries[e.FullPath] = entry;
                    }

                    break;
            }
        }

        private void SetFileSystemEntries()
        {
            EnumerationOptions options = new()
            {
                IgnoreInaccessible = true,
                RecurseSubdirectories = true,
            };
            foreach (string entry in Directory.EnumerateFileSystemEntries(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "*", options))
            {
                // Skips file system entry if it's located in the user's %APPDATA%
                // or if its file name begins with a period.
                if (entry.StartsWith(_appDataPath) || Path.GetFileName(entry).StartsWith('.'))
                {
                    continue;
                }

                FileSystemEntries[entry] = new FileSystemEntry(entry);
            }
        }
    }
}
