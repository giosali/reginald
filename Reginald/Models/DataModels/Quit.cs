namespace Reginald.Models.DataModels
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Newtonsoft.Json;
    using Reginald.Core.Utilities;
    using Reginald.Models.Inputs;
    using Reginald.Models.Producers;
    using Reginald.Models.Products;
    using Reginald.Services.Utilities;

    public class Quit : DataModel, IMultipleProducer<SearchResult>
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

            if (!keyInput.StartsWith(Key + " ", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }

        public SearchResult[] Produce()
        {
            List<SearchResult> results = new();
            List<Process> processes = ProcessUtility.GetTopLevelProcesses();

            string[] keyInputSplit = _keyInput.Split(' ', 2);
            string input = keyInputSplit[^1];
            if (keyInputSplit.Length < 2 || input == " ")
            {
                for (int i = 0; i < processes.Count; i++)
                {
                    Process process = processes[i];
                    string fileName = process.MainModule.FileName;
                    string fileDescription = FileVersionInfo.GetVersionInfo(fileName).FileDescription;
                    if (fileDescription.Length == 0)
                    {
                        continue;
                    }

                    SearchResult result = new(Caption, fileName, string.Format(Format, fileDescription), StaticRandom.Next());
                    result.EnterKeyPressed += OnEnterKeyPressed;
                    _processIds[result.GetHashCode()] = process.Id;
                    results.Add(result);
                }

                return results.ToArray();
            }

            for (int i = 0; i < processes.Count; i++)
            {
                Process process = processes[i];
                string fileName = process.MainModule.FileName;
                string fileDescription = FileVersionInfo.GetVersionInfo(fileName).FileDescription;
                if (!fileDescription.StartsWith(input, StringComparison.OrdinalIgnoreCase) || fileDescription.Length == 0)
                {
                    continue;
                }

                SearchResult result = new(Caption, fileName, string.Format(Format, fileDescription), StaticRandom.Next());
                result.EnterKeyPressed += OnEnterKeyPressed;
                _processIds[result.GetHashCode()] = process.Id;
                results.Add(result);
            }

            return results.ToArray();
        }

        private void OnEnterKeyPressed(object sender, InputProcessingEventArgs e)
        {
            if (!_processIds.TryRemove(sender.GetHashCode(), out int processId))
            {
                return;
            }

            ProcessUtility.QuitProcessById(processId);
            _processIds.Clear();
            e.Handled = true;
        }
    }
}
