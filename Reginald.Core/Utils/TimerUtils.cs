using Reginald.Extensions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Reginald.Core.Utils
{
    public static class TimerUtils
    {
        /// <summary>
        /// Executes a task every X seconds.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns></returns>
        public static async Task DoEveryXSecondsAsync(double? time, CancellationToken cancellationToken)
        {
            int millisecondsDelay = (int)time;
            while (true)
            {
                await Task.Delay(millisecondsDelay, cancellationToken);
            }
        }

        //public static async Task<(string, double?)> ParseTimeFromStringAsync(string expression, string format, string split, string defaultText)
        //{
        //    int minSplit = int.Parse(split);
        //    string description = null;
        //    double? seconds = null;
        //    string[] substrings = expression.Split(' ', minSplit);
        //    if (substrings.Length > 1)
        //    {
        //        string time = substrings[0];
        //        string remainder = substrings[^1];
        //        if (double.TryParse(time, out double result))
        //        {
        //            string firstWord = remainder.FirstWord(out string rest);
        //            seconds = await TimeUtils.GetTimeAsSecondsAsync(result, firstWord);
        //            string unit;
        //            string text;
        //            if (seconds is null)
        //            {
        //                seconds = result;
        //                unit = TimeUtils.GetTimeUnit("s", result);
        //                text = remainder;
        //            }
        //            else
        //            {
        //                unit = TimeUtils.GetTimeUnit(firstWord, result);
        //                text = rest;
        //            }
        //            description = string.Format(format, time, unit, text);
        //        }
        //    }
        //    else if (substrings.Length == 1)
        //    {
        //        string time = substrings[0];
        //        if (double.TryParse(time, out double result))
        //        {
        //            string unit = TimeUtils.GetTimeUnit("s", result);
        //            description = string.Format(format, time, unit, defaultText);
        //        }
        //        seconds = result;
        //    }
        //    else
        //    {
        //        description = string.Format(format, defaultText, defaultText, defaultText);
        //    }
        //    return (description, seconds);
        //}

        /// <summary>
        /// Parses a string potentially containing a measure of time, a unit of time, and other text and returns a tuple consisting of the time and other text.
        /// </summary>
        /// <param name="expression">The primary input containing the time measurement.</param>
        /// <param name="format">The format that the other text will be inserted in.</param>
        /// <param name="splitText">The number of format options.</param>
        /// <param name="defaultText">Placeholder text for the formatted string.</param>
        /// <returns>A tuple consisting of a description and the parsed time in seconds.</returns>
        /// <example>
        /// <code>
        /// string expression = "5 minutes Take out the trash";
        /// string format = "In {0} {1}: {2}";
        /// string split = "3";
        /// string defaultText = "...";
        /// (string description, double? time) = await ParseTimeFromStringAsync(expression, format, split, defaultText);
        /// </code>
        /// </example>
        public static async Task<(string, double?)> ParseTimeFromStringAsync(string expression, string format, string splitText, string defaultText)
        {
            int split = int.Parse(splitText);
            int count = 0;
            string[] formatArgs = Enumerable.Repeat(defaultText, split).ToArray();
            string description = null;
            double? seconds = null;

            if (string.IsNullOrEmpty(expression))
            {
                description = string.Format(format, formatArgs);
            }
            else
            {
                (string time, string timeSeparator, string timeRemainder) = expression.Partition(" ");
                //string time = expression.FirstWord(out string timeRemainder);
                if (double.TryParse(time, out double timeValue))
                {
                    formatArgs[count++] = time;
                    (string unit, string unitSeparator, string unitRemainder) = timeRemainder.Partition(" ");
                    //string unit = timeRemainder.FirstWord(out string unitRemainder);
                    string timerDescription;
                    seconds = await TimeUtils.GetTimeAsSecondsAsync(unit, timeValue);
                    if (seconds is not null)
                    {
                        unit = TimeUtils.GetTimeUnit(unit, timeValue);
                        timerDescription = string.IsNullOrEmpty(unitSeparator) ? defaultText : unitRemainder;
                        //timerDescription = unitRemainder;
                    }
                    else
                    {
                        unit = TimeUtils.GetTimeUnit("s", timeValue);
                        timerDescription = timeRemainder;
                        seconds = timeValue;
                    }
                    formatArgs[count++] = unit;
                    if (!string.IsNullOrEmpty(timeSeparator))
                    {
                        formatArgs[count++] = timerDescription;
                    }
                    description = string.Format(format, formatArgs);
                }
            }
            return (description, seconds);
        }
    }
}
