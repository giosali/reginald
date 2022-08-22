namespace Reginald.Data.ObjectModels
{
    using Reginald.Data.Producers;
    using Reginald.Data.Products;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Reginald.Data.Inputs;

    public class Timers : ObjectModel, IMultipleProducer<SearchResult>
    {
        private static readonly List<Timer> _timers = new();

        [JsonProperty("altCaption")]
        public static string AltCaption { get; set; }

        [JsonProperty("altIcon")]
        public static string AltIcon { get; set; }

        [JsonProperty("format")]
        public static string Format { get; set; }

        [JsonProperty("isEnabled")]
        public bool IsEnabled { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        public static void AddTimer(Timer timer)
        {
            timer.InternalTimer.Elapsed += OnElapsed;
            timer.InternalTimer.Start();
            timer.Result.AltAndEnterKeysPressed += OnAltAndEnterKeysPressed;
            timer.Result.AltKeyPressed += OnAltKeyPressed;
            _timers.Add(timer);
        }

        private static void OnAltAndEnterKeysPressed(object sender, InputProcessingEventArgs e)
        {
            _timers.Remove(_timers.Single(t => t.Result.GetHashCode() == e.HashCode));
            e.Remove = true;
        }

        private static void OnAltKeyPressed(object sender, InputProcessingEventArgs e)
        {
            e.Caption = AltCaption;
            e.Icon = AltIcon;
        }

        private static void OnElapsed(object sender, EventArgs e)
        {
            if (((System.Timers.Timer)sender).Enabled)
            {
                return;
            }

            Timer timer = _timers.SingleOrDefault(t => t.InternalTimer.GetHashCode() == sender.GetHashCode());
            if (timer is null)
            {
                return;
            }

            _timers.Remove(timer);
        }

        public bool Check(string input)
        {
            return IsEnabled && Key.StartsWith(input);
        }

        public SearchResult[] Produce()
        {
            return _timers.Select(t => t.Result).ToArray();
        }
    }
}
