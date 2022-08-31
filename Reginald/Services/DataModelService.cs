namespace Reginald.Services
{
    using System.Collections.Generic;
    using System.IO;
    using Reginald.Core.IO;
    using Reginald.Data.DataModels;
    using Reginald.Data.Producers;
    using Reginald.Data.Products;

    internal class DataModelService
    {
#pragma warning disable IDE0052
        private readonly FileSystemWatcher[] _fsws;
#pragma warning restore IDE0052

        public DataModelService()
        {
            Settings = FileOperations.GetGenericDatum<Settings>(Settings.FileName, false);
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

        public Settings Settings { get; set; }

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
            Quit quit = FileOperations.GetGenericDatum<Quit>("Quit.json", true);
            quit.IsEnabled = Settings.IsQuitEnabled;
            multipleProducers.Add(quit);

            multipleProducers.Add(FileOperations.GetGenericDatum<Timers>("Timers.json", true));

            MultipleProducers = multipleProducers.ToArray();
        }

        private void SetSingleProducers()
        {
            List<ISingleProducer<SearchResult>> singleProducers = new();

            // Handles those that receive key inputs.
            singleProducers.AddRange(FileOperations.GetGenericData<WebQuery>(WebQuery.FileName, true));
            singleProducers.AddRange(FileOperations.GetGenericData<WebQuery>(WebQuery.UserFileName, false));

            Recycle recycle = FileOperations.GetGenericDatum<Recycle>("Recycle.json", true);
            recycle.IsEnabled = Settings.IsRecycleEnabled;
            singleProducers.Add(recycle);

            Timer timer = FileOperations.GetGenericDatum<Timer>("Timer.json", true);
            timer.IsEnabled = Settings.IsTimerEnabled;
            singleProducers.Add(timer);

            // Handles those that receive inputs.
            Calculator calculator = FileOperations.GetGenericDatum<Calculator>("Calculator.json", true);
            calculator.IsEnabled = Settings.IsCalculatorEnabled;
            singleProducers.Add(calculator);

            Url url = FileOperations.GetGenericDatum<Url>("Url.json", true);
            url.IsEnabled = Settings.IsUrlEnabled;
            singleProducers.Add(url);

            if (Settings.AreMicrosoftSettingsEnabled)
            {
                singleProducers.AddRange(FileOperations.GetGenericData<MicrosoftSetting>("MicrosoftSettings.json", true));
            }

            SingleProducers = singleProducers.ToArray();
        }
    }
}
