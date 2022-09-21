namespace Reginald.Models.DataModels
{
    using System;
    using Newtonsoft.Json;
    using Reginald.Core.Extensions;
    using Reginald.Core.Services;
    using Reginald.Models.Inputs;
    using Reginald.Models.Producers;
    using Reginald.Models.Products;

    internal sealed class WebQuery : DataModel, ISingleProducer<SearchResult>
    {
        public const string FileName = "WebQueries.json";

        public const string UserFileName = "YourWebQueries.json";

        private string _keyInput;

        [JsonProperty("altDescription")]
        public string AltDescription { get; set; }

        [JsonProperty("altUrl")]
        public string AltUrl { get; set; }

        [JsonProperty("encodeInput")]
        public bool EncodeInput { get; set; }

        [JsonProperty("isEnabled")]
        public bool IsEnabled { get; set; }

        [JsonProperty("isCustom")]
        public bool IsCustom { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("placeholder")]
        public string Placeholder { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        public bool Check(string keyInput)
        {
            if (!IsEnabled || string.IsNullOrEmpty(Description))
            {
                return false;
            }

            _keyInput = keyInput;
            if (keyInput.Length <= Key.Length)
            {
                return Key.StartsWith(_keyInput, StringComparison.OrdinalIgnoreCase);
            }

            return Description.IndexOf("{0}") == -1 ? false : keyInput.StartsWith(Key + " ", StringComparison.OrdinalIgnoreCase);
        }

        public SearchResult Produce()
        {
            SearchResult result = new(Caption, IconPath, Id);
            result.AltAndEnterKeysPressed += OnAltAndEnterKeysPressed;
            result.AltKeyPressed += OnAltKeyPressed;
            result.AltKeyReleased += OnAltKeyReleased;
            result.EnterKeyPressed += OnEnterKeyPressed;
            result.TabKeyPressed += OnTabKeyPressed;

            // Handles cases where Description isn't formattable.
            if (Description.IndexOf("{0}") == -1)
            {
                result.Description = Description;
                return result;
            }

            if (_keyInput.Length <= Key.Length)
            {
                result.Description = string.Format(Description, Placeholder);
                return result;
            }

            string input = _keyInput.Split(' ', 2)[^1];
            result.Description = string.Format(Description, input == string.Empty ? Placeholder : input);
            return result;
        }

        public SearchResult Produce(string input)
        {
            _keyInput = Key + " " + input;
            return Produce();
        }

        private void OnAltAndEnterKeysPressed(object sender, InputProcessingEventArgs e)
        {
            // Handles cases where AltUrl isn't formattable.
            if (!AltUrl.Contains("{0}"))
            {
                ProcessService.GoTo(AltUrl);
                e.Handled = true;
                return;
            }

            string[] keyInputArray = _keyInput.Split(' ', 2);
            string input = keyInputArray[^1];
            if (keyInputArray.Length < 2 || input.Length == 0)
            {
                if (input == Key + " ")
                {
                    return;
                }

                // Handles autocompletion.
                e.IsInputIncomplete = true;
                e.CompleteInput = Key + " ";
                return;
            }

            e.Handled = true;
            ProcessService.GoTo(string.Format(AltUrl, input.Quote(EncodeInput)));
        }

        private void OnAltKeyPressed(object sender, InputProcessingEventArgs e)
        {
            if (sender is not SearchResult result || string.IsNullOrEmpty(AltDescription))
            {
                return;
            }

            // Handles cases where AltDescription isn't formattable.
            if (!AltDescription.Contains("{0}"))
            {
                result.Description = AltDescription;
                return;
            }

            if (_keyInput.Length <= Key.Length)
            {
                result.Description = string.Format(AltDescription, Placeholder);
            }

            string input = _keyInput.Split(' ', 2)[^1];
            result.Description = string.Format(AltDescription, input == string.Empty ? Placeholder : input);
        }

        private void OnAltKeyReleased(object sender, InputProcessingEventArgs e)
        {
            if (sender is not SearchResult result || string.IsNullOrEmpty(AltDescription))
            {
                return;
            }

            // Handles cases where Description isn't formattable.
            if (!Description.Contains("{0}"))
            {
                result.Description = Description;
                return;
            }

            string[] keyInputArray = _keyInput.Split(' ', 2);
            string input = keyInputArray[^1];
            result.Description = string.Format(Description, keyInputArray.Length < 2 || input == string.Empty ? Placeholder : input);
        }

        private void OnEnterKeyPressed(object sender, InputProcessingEventArgs e)
        {
            // Handles cases where Url isn't formattable.
            if (!Url.Contains("{0}"))
            {
                ProcessService.GoTo(Url);
                e.Handled = true;
                return;
            }

            string[] keyInputArray = _keyInput.Split(' ', 2);
            string input = keyInputArray[^1];
            if (keyInputArray.Length < 2 || input.Length == 0)
            {
                if (input == Key + " ")
                {
                    return;
                }

                // Handles autocompletion.
                e.IsInputIncomplete = true;
                e.CompleteInput = Key + " ";
                return;
            }

            e.Handled = true;
            ProcessService.GoTo(string.Format(Url, input.Quote(EncodeInput)));
        }

        private void OnTabKeyPressed(object sender, InputProcessingEventArgs e)
        {
            string[] keyInputArray = _keyInput.Split(' ', 2);
            string input = keyInputArray[^1];
            if (keyInputArray.Length == 2 && input.Length != 0)
            {
                return;
            }

            e.IsInputIncomplete = true;
            e.CompleteInput = Description.Contains("{0}") ? Key + " " : Key;
        }
    }
}
