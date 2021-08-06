using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Reginald.Core.Utils
{
    public static class TimeUtils
    {
        /// <summary>
        /// Takes a unit (second, minute, or hour) and a time measurement and converts it to seconds.
        /// </summary>
        /// <param name="input">The unit of time.</param>
        /// <param name="time">The time measurement.</param>
        /// <returns>The time in seconds or null.</returns>
        /// <example>
        /// <code>
        /// string input = "minutes";
        /// double time = 5;
        /// double? timeInSeconds = await GetTimeAsSecondsAsync(input, time);
        /// Console.WriteLine(timeInSeconds == time * 60);
        /// </code>
        /// </example>
        public static async Task<double?> GetTimeAsSecondsAsync(string input, double time)
        {
            Task<bool> isSecondTask = IsSecond(input);
            Task<bool> isMinuteTask = IsMinute(input);
            Task<bool> isHourTask = IsHour(input);

            bool isSecond = await isSecondTask;
            bool isMinute = await isMinuteTask;
            bool isHour = await isHourTask;

            double twentyFourHours = 86400;

            if (isSecond)
            {
                if (!(time > twentyFourHours))
                {
                    return time;
                }
            }
            else if (isMinute)
            {
                time *= 60;
                if (!(time > twentyFourHours))
                {
                    return time;
                }
            }
            else if (isHour)
            {
                time *= (60 * 60);
                if (!(time > twentyFourHours))
                {
                    return time;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns whether or not the given unit is a second.
        /// </summary>
        /// <param name="input">The unit.</param>
        /// <returns>Whether or not the unit is a second.</returns>
        /// <example>
        /// <code>
        /// string unit = "seconds";
        /// bool isSecond = await IsSecond(unit);
        /// Console.WriteLine(isSecond == true);
        /// </code>
        /// </example>
        public static Task<bool> IsSecond(string input)
        {
            Regex rx = new(@"(?<!.)(s(?!.)|secs?(?!.)|seconds?(?!.))");
            return Task.FromResult(rx.IsMatch(input));
        }

        /// <summary>
        /// Returns whether or not the given unit is a minute.
        /// </summary>
        /// <param name="input">The unit.</param>
        /// <returns>Whether or not the unit is a minute.</returns>
        /// <example>
        /// <code>
        /// string unit = "minutes";
        /// bool isMinute = await IsMinute(unit);
        /// Console.WriteLine(isMinute == true);
        /// </code>
        /// </example>
        public static Task<bool> IsMinute(string input)
        {
            Regex rx = new(@"(?<!.)(m(?!.)|mins?(?!.)|minutes?(?!.))");
            return Task.FromResult(rx.IsMatch(input));
        }

        /// <summary>
        /// Returns whether or not the given unit is an hour.
        /// </summary>
        /// <param name="input">The unit.</param>
        /// <returns>Whether or not the unit is an hour.</returns>
        /// <example>
        /// <code>
        /// string unit = "hours";
        /// bool isHour = await IsHour(unit);
        /// Console.WriteLine(isHour == true);
        /// </code>
        /// </example>
        public static Task<bool> IsHour(string input)
        {
            Regex rx = new(@"(?<!.)(h(?!.)|hours?(?!.))");
            return Task.FromResult(rx.IsMatch(input));
        }

        /// <summary>
        /// Takes a unit of time and a measurement of time and returns the proper singular/plural version of that unit.
        /// </summary>
        /// <param name="word"></param>
        /// <param name="time"></param>
        /// <returns>A proper measurement of time.</returns>
        /// <example>
        /// <code>
        /// string unit = "mins";
        /// double time = "1";
        /// string properUnit = GetTimeUnit(unit, time);
        /// string expectedProperUnit = "minute";
        /// Console.WriteLine(expectedProperUnit == properUnit);
        /// </code>
        /// </example>
        public static string GetTimeUnit(string word, double time)
        {
            string unit = string.Empty;
            if (word.StartsWith("s", StringComparison.InvariantCultureIgnoreCase))
            {
                unit = time == 1 ? "second" : "seconds";
            }
            else if (word.StartsWith("m", StringComparison.InvariantCultureIgnoreCase))
            {
                unit = time == 1 ? "minute" : "minutes";
            }
            else if (word.StartsWith("h", StringComparison.InvariantCultureIgnoreCase))
            {
                unit = time == 1 ? "hour" : "hours";
            }

            return unit;
        }
    }
}
