namespace Reginald.Data.Keyphrases
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Reginald.Core.Extensions;

    public abstract partial class Keyphrase
    {
        public static Task<IEnumerable<Keyphrase>> Filter(IEnumerable<Keyphrase> phrases, bool include, string input)
        {
            IEnumerable<Keyphrase> matches = Enumerable.Empty<Keyphrase>();
            if (include && !input.StartsWith(' '))
            {
                string cleanInput = input.RegexClean();
                string pattern = string.Format(CultureInfo.InvariantCulture, Keyphrase.KeyphraseRegexFormat, cleanInput);
                Regex rx = new(pattern, RegexOptions.IgnoreCase);
                matches = phrases.Where(p => p.Predicate(p, rx, cleanInput))
                                 .Take(20);
            }

            return Task.FromResult(matches);
        }
    }
}
