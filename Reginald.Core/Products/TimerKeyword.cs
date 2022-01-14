namespace Reginald.Core.Products
{
    using System;
    using System.Linq;
    using Reginald.Core.Base;
    using Reginald.Core.Extensions;
    using Reginald.Core.Helpers;

    /// <summary>
    /// Specifies a unit of time.
    /// </summary>
    public enum TimeUnit
    {
        /// <summary>
        /// A unit of time specifying seconds.
        /// </summary>
        Second,

        /// <summary>
        /// A unit of time specifying minutes.
        /// </summary>
        Minute,

        /// <summary>
        /// A unit of time specifying hours.
        /// </summary>
        Hour,
    }

    public class TimerKeyword : CommandKeyword
    {
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
        /// Takes a string and returns a boolean indicating whether or not it contains time values and/or a description.
        /// </summary>
        /// <param name="input">The input to evaluate.</param>
        /// <returns><see langword="true"/> if the input contains time values and/or a description; otherwise, <see langword="false"/>.</returns>
        public bool TryParseTimeFromString(string input)
        {
            double time = 0;

            // The startingIndex is used to prevent the following from being valid:
            // `Take out the trash 30m 30s Do the laundry`
            // Otherwise, the description would only be set to "Do the laundry" and
            // "Take out the trash" would be completely ignored
            // We need the time values to start at the very beginning of the input string
            int startingIndex = 1;
            int largestIndex = 0;
            string[] timeRepresentations = new string[3];
            if (TimerKeywordHelper.TryGetTime(input, Constants.CommandTimerSecondRegexPattern, out double seconds, out int secondsStartingIndex, out int secondsEndingIndex))
            {
                time += seconds.ToMilliseconds(TimeUnit.Second, out timeRepresentations[2]);
                if (secondsStartingIndex < startingIndex)
                {
                    startingIndex = secondsStartingIndex;
                }

                if (secondsEndingIndex > largestIndex)
                {
                    largestIndex = secondsEndingIndex;
                }
            }

            if (TimerKeywordHelper.TryGetTime(input, Constants.CommandTimerMinuteRegexPattern, out double minutes, out int minutesStartingIndex, out int minutesEndingIndex))
            {
                time += minutes.ToMilliseconds(TimeUnit.Minute, out timeRepresentations[1]);
                if (minutesStartingIndex < startingIndex)
                {
                    startingIndex = minutesStartingIndex;
                }

                if (minutesEndingIndex > largestIndex)
                {
                    largestIndex = minutesEndingIndex;
                }
            }

            if (TimerKeywordHelper.TryGetTime(input, Constants.CommandTimerHourRegexPattern, out double hours, out int hoursStartingIndex, out int hoursEndingIndex))
            {
                time += hours.ToMilliseconds(TimeUnit.Hour, out timeRepresentations[0]);
                if (hoursStartingIndex < startingIndex)
                {
                    startingIndex = hoursStartingIndex;
                }

                if (hoursEndingIndex > largestIndex)
                {
                    largestIndex = hoursEndingIndex;
                }
            }

            if (time > 0 && startingIndex == 0)
            {
                // We add 1 to the index of the final character to account for the space proceeding it
                int titleIndex = largestIndex + 1;
                string title = titleIndex > input.Length ? Placeholder : input[titleIndex..];
                if (string.IsNullOrEmpty(title))
                {
                    title = Placeholder;
                }

                string representation = string.Join(' ', timeRepresentations.Where(r => !string.IsNullOrEmpty(r)));
                Completion = title;
                Description = string.Format(Format, representation, title);
                Time = time;
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
