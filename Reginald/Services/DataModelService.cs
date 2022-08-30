using System.Collections.Generic;
using System.IO;
using Reginald.Core.IO;
using Reginald.Data.DataModels;
using Reginald.Data.Producers;
using Reginald.Data.Products;

namespace Reginald.Services
{
    internal class DataModelService
    {
        private readonly FileSystemWatcher[] _fsws;

        public DataModelService()
        {
            SetSingleProducers();
            SetMultipleProducers();

            FileSystemWatcher fsw = new(FileOperations.ApplicationAppDataDirectoryPath, "*.json");
            fsw.NotifyFilter = NotifyFilters.Attributes
                             | NotifyFilters.CreationTime
                             | NotifyFilters.LastAccess
                             | NotifyFilters.LastWrite
                             | NotifyFilters.Size;
            fsw.Changed += OnChanged;
            fsw.EnableRaisingEvents = true;
            _fsws = new FileSystemWatcher[] { fsw };
        }

        public IMultipleProducer<SearchResult>[] MultipleProducers { get; set; }

        public ISingleProducer<SearchResult>[] SingleProducers { get; set; }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Created:
                case WatcherChangeTypes.Deleted:
                case WatcherChangeTypes.Changed:
                    SetSingleProducers();
                    SetMultipleProducers();
                    break;
            }
        }

        private void SetMultipleProducers()
        {
            List<IMultipleProducer<SearchResult>> multipleProducers = new();

            // Handles those that receive key inputs.
            multipleProducers.Add(FileOperations.GetGenericDatum<Quit>("Quit.json", true));
            multipleProducers.Add(FileOperations.GetGenericDatum<Timers>("Timers.json", true));

            MultipleProducers = multipleProducers.ToArray();
        }

        private void SetSingleProducers()
        {
            List<ISingleProducer<SearchResult>> singleProducers = new();

            // Handles those that receive key inputs.
            singleProducers.AddRange(FileOperations.GetGenericData<WebQuery>(WebQuery.FileName, true));
            singleProducers.AddRange(FileOperations.GetGenericData<WebQuery>(WebQuery.UserFileName, false));
            singleProducers.Add(FileOperations.GetGenericDatum<Recycle>("Recycle.json", true));
            singleProducers.Add(FileOperations.GetGenericDatum<Timer>("Timer.json", true));

            // Handles those that receive inputs.
            singleProducers.Add(FileOperations.GetGenericDatum<Calculator>("Calculator.json", true));
            singleProducers.Add(FileOperations.GetGenericDatum<Url>("Url.json", true));
            singleProducers.AddRange(FileOperations.GetGenericData<MicrosoftSetting>("MicrosoftSettings.json", true));

            SingleProducers = singleProducers.ToArray();
        }
    }
}
