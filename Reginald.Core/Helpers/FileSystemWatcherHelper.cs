namespace Reginald.Core.Helpers
{
    using System.IO;

    public static class FileSystemWatcherHelper
    {
        public static FileSystemWatcher Initialize(string directoryPath, string filename, FileSystemEventHandler handler)
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
    }
}
