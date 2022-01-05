using System.Text.RegularExpressions;

namespace Reginald.Core.Helpers
{
    public static class TimerKeywordHelper
    {
        public static bool TryGetTime(string input, string pattern, out double time, out int startingIndex, out int endingIndex)
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
    }
}
