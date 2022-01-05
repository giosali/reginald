using Reginald.Core.Base;
using Reginald.Core.Enums;
using Reginald.Core.Helpers;
using Reginald.Core.Products;
using System.Linq;

namespace Reginald.Core.Extensions
{
    public static class TimerKeywordExtensions
    {
        /// <summary>
        /// Takes a string and returns a boolean indicating whether or not it contains time values and/or a description.
        /// </summary>
        /// <param name="timer">A TimerModel object</param>
        /// <param name="input"></param>
        /// <returns><see langword="true"/> if the input contains time values and/or a description; otherwise, <see langword="false"/></returns>
        public static bool TryParseTimeFromString(this TimerKeyword timer, string input)
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
                time += seconds.ToMilliseconds(Unit.Second, out timeRepresentations[2]);
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
                time += minutes.ToMilliseconds(Unit.Minute, out timeRepresentations[1]);
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
                time += hours.ToMilliseconds(Unit.Hour, out timeRepresentations[0]);
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
                string title = titleIndex > input.Length ? timer.Placeholder : input[titleIndex..];
                if (string.IsNullOrEmpty(title))
                {
                    title = timer.Placeholder;
                }

                string representation = string.Join(' ', timeRepresentations.Where(r => !string.IsNullOrEmpty(r)));
                timer.Completion = title;
                timer.Description = string.Format(timer.Format, representation, title);
                timer.Time = time;
                return true;
            }
            else
            {
                timer.Description = null;
                return false;
            }
        }
    }
}
