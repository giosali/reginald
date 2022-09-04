﻿namespace Reginald.Models.DataModels
{
    using System;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Reginald.Models.Drawing;
    using Reginald.Models.Inputs;
    using Reginald.Models.Producers;
    using Reginald.Models.Products;
    using Reginald.Services.Utilities;

    public class Recycle : DataModel, ISingleProducer<SearchResult>
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
            _hasBeenPrompted = false;
            SearchResult result = new(Caption, IconPath, Description);
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