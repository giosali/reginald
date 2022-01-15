namespace Reginald.Core.Helpers
{
    using System;
    using System.Text.RegularExpressions;

    public static class TimerKeywordHelper
    {
        /// <summary>
        /// Converts the string representation of a time representation to its double-precision floating-point number equivalent. A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="input">The string containing a time representation.</param>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <param name="time">Contains the double-precision floating-point number equivalent of <paramref name="input"/> if the conversion succeeded, or 0 if the conversion failed.</param>
        /// <param name="startingIndex">Contains the 32-bit signed integer value of the starting position of the regular expression pattern if the match was successful, or 1 if the match was unsuccessful.</param>
        /// <param name="endingIndex">Contains the 32-bit signed integer value of the ending position of the regular expression pattern if the match was successful, or 0 if the match was unsuccessful.</param>
        /// <returns><see langword="true"/> if <paramref name="input"/> was converted successfully; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetTime(string input, string pattern, out double time, out int startingIndex, out int endingIndex)
        {
            try
            {
                Regex rx = new(pattern, RegexOptions.IgnoreCase);
                Match match = rx.Match(input);
                if (match.Success)
                {
                    time = double.Parse(match.Groups[1].Value);
                    startingIndex = match.Index;
                    endingIndex = match.Index + match.Length;
                }
                else
                {
                    time = 0;
                    startingIndex = 1;
                    endingIndex = 0;
                }

                return match.Success;
            }
            catch (ArgumentNullException)
            {
                time = 0;
                startingIndex = 1;
                endingIndex = 0;
                return false;
            }
        }
    }
}
