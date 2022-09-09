namespace Reginald.Models.DataModels
{
    using System;
    using System.Threading;
    using System.Windows;
    using Newtonsoft.Json;
    using Reginald.Core.Math;
    using Reginald.Models.Inputs;
    using Reginald.Models.Producers;
    using Reginald.Models.Products;

    public class Calculator : DataModel, ISingleProducer<SearchResult>
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
            if (sender is not SearchResult result)
            {
                return;
            }

            if (!decimal.TryParse(Description, out decimal n))
            {
                return;
            }

            decimal integer = Math.Truncate(n);
            decimal mantissa = n - integer;
            if (mantissa == 0)
            {
                result.Description = integer.ToString("N0");
                return;
            }

            string decSep = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            string mantissaStr = mantissa.ToString();
            int decSepIndex = mantissaStr.IndexOf(decSep);
            result.Description = integer.ToString("N0") + (decSepIndex == -1 ? string.Empty : decSep + mantissaStr[(decSepIndex + 1)..]);
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
