namespace Reginald.Core.Helpers
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Reginald.Core.AbstractProducts;
    using Reginald.Core.Base;
    using Reginald.Core.Clients;
    using Reginald.Core.Factories;
    using Reginald.Extensions;

    public static class KeyphraseHelper
    {
        public static Task<IEnumerable<Keyphrase>> Filter(IEnumerable<Keyphrase> phrases, bool include, string input)
        {
            IEnumerable<Keyphrase> matches = Enumerable.Empty<Keyphrase>();
            if (include && !input.StartsWith(' '))
            {
                string cleanInput = input.RegexClean();
                string pattern = string.Format(CultureInfo.InvariantCulture, Constants.KeyphraseRegexFormat, cleanInput);
                Regex rx = new(pattern, RegexOptions.IgnoreCase);
                matches = phrases.Where(p => p.Predicate(p, rx, cleanInput));
            }

            return Task.FromResult(matches);
        }

        public static IEnumerable<DisplayItem> ToSearchResults(IEnumerable<Keyphrase> keywords)
        {
            ResultFactory factory = new();
            DisplayItemClient client = new(factory, keywords);
            return client.Items;
        }
    }
}
