namespace Reginald.Models.DataModels
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using Newtonsoft.Json;
    using Reginald.Core.Services;
    using Reginald.Models.Inputs;
    using Reginald.Models.Producers;
    using Reginald.Models.Products;

    internal sealed class Quit : DataModel, IMultipleProducer<SearchResult>
    {
        private static readonly ConcurrentDictionary<int, int> _processIds = new();

        private string _keyInput;

        [JsonProperty("format")]
        public string Format { get; set; }

        [JsonProperty("isEnabled")]
        public bool IsEnabled { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        public bool Check(string keyInput)
        {
            if (!IsEnabled)
            {
                return false;
            }

            _keyInput = keyInput;
            if (keyInput.Length <= Key.Length)
            {
                return Key.StartsWith(keyInput, StringComparison.OrdinalIgnoreCase);
            }

            return keyInput.StartsWith(Key + " ", StringComparison.OrdinalIgnoreCase);
        }

        public SearchResult[] Produce(CancellationToken token)
        {
            List<SearchResult> results = new();
            List<Process> processes = ProcessService.GetTopLevelProcesses();

            string[] keyInputSplit = _keyInput.Split(' ', 2);
            string input = keyInputSplit[^1];
            bool isInputInvalid = keyInputSplit.Length < 2 || input == " ";
            for (int i = 0; i < processes.Count; i++)
            {
                token.ThrowIfCancellationRequested();
                Process process = processes[i];

                // Skips File Explorer.
                if (process.ProcessName == "explorer")
                {
                    continue;
                }

                string fileName = process.MainModule.FileName;
                string fileDescription = FileVersionInfo.GetVersionInfo(fileName).FileDescription;

                // Skips if the file description is empty
                // or if it doesn't start with the input if the input is valid.
                if (fileDescription.Length == 0 || !fileDescription.StartsWith(isInputInvalid ? string.Empty : input, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                SearchResult result = new(Caption, fileName, string.Format(Format, fileDescription), fileName.GetHashCode());
                result.EnterKeyPressed += OnEnterKeyPressed;
                _processIds[result.GetHashCode()] = process.Id;
                if (results.IndexOf(result) == -1)
                {
                    results.Add(result);
                }
            }

            return results.ToArray();
        }

        private void OnEnterKeyPressed(object sender, InputProcessingEventArgs e)
        {
            if (!_processIds.TryRemove(sender.GetHashCode(), out int processId))
            {
                return;
            }

            _processIds.Clear();
            ProcessService.QuitProcessById(processId);
            e.Handled = true;
        }
    }
}
