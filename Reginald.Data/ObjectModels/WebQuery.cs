namespace Reginald.Data.ObjectModels
{
    using Newtonsoft.Json;
    using Reginald.Core.Extensions;
    using Reginald.Data.Inputs;
    using Reginald.Data.Producers;
    using Reginald.Data.Products;
    using Reginald.Services.Utilities;

    public class WebQuery : ObjectModel, ISingleProducer<SearchResult>
    {
        private string _keyInput;

        [JsonProperty("altUrl")]
        public string AltUrl { get; set; }

        [JsonProperty("altDescription")]
        public string AltDescription { get; set; }

        [JsonProperty("descriptionFormat")]
        public string DescriptionFormat { get; set; }

        [JsonProperty("isEnabled")]
        public bool IsEnabled { get; set; }

        [JsonProperty("encodeInput")]
        public bool EncodeInput { get; set; }

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
            _keyInput = keyInput;
            if (!string.IsNullOrEmpty(Description))
            {
                if (_keyInput.Length < Key.Length)
                {
                    return Key.StartsWith(_keyInput);
                }

                return _keyInput == Key;
            }

            if (_keyInput.Length <= Key.Length)
            {
                return Key.StartsWith(_keyInput);
            }

            return _keyInput.StartsWith(Key + " ");
        }

        public SearchResult Produce()
        {
            if (!string.IsNullOrEmpty(Description))
            {
                SearchResult r = new(Caption, Description, Icon);
                r.AltKeyPressed += AltKeyPressed;
                r.AltKeyReleased += AltKeyReleased;
                r.EnterKeyPressed += EnterKeyPressed;
                return r;
            }

            if (_keyInput.Length <= Key.Length)
            {
                SearchResult r = new(Caption, string.Format(DescriptionFormat, Placeholder), Icon);
                r.AltKeyPressed += AltKeyPressed;
                r.AltKeyReleased += AltKeyReleased;
                r.EnterKeyPressed += EnterKeyPressed;
                return r;
            }

            string input = _keyInput.Split(' ', 2)[^1];
            SearchResult result = new SearchResult(Caption, string.Format(DescriptionFormat, input == string.Empty ? Placeholder : input), Icon);
            result.AltKeyPressed += AltKeyPressed;
            result.AltKeyReleased += AltKeyReleased;
            result.EnterKeyPressed += EnterKeyPressed;
            return result;
        }

        private void AltKeyPressed(object sender, InputProcessingEventArgs e)
        {
            e.IsAltKeyDown = true;
            e.Description = AltDescription;
        }

        private void AltKeyReleased(object sender, InputProcessingEventArgs e)
        {
            string[] keyInputArray = _keyInput.Split(' ', 2);
            string input = keyInputArray[^1];
            e.Description = string.Format(DescriptionFormat, keyInputArray.Length < 2 || input == string.Empty ? Placeholder : input);
        }

        private void EnterKeyPressed(object sender, InputProcessingEventArgs e)
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
    }
}
