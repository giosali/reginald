namespace Reginald.Data.Keywords
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Reginald.Core.Extensions;
    using Reginald.Data.Base;
    using Reginald.Data.DisplayItems;
    using Reginald.Data.Keyphrases;
    using Reginald.Data.ShellItems;

    public static class KeywordHelper
    {
        private static SearchTermFactory SearchTermFactory { get; set; } = new();

        private static CancellationTokenSource Source { get; set; } = new();

        public static Task<IEnumerable<Keyword>> Filter(IEnumerable<Keyword> keywords, bool include, string input)
        {
            IEnumerable<Keyword> matches;
            if (include)
            {
                (string Keyword, string Separator, string Description) partition = input.Partition(" ");
                string cleanInput = partition.Keyword.RegexClean();
                string pattern = string.Format(CultureInfo.InvariantCulture, Keyword.KeywordRegexFormat, cleanInput);
                Regex rx = new(pattern, RegexOptions.IgnoreCase);
                matches = keywords.Where(k => k.Predicate(rx, partition));
            }
            else
            {
                matches = Enumerable.Empty<Keyword>();
            }

            return Task.FromResult(matches);
        }

        public static async Task<IEnumerable<Keyword>> FilterAsync(IEnumerable<Keyword> keywords, bool include, string input)
        {
            IEnumerable<Keyword> matches;
            if (include && !input.StartsWith(' '))
            {
                Source.Cancel();
                Source = new();

                (string Keyword, string Separator, string Description) partition = input.Partition(" ");
                string cleanInput = partition.Keyword.RegexClean();
                string pattern = string.Format(Keyword.PreciseKeywordRegexFormat, cleanInput);
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

        public static Task<IEnumerable<Keyword>> Set(IEnumerable<Keyword> keywords, string input)
        {
            foreach (Keyword keyword in keywords)
            {
                keyword.Description = string.Format(keyword.Format, input);
                keyword.Completion = input;
            }

            return Task.FromResult(keywords);
        }

        public static Task<IEnumerable<Keyword>> Process(IEnumerable<Keyword> keywords, bool include, string input, IEnumerable<ShellItem> applications)
        {
            List<Keyword> commands = new();
            if (include)
            {
                (string Keyword, string Separator, string Description) partition = input.Partition(" ");
                string cleanInput = partition.Keyword.RegexClean();
                string pattern = string.Format(CultureInfo.InvariantCulture, Keyword.KeywordRegexFormat, cleanInput);
                Regex rx = new(pattern, RegexOptions.IgnoreCase);
                IEnumerable<Keyword> matches = keywords.Where(k => k.Predicate(rx, partition));
                foreach (Keyword match in matches)
                {
                    KeywordClient client = new(SearchTermFactory, match, partition.Description, applications);
                    if (client.Keyword is not null)
                    {
                        commands.Add(client.Keyword);
                    }
                    else if (client.Keywords is not null)
                    {
                        commands.AddRange(client.Keywords);
                    }
                }
            }

            return Task.FromResult(commands as IEnumerable<Keyword>);
        }

        public static DisplayItem ToDisplayItem(Keyword keyword)
        {
            ResultFactory factory = new();
            DisplayItemClient client = new(factory, keyword);
            return client.Item;
        }

        public static DisplayItem ToConfirmationResult(Keyphrase keyphrase)
        {
            ResultFactory factory = new();
            DisplayItemClient client = new(factory, keyphrase);
            return client.Item;
        }

        public static IEnumerable<DisplayItem> ToSearchResults(IEnumerable<Keyword> keywords)
        {
            ResultFactory factory = new();
            DisplayItemClient client = new(factory, keywords);
            return client.Items;
        }

        public static IEnumerable<Keyword> ToKeywords(IEnumerable<KeywordDataModelBase> models)
        {
            SearchTermFactory factory = new SearchTermFactory();
            KeywordClient client = new(factory, models);
            return client.Keywords;
        }
    }
}
