namespace Reginald.Data.ObjectModels
{
    using Reginald.Data.Producers;
    using Reginald.Data.Products;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System;
    using System.Linq;
    using Reginald.Services.Notifications;

    public class Timers : ObjectModel, IMultipleProducer<SearchResult>
    {
        private static readonly List<MyTimer> _timers = new();

        private const string Format = "{0}:{1:D2}:{2:D2}";

        [JsonProperty("isEnabled")]
        public bool IsEnabled { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        public static void AddTimer(double milliseconds, string caption, string icon, string header)
        {
            _timers.Add(new MyTimer(milliseconds, new SearchResult(caption, icon), header));
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

            public MyTimer(double milliseconds, SearchResult result, string header)
            {
                _time = milliseconds;

                Result = result;
                TimeSpan ts = TimeSpan.FromMilliseconds(milliseconds);
                result.Description = string.Format(Format, ts.Hours, ts.Minutes, ts.Seconds);

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
                    _timers.Remove(this);
                    _timer.Stop();
                    ToastNotification notification = new(_header, Result.Caption);
                    notification.Show();
                }

                TimeSpan ts = TimeSpan.FromMilliseconds(_time);
                Result.Description = string.Format(Format, ts.Hours, ts.Minutes, ts.Seconds);
            }
        }
    }
}