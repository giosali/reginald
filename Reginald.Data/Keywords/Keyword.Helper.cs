namespace Reginald.Data.Keywords
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Reginald.Core.Extensions;

    public abstract partial class Keyword
    {
        public static Task<IEnumerable<Keyword>> Filter(IEnumerable<Keyword> keywords, bool include, string input)
        {
            IEnumerable<Keyword> matches;
            if (include)
            {
                (string Keyword, string Separator, string Description) partition = input.Partition(" ");
                string cleanInput = partition.Keyword.RegexClean();
                string pattern = string.Format(CultureInfo.InvariantCulture, KeywordRegexFormat, cleanInput);
                Regex rx = new(pattern, RegexOptions.IgnoreCase);
                matches = keywords.Where(k => k.Predicate(rx, partition));
            }
            else
            {
                matches = Enumerable.Empty<Keyword>();
            }

            return Task.FromResult(matches);
        }

        public static Task<IEnumerable<Keyword>> Set(IEnumerable<Keyword> keywords, string input)
        {
            foreach (Keyword keyword in keywords)
            {
                keyword.Description = string.Format(keyword.Format, input);
                keyword.Completion = input;
            }

            return Task.FromResult(keywords);
        }
    }
}
