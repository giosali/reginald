namespace Reginald.Models.DataModels
{
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Reginald.Core.Extensions;
    using Reginald.Core.IO;
    using Reginald.Models.Drawing;
    using Reginald.Models.Inputs;
    using Reginald.Models.Producers;
    using Reginald.Models.Products;

    internal sealed class Recycle : DataModel, ISingleProducer<SearchResult>
    {
        private bool _hasBeenPrompted;

        [JsonProperty("enterCaption")]
        public string EnterCaption { get; set; }

        [JsonProperty("enterIconPath")]
        public string EnterIconPath { get; set; }

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
            _hasBeenPrompted = false;
            SearchResult result = new(Caption, IconPath, Description, Id);
            result.EnterKeyPressed += OnEnterKeyPressed;
            return result;
        }

        private async void OnEnterKeyPressed(object sender, InputProcessingEventArgs e)
        {
            if (!_hasBeenPrompted)
            {
                _hasBeenPrompted = true;
                if (sender is not SearchResult result)
                {
                    return;
                }

                result.Caption = EnterCaption;
                result.Icon = new Icon(EnterIconPath);
                return;
            }

            e.Handled = true;
            await Task.Run(() => RecycleBin.Empty());
        }
    }
}
