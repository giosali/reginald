namespace Reginald.Data.ObjectModels
{
    using System;
    using Newtonsoft.Json;
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
                return new SearchResult(Caption, Description, Icon);
            }

            if (_keyInput.Length <= Key.Length)
            {
                return new SearchResult(Caption, string.Format(DescriptionFormat, Placeholder), Icon);
            }

            string input = _keyInput.Split(' ', 2)[^1];
            return new SearchResult(Caption, string.Format(DescriptionFormat, input == string.Empty ? Placeholder : input), Icon);
        }

        private void EnterKeyPressed(object sender, EventArgs e)
        {
            string[] keyInputArray = _keyInput.Split(' ', 2);
            string input = keyInputArray[^1];
            if (keyInputArray.Length < 2 || string.IsNullOrEmpty(input))
            {
                return;
            }

            ProcessUtility.GoTo(string.IsNullOrEmpty(Url) ? string.Format(UrlFormat, input) : Url);
        }
    }
}
