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
        private static readonly FileSystemWatcher _fsw;

        public DataModelService()
        {
            SetSingleProducers();
            SetMultipleProducers();

            FileSystemWatcher _fsw = new(FileOperations.ApplicationAppDataDirectoryPath);
            _fsw.NotifyFilter = NotifyFilters.Attributes
                              | NotifyFilters.CreationTime
                              | NotifyFilters.LastAccess
                              | NotifyFilters.LastWrite
                              | NotifyFilters.Size;
            _fsw.Changed += OnChanged;
            _fsw.EnableRaisingEvents = true;
        }

        public IMultipleProducer<SearchResult>[] MultipleProducers { get; set; }

        public ISingleProducer<SearchResult>[] SingleProducers { get; set; }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            SetSingleProducers();
            SetMultipleProducers();
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
            singleProducers.AddRange(FileOperations.GetGenericData<WebQuery>("WebQueries.json", true));
            singleProducers.AddRange(FileOperations.GetGenericData<WebQuery>("YourWebQueries.json", false));
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