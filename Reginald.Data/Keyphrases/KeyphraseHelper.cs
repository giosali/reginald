namespace Reginald.Data.Keyphrases
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Reginald.Core.Extensions;
    using Reginald.Data.Base;
    using Reginald.Data.DisplayItems;

    public static class KeyphraseHelper
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

        public static IEnumerable<DisplayItem> ToSearchResults(IEnumerable<Keyphrase> keywords)
        {
            ResultFactory factory = new();
            DisplayItemClient client = new(factory, keywords);
            return client.Items;
        }

        public static IEnumerable<Keyphrase> ToKeyphrases(IEnumerable<KeyphraseDataModelBase> models)
        {
            SearchTermFactory factory = new();
            KeyphraseClient client = new(factory, models);
            return client.Keyphrases;
        }
    }
}
