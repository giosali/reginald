namespace Reginald.Data.ObjectModels
{
    using Reginald.Data.Producers;
    using Reginald.Data.Products;
    using Newtonsoft.Json;
    using Reginald.Data.Inputs;
    using Reginald.Services.Utilities;
    using System.Threading.Tasks;

    public class Recycle : ObjectModel, ISingleProducer<SearchResult>
    {
        [JsonProperty("isEnabled")]
        public bool IsEnabled { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("requiresPrompt")]
        public bool RequiresPrompt { get; set; }

        public bool Check(string input)
        {
            return IsEnabled && Key.Contains(input, System.StringComparison.OrdinalIgnoreCase);
        }

        public SearchResult Produce()
        {
            SearchResult result = new(Caption, Icon, Description);
            result.EnterKeyPressed += OnEnterKeyPressed;
            return result;
        }

        private async void OnEnterKeyPressed(object sender, InputProcessingEventArgs e)
        {
            await Task.Run(() => RecycleBin.Empty());
            e.Handled = true;
        }
    }
}