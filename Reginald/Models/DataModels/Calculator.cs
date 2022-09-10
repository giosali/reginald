namespace Reginald.Models.DataModels
{
    using System;
    using System.Threading;
    using System.Windows;
    using Caliburn.Micro;
    using Newtonsoft.Json;
    using Reginald.Core.Math;
    using Reginald.Models.Inputs;
    using Reginald.Models.Producers;
    using Reginald.Models.Products;
    using Reginald.Services;

    public class Calculator : DataModel, ISingleProducer<SearchResult>
    {
        private static readonly DataModelService _dms = IoC.Get<DataModelService>();

        [JsonProperty("isEnabled")]
        public bool IsEnabled { get; set; }

        public bool Check(string input)
        {
            if (!IsEnabled)
            {
                return false;
            }

            char decSep = _dms.Settings.DecimalSeparator;
            if (decSep == '\0')
            {
                decSep = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
            }

            if (!ShuntingYardAlgorithm.TryParse(input, decSep, out string result) && result is null)
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

            char decSep = _dms.Settings.DecimalSeparator;
            if (decSep == '\0')
            {
                decSep = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
            }

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
