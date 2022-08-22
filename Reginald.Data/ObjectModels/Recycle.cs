namespace Reginald.Data.ObjectModels
{
    using Reginald.Data.Producers;
    using Reginald.Data.Products;
    using Newtonsoft.Json;
    using Reginald.Data.Inputs;
    using Reginald.Services.Utilities;
    using System;
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
            if (!IsEnabled)
            {
                return false;
            }

            int index = Key.IndexOf(input, StringComparison.OrdinalIgnoreCase);
            if (index == -1)
            {
                return false;
            }

            // Returns false
            // if the index isn't the beginning of the Key
            // and if the character before the index isn't a space.
            if (index != 0 && Key[index - 1] != ' ')
            {
                return false;
            }

            return true;
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