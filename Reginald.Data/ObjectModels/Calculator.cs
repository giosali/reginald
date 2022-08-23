namespace Reginald.Data.ObjectModels
{
    using Reginald.Data.Producers;
    using Reginald.Data.Products;
    using Newtonsoft.Json;
    using Reginald.Core.Math;
    using Reginald.Data.Inputs;
    using System.Windows;

    public class Calculator : ObjectModel, ISingleProducer<SearchResult>
    {
        [JsonProperty("isEnabled")]
        public bool IsEnabled { get; set; }

        public bool Check(string input)
        {
            if (!IsEnabled)
            {
                return false;
            }

            if (!ShuntingYardAlgorithm.TryParse(input, out string result) && result is null)
            {
                return false;
            }

            Description = result;
            return true;
        }

        public SearchResult Produce()
        {
            SearchResult result = new(Caption, Icon, Description);
            result.AltKeyPressed += OnAltKeyPressed;
            result.AltKeyReleased += OnAltKeyReleased;
            result.EnterKeyPressed += OnEnterKeyPressed;
            return result;
        }

        private void OnAltKeyPressed(object sender, InputProcessingEventArgs e)
        {
            if (!double.TryParse(Description, out double n))
            {
                return;
            }

            SearchResult result = (SearchResult)sender;
            result.Description = n.ToString("N0");
        }

        private void OnAltKeyReleased(object sender, InputProcessingEventArgs e)
        {
            SearchResult result = (SearchResult)sender;
            result.Description = Description;
        }

        private void OnEnterKeyPressed(object sender, InputProcessingEventArgs e)
        {
            if (Description is not null)
            {
                Clipboard.SetText(Description);
            }

            e.Handled = true;
        }
    }
}