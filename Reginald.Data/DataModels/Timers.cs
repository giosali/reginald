﻿namespace Reginald.Data.DataModels
{
    using Reginald.Data.Producers;
    using Reginald.Data.Products;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Reginald.Data.Inputs;
    using Reginald.Data.Drawing;

    public class Timers : DataModel, IMultipleProducer<SearchResult>
    {
        private static readonly List<Timer> _timers = new();

        [JsonProperty("altCaption")]
        public static string AltCaption { get; set; }

        [JsonProperty("altIconPath")]
        public static string AltIconPath { get; set; }

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

        public bool Check(string input)
        {
            return IsEnabled && Key.StartsWith(input, StringComparison.OrdinalIgnoreCase);
        }

        public SearchResult[] Produce()
        {
            return _timers.Select(t => t.Result).ToArray();
        }

        private static void OnAltAndEnterKeysPressed(object sender, InputProcessingEventArgs e)
        {
            if (sender is not SearchResult result)
            {
                return;
            }

            Timer timer = _timers.SingleOrDefault(t => t.Result == result);
            timer.InternalTimer.Enabled = false;
            _timers.Remove(timer);
            e.Remove = true;
        }

        private static void OnAltKeyPressed(object sender, InputProcessingEventArgs e)
        {
            if (sender is not SearchResult result)
            {
                return;
            }

            result.Caption = AltCaption;
            result.Icon = new Icon(AltIconPath);
        }

        private static void OnElapsed(object sender, EventArgs e)
        {
            System.Timers.Timer timer = (System.Timers.Timer)sender;
            if (timer.Enabled)
            {
                return;
            }

            _timers.Remove(_timers.SingleOrDefault(t => t.InternalTimer == timer));
        }
    }
}