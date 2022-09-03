namespace Reginald.Models.DataModels
{
    using System;
    using Newtonsoft.Json;
    using Reginald.Core.Extensions;
    using Reginald.Models.Inputs;
    using Reginald.Models.Producers;
    using Reginald.Models.Products;
    using Reginald.Services.Utilities;

    public class WebQuery : DataModel, ISingleProducer<SearchResult>
    {
        public static string FileName = "WebQueries.json";

        public static string UserFileName = "YourWebQueries.json";

        private string _keyInput;

        [JsonProperty("altDescription")]
        public string AltDescription { get; set; }

        [JsonProperty("altUrl")]
        public string AltUrl { get; set; }

        [JsonProperty("descriptionFormat")]
        public string DescriptionFormat { get; set; }

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

        [JsonProperty("urlFormat")]
        public string UrlFormat { get; set; }

        public bool Check(string keyInput)
        {
            if (!IsEnabled)
            {
                return false;
            }

            _keyInput = keyInput;
            if (!string.IsNullOrEmpty(Description))
            {
                if (_keyInput.Length < Key.Length)
                {
                    return Key.StartsWith(_keyInput, StringComparison.OrdinalIgnoreCase);
                }

                return _keyInput == Key;
            }

            if (_keyInput.Length <= Key.Length)
            {
                return Key.StartsWith(_keyInput, StringComparison.OrdinalIgnoreCase);
            }

            return _keyInput.StartsWith(Key + " ", StringComparison.OrdinalIgnoreCase);
        }

        public SearchResult Produce()
        {
            SearchResult result = new(Caption, IconPath);
            result.AltAndEnterKeysPressed += OnAltAndEnterKeysPressed;
            result.AltKeyPressed += OnAltKeyPressed;
            result.AltKeyReleased += OnAltKeyReleased;
            result.EnterKeyPressed += OnEnterKeyPressed;
            result.TabKeyPressed += OnTabKeyPressed;

            if (!string.IsNullOrEmpty(Description))
            {
                result.Description = Description;
                return result;
            }

            if (_keyInput.Length <= Key.Length)
            {
                result.Description = string.Format(DescriptionFormat, Placeholder);
                return result;
            }

            string input = _keyInput.Split(' ', 2)[^1];
            result.Description = string.Format(DescriptionFormat, input == string.Empty ? Placeholder : input);
            return result;
        }

        public SearchResult Produce(string input)
        {
            SearchResult result = new(Caption, IconPath);
            result.AltAndEnterKeysPressed += OnAltAndEnterKeysPressed;
            result.AltKeyPressed += OnAltKeyPressed;
            result.AltKeyReleased += OnAltKeyReleased;
            result.EnterKeyPressed += OnEnterKeyPressed;
            result.TabKeyPressed += OnTabKeyPressed;

            if (!string.IsNullOrEmpty(Description))
            {
                result.Description = Description;
                return result;
            }

            _keyInput = Key + " " + input;
            result.Description = string.Format(DescriptionFormat, input);
            return result;
        }

        private void OnAltAndEnterKeysPressed(object sender, InputProcessingEventArgs e)
        {
            if (string.IsNullOrEmpty(AltUrl))
            {
                return;
            }

            e.Handled = true;
            ProcessUtility.GoTo(AltUrl);
        }

        private void OnAltKeyPressed(object sender, InputProcessingEventArgs e)
        {
            if (sender is not SearchResult result)
            {
                return;
            }

            result.Description = AltDescription;
        }

        private void OnAltKeyReleased(object sender, InputProcessingEventArgs e)
        {
            if (sender is not SearchResult result)
            {
                return;
            }

            if (!string.IsNullOrEmpty(Description))
            {
                result.Description = Description;
                return;
            }

            string[] keyInputArray = _keyInput.Split(' ', 2);
            string input = keyInputArray[^1];
            result.Description = string.Format(DescriptionFormat, keyInputArray.Length < 2 || input == string.Empty ? Placeholder : input);
        }

        private void OnEnterKeyPressed(object sender, InputProcessingEventArgs e)
        {
            string[] keyInputArray = _keyInput.Split(' ', 2);
            string input = keyInputArray[^1];
            if (keyInputArray.Length < 2 || string.IsNullOrEmpty(input))
            {
                if (input == Key + " ")
                {
                    return;
                }

                // Handles autocompletion.
                e.IsInputIncomplete = true;
                e.CompleteInput = string.IsNullOrEmpty(Description) ? Key + " " : Key;
                return;
            }

            e.Handled = true;
            ProcessUtility.GoTo(string.IsNullOrEmpty(Url) ? string.Format(UrlFormat, input.Quote(EncodeInput)) : Url);
        }

        private void OnTabKeyPressed(object sender, InputProcessingEventArgs e)
        {
            string[] keyInputArray = _keyInput.Split(' ', 2);
            string input = keyInputArray[^1];

            if (keyInputArray.Length == 2 && input.Length != 0)
            {
                return;
            }

            bool isDescriptionNullOrEmpty = string.IsNullOrEmpty(Description);
            if (input == (isDescriptionNullOrEmpty ? Key + " " : Key))
            {
                return;
            }

            // Handles autocompletion.
            e.IsInputIncomplete = true;
            e.CompleteInput = isDescriptionNullOrEmpty ? Key + " " : Key;
        }
    }
}
