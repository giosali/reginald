namespace Reginald.Core.Products
{
    using System;
    using System.Linq;
    using Reginald.Core.Extensions;
    using Reginald.Core.Helpers;

    /// <summary>
    /// Specifies a unit of time.
    /// </summary>
    public enum TimeUnit
    {
        /// <summary>
        /// A unit of time specifying hours.
        /// </summary>
        Hour,

        /// <summary>
        /// A unit of time specifying minutes.
        /// </summary>
        Minute,

        /// <summary>
        /// A unit of time specifying seconds.
        /// </summary>
        Second,
    }

    public class TimerKeyword : CommandKeyword
    {
        private const string SecondRegexPattern = @"(?<!\S)(\d+(\.\d+)?) ?s((ec(ond)?s?)?)?(?!\S)";

        private const string MinuteRegexPattern = @"(?<!\S)(\d+(\.\d+)?) ?m((in(ute)?s?)?)?(?!\S)";

        private const string HourRegexPattern = @"(?<!\S)(\d+(\.\d+)?) ?h((ou)?rs?)?(?!\S)";

        private double _time;

        private bool _isRunning;

        public TimerKeyword()
        {
        }

        public TimerKeyword(CommandKeyword keyword)
        {
            Guid = keyword.Guid;
            Name = keyword.Name;
            Word = keyword.Word;
            Icon = keyword.Icon;
            Format = keyword.Format;
            Placeholder = keyword.Placeholder;
            Caption = keyword.Caption;
            IsEnabled = keyword.IsEnabled;
            Command = keyword.Command;
        }

        public double Time
        {
            get => _time;
            set
            {
                _time = value;
                NotifyOfPropertyChange(() => Time);
            }
        }

        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                _isRunning = value;
                NotifyOfPropertyChange(() => IsRunning);
            }
        }

        public override void EnterDown(bool isAltDown, Action action)
        {
            if (!string.IsNullOrEmpty(Completion) && Time > 0)
            {
                action();
                IsRunning = true;
            }
        }

        /// <summary>
        /// Takes a string and returns a boolean indicating whether or not it contains a time representation and/or a description.
        /// </summary>
        /// <param name="input">The input to evaluate.</param>
        /// <returns><see langword="true"/> if the input contains a time representation and/or a description; otherwise, <see langword="false"/>.</returns>
        public bool TryParseTimeFromString(string input)
        {
            double totalTime = 0;

            //// The currentIndex is used to prevent the following from being valid:
            //// `Take out the trash 30m 30s Do the laundry`
            //// Otherwise, the description would only be set to "Do the laundry" and
            //// "Take out the trash" would be completely ignored
            //// We need the time values to start at the very beginning of the input string
            int currentIndex = 0;
            int span = 0;
            string[] timeRepresentations = new string[3];
            string[] patterns = new string[3] { HourRegexPattern, MinuteRegexPattern, SecondRegexPattern };
            for (int i = 0; i < patterns.Length; i++)
            {
                if (TimerKeywordHelper.TryGetTime(input, patterns[i], out double time, out int start, out int end))
                {
                    if (start == currentIndex)
                    {
                        currentIndex = end + 1;

                        // We add 1 to the index of the final character to account for the space proceeding it
                        span = end + 1;
                        totalTime += time.ToMilliseconds((TimeUnit)i, out timeRepresentations[i]);
                    }
                }
            }

            if (totalTime > 0)
            {
                string title = span > input.Length ? Placeholder : input[span..];
                if (string.IsNullOrEmpty(title))
                {
                    title = Placeholder;
                }

                string representation = string.Join(' ', timeRepresentations.Where(r => !string.IsNullOrEmpty(r)));
                Completion = title;
                Description = string.Format(Format, representation, title);
                Time = totalTime;
                return true;
            }
            else
            {
                Description = null;
                return false;
            }
        }
    }
}
