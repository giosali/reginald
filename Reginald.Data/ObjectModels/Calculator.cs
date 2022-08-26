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
            SearchResult result = new(Caption, IconPath, Description);
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

            if (sender is not SearchResult result)
            {
                return;
            }

            string withCommas = n.ToString("N0");
            int index = Description.IndexOf('.');
            if (index != -1)
            {
                withCommas += Description[(index - 1)..];
            }

            result.Description = withCommas;
        }

        private void OnAltKeyReleased(object sender, InputProcessingEventArgs e)
        {
            if (sender is not SearchResult result)
            {
                return;
            }

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