using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Reginald.Core.Utils
{
    public static class TimeUtils
    {
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

        public static Task<bool> IsSecond(string input)
        {
            Regex rx = new(@"(?<!.)(s(?!.)|secs?(?!.)|seconds?(?!.))");
            return Task.FromResult(rx.IsMatch(input));
        }

        public static Task<bool> IsMinute(string input)
        {
            Regex rx = new(@"(?<!.)(m(?!.)|mins?(?!.)|minutes?(?!.))");
            return Task.FromResult(rx.IsMatch(input));
        }

        public static Task<bool> IsHour(string input)
        {
            Regex rx = new(@"(?<!.)(h(?!.)|hours?(?!.))");
            return Task.FromResult(rx.IsMatch(input));
        }

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
