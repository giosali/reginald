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
    using Reginald.Core.Extensions;
    using Reginald.Core.Factories;

    public class ShellItemHelper
    {
        public static Task<IEnumerable<ShellItem>> FilterByStrictNames(IEnumerable<ShellItem> items, bool include, string input)
        {
            IEnumerable<ShellItem> matches;
            if (include)
            {
                string cleanInput = input.RegexClean();
                string pattern = string.Format(CultureInfo.InvariantCulture, Constants.KeywordRegexFormat, cleanInput);
                Regex rx = new(pattern, RegexOptions.IgnoreCase);
                matches = items.Where(item => rx.IsMatch(item.Name));
            }
            else
            {
                matches = Enumerable.Empty<ShellItem>();
            }

            return Task.FromResult(matches);
        }

        public static Task<IEnumerable<ShellItem>> FilterByUppercaseCharacters(IEnumerable<ShellItem> items, bool include, string input)
        {
            IEnumerable<ShellItem> matches;
            if (include && !input.StartsWith(' '))
            {
                string cleanInput = input.RegexClean();
                string pattern = string.Format(Constants.ShellItemUppercaseRegexFormat, cleanInput);
                Regex rx = new(pattern, RegexOptions.IgnoreCase);
                matches = items.Where(item =>
                {
                    Match match = rx.Match(item.Name);
                    return match.Success && char.IsUpper(item.Name[match.Index]);
                });
            }
            else
            {
                matches = Enumerable.Empty<ShellItem>();
            }

            return Task.FromResult(matches);
        }

        public static IEnumerable<DisplayItem> ToSearchResults(IEnumerable<ShellItem> items)
        {
            ResultFactory factory = new();
            DisplayItemClient client = new(factory, items);
            return client.Items;
        }
    }
}
