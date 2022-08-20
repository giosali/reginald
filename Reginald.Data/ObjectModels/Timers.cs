namespace Reginald.Data.ObjectModels
{
    using Reginald.Data.Producers;
    using Reginald.Data.Products;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System;
    using System.Linq;
    using Reginald.Services.Notifications;
    using Reginald.Data.Inputs;

    public class Timers : ObjectModel, IMultipleProducer<SearchResult>
    {
        private static readonly List<MyTimer> _timers = new();

        private static string _caption;

        private static string _icon;

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

        public static void AddTimer(double milliseconds, string caption, string icon, string header)
        {
            if (string.IsNullOrEmpty(_caption))
            {
                _caption = caption;
            }

            if (string.IsNullOrEmpty(_icon))
            {
                _icon = icon;
            }

            TimeSpan ts = TimeSpan.FromMilliseconds(milliseconds);
            SearchResult result = new(caption, string.Format(Format, ts.Hours, ts.Minutes, ts.Seconds), icon);
            result.AltKeyPressed += OnAltKeyPressed;
            result.AltKeyReleased += OnAltKeyReleased;
            _timers.Add(new MyTimer(result, milliseconds, header));
        }

        private static void OnAltKeyPressed(object sender, InputProcessingEventArgs e)
        {
            e.Caption = AltCaption;
            e.Icon = AltIcon;
        }

        private static void OnAltKeyReleased(object sender, InputProcessingEventArgs e)
        {
            e.Caption = _caption;
            e.Icon = _icon;
        }

        public bool Check(string input)
        {
            return IsEnabled && Key.StartsWith(input);
        }

        public SearchResult[] Produce()
        {
            return _timers.Select(t => t.Result).ToArray();
        }

        private class MyTimer
        {
            private string _header;

            private double _time;

            private System.Timers.Timer _timer;

            public MyTimer(SearchResult result, double milliseconds, string header)
            {
                Result = result;
                _time = milliseconds;
                _header = header;

                _timer = new(1000);
                _timer.Elapsed += OnTimeElapsed;
                _timer.AutoReset = true;
                _timer.Start();
            }

            public SearchResult Result { get; set; }

            private void OnTimeElapsed(object sender, EventArgs e)
            {
                _time -= 1000;
                if (_time <= 0)
                {
                    _timer.Stop();
                    _timers.Remove(this);
                    ToastNotification notification = new(_header, Result.Caption);
                    notification.Show();
                }

                TimeSpan ts = TimeSpan.FromMilliseconds(_time);
                Result.Description = string.Format(Format, ts.Hours, ts.Minutes, ts.Seconds);
            }
        }
    }
}