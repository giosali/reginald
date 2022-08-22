﻿namespace Reginald.Data.ObjectModels
{
    using System;
    using System.Text.RegularExpressions;
    using Reginald.Data.Producers;
    using Reginald.Data.Products;
    using Newtonsoft.Json;
    using Reginald.Core.Extensions;
    using System.Linq;
    using Reginald.Data.Inputs;
    using Reginald.Services.Notifications;

    public class Timer : ObjectModel, ICloneable, ISingleProducer<SearchResult>
    {
        private static readonly Regex[] _timePatterns = new Regex[3] { new Regex(@"(?<!\S)(\d+(\.\d+)?) ?h((ou)?rs?)?(?!\S)", RegexOptions.IgnoreCase), new Regex(@"(?<!\S)(\d+(\.\d+)?) ?m((in(ute)?s?)?)?(?!\S)", RegexOptions.IgnoreCase), new Regex(@"(?<!\S)(\d+(\.\d+)?) ?s((ec(ond)?s?)?)?(?!\S)", RegexOptions.IgnoreCase) };

        private string _keyInput;

        private string _message;

        private string _representation;

        private double _time;

        [JsonProperty("format")]
        public string Format { get; set; }

        public System.Timers.Timer InternalTimer { get; set; }

        [JsonProperty("isEnabled")]
        public bool IsEnabled { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("placeholder")]
        public string Placeholder { get; set; }

        public SearchResult Result { get; set; }

        public bool Check(string keyInput)
        {
            if (!IsEnabled)
            {
                return false;
            }

            _keyInput = keyInput;
            if (keyInput.Length <= Key.Length)
            {
                return Key.StartsWith(keyInput);
            }

            if (!keyInput.StartsWith(Key + " "))
            {
                return false;
            }

            // `pos` is used to prevent the following from being valid:
            // `Take out the trash 30m 30s Do the laundry`.
            // Otherwise, the description would only be set to "Do the laundry" and
            // "Take out the trash" would be completely ignored.
            // We need the time values to start at the very beginning of the input string.
            string input = keyInput.Split(' ', 2)[^1];
            int pos = 0;
            double totalTime = 0;
            string[] representations = new string[3];
            for (int i = 0, j = _timePatterns.Length - 1; i < _timePatterns.Length; i++, j--)
            {
                int start = 1;
                int end = 0;
                Match match = _timePatterns[i].Match(input);

                if (match.Success)
                {
                    start = match.Index;
                    end = match.Index + match.Length;
                }

                if (pos != start)
                {
                    continue;
                }

                _ = double.TryParse(match.Groups[1].Value, out double time);

                // Adds 1 to the position of the final character to account for the
                // space proceeding it.
                pos = end + 1;
                representations[i] = i switch
                {
                    0 => time.Quantify("hr"),
                    1 => time.Quantify("min"),
                    2 => time.Quantify("sec"),
                    _ => throw new ArgumentOutOfRangeException(),
                };
                totalTime += time * 1000 * Math.Pow(60, j);
            }

            _time = totalTime;
            if (totalTime == 0)
            {
                _message = _representation = Placeholder;
            }

            _message = pos > input.Length ? Placeholder : input[pos..];
            if (string.IsNullOrEmpty(_message))
            {
                _message = Placeholder;
            }

            _representation = string.Join(' ', representations.Where(r => !string.IsNullOrEmpty(r)));
            if (string.IsNullOrEmpty(_representation))
            {
                _representation = Placeholder;
            }

            // Returns false if the message is filled out before the time
            // representation.
            if (_representation == Placeholder && _message != Placeholder)
            {
                if (double.TryParse(input, out _))
                {
                    _message = Placeholder;
                    return true;
                }

                return false;
            }

            return true;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public SearchResult Produce()
        {
            SearchResult result = new(Caption, Icon);
            result.EnterKeyPressed += OnEnterKeyPressed;

            if (_keyInput.Split(' ', 2).Length < 2)
            {
                result.Description = string.Format(Format, Placeholder, Placeholder);
                return result;
            }

            result.Description = string.Format(Format, _representation, _message);
            return result;
        }

        private Timer DeepCopy()
        {
            Timer timer = (Timer)Clone();
            timer.InternalTimer = new(1000);
            timer.InternalTimer.Elapsed += timer.OnElapsed;
            timer.InternalTimer.AutoReset = true;
            timer.Result = new(Caption, Icon);
            timer.Result.AltKeyReleased += timer.OnAltKeyReleased;
            return timer;
        }

        private void OnAltKeyReleased(object sender, InputProcessingEventArgs e)
        {
            e.Caption = _message;
            e.Icon = Icon;
        }

        private void OnElapsed(object sender, EventArgs e)
        {
            _time -= 1000;
            if (_time <= 0)
            {
                InternalTimer.Stop();
                ToastNotification notification = new(Caption, _message);
                notification.Show();
            }

            TimeSpan ts = TimeSpan.FromMilliseconds(_time);
            Result.Description = string.Format(Timers.Format, ts.Hours, ts.Minutes, ts.Seconds);
        }

        private void OnEnterKeyPressed(object sender, InputProcessingEventArgs e)
        {
            if (_representation == Placeholder || _message == Placeholder)
            {
                return;
            }

            Timer timer = DeepCopy();
            timer.Result.Caption = _message;
            TimeSpan ts = TimeSpan.FromMilliseconds(_time);
            timer.Result.Description = string.Format(Timers.Format, ts.Hours, ts.Minutes, ts.Seconds);
            Timers.AddTimer(timer);

            // Prevents blank timers from being added by resetting
            // the time representation and associated message.
            _representation = _message = Placeholder;

            e.Handled = true;
        }
    }
}
