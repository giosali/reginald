namespace Reginald.Models.DataModels
{
    using Caliburn.Micro;
    using Newtonsoft.Json;
    using Reginald.Core.Extensions;
    using Reginald.Models.Inputs;
    using Reginald.Models.Producers;
    using Reginald.Models.Products;
    using Reginald.ViewModels;

    internal class ClearClipboard : DataModel, ISingleProducer<SearchResult>
    {
        [JsonProperty("isEnabled")]
        public bool IsEnabled { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        public bool Check(string input)
        {
            if (!IsEnabled)
            {
                return false;
            }

            return Key.ContainsPhrase(input);
        }

        public SearchResult Produce()
        {
            SearchResult result = new(Caption, IconPath, Description);
            result.EnterKeyPressed += OnEnterKeyPressed;
            return result;
        }

        private void OnEnterKeyPressed(object sender, InputProcessingEventArgs e)
        {
            e.Handled = true;
            IoC.Get<ClipboardManagerPopupViewModel>().Clear();
        }
    }
}
