namespace Reginald.Data.ObjectModels
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Newtonsoft.Json;
    using Reginald.Data.Inputs;
    using Reginald.Data.Producers;
    using Reginald.Data.Products;
    using Reginald.Services.Utilities;

    public class Quit : DataModel, IMultipleProducer<SearchResult>
    {
        private static Dictionary<int, int> _processIds = new();

        private string _keyInput;

        [JsonProperty("format")]
        public string Format { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        public bool Check(string keyInput)
        {
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
                    SearchResult result = new(Caption, fileName, string.Format(Format, FileVersionInfo.GetVersionInfo(fileName).FileDescription));
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
                if (!fileDescription.StartsWith(input, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                SearchResult result = new(Caption, fileName, string.Format(Format, fileDescription));
                result.EnterKeyPressed += OnEnterKeyPressed;
                _processIds[result.GetHashCode()] = process.Id;
                results.Add(result);
            }

            return results.ToArray();
        }

        private void OnEnterKeyPressed(object sender, InputProcessingEventArgs e)
        {
            int hashCode = sender.GetHashCode();
            if (!_processIds.TryGetValue(hashCode, out int processId))
            {
                return;
            }

            ProcessUtility.QuitProcessById(processId);
            _processIds.Remove(hashCode);
            e.Handled = true;
        }
    }
}
