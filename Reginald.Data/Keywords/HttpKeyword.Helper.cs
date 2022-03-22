namespace Reginald.Data.Keywords
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Reginald.Core.Extensions;

    public partial class HttpKeyword
    {
        public static async Task<IEnumerable<Keyword>> FilterAsync(IEnumerable<Keyword> keywords, bool include, string input)
        {
            IEnumerable<Keyword> matches;
            if (include && !input.StartsWith(' '))
            {
                Source.Cancel();
                Source = new();

                (string Keyword, string Separator, string Description) partition = input.Partition(" ");
                string cleanInput = partition.Keyword.RegexClean();
                string pattern = string.Format(CultureInfo.InvariantCulture, PreciseKeywordRegexFormat, cleanInput);
                Regex rx = new(pattern, RegexOptions.IgnoreCase);

                List<Keyword> filteredKeywords = new(keywords.Count());
                foreach (Keyword keyword in keywords)
                {
                    if (await keyword.PredicateAsync(rx, partition, Source.Token))
                    {
                        filteredKeywords.Add(keyword);
                    }
                }

                matches = filteredKeywords;
            }
            else
            {
                matches = Enumerable.Empty<Keyword>();
            }

            return matches;
        }
    }
}
