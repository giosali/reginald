namespace Reginald.Data.ShellItems
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Reginald.Core.Extensions;

    public partial class Application
    {
        public static Task<IEnumerable<ShellItem>> FilterByNames(IEnumerable<ShellItem> items, bool include, string input)
        {
            IEnumerable<ShellItem> matches;
            if (include)
            {
                string cleanInput = input.RegexClean();
                string pattern = string.Format(CultureInfo.InvariantCulture, NameRegexFormat, cleanInput);
                Regex rx = new(pattern, RegexOptions.IgnoreCase);
                matches = items.Where(item => rx.IsMatch(item.Name))
                               .OrderBy(item => item.Name);
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
                string pattern = string.Format(ShellItemUppercaseRegexFormat, cleanInput);
                Regex rx = new(pattern, RegexOptions.IgnoreCase);
                matches = items.Where(item =>
                {
                    Match match = rx.Match(item.Name);
                    return match.Success && char.IsUpper(item.Name[match.Index]);
                }).OrderBy(item => item.Name);
            }
            else
            {
                matches = Enumerable.Empty<ShellItem>();
            }

            return Task.FromResult(matches);
        }
    }
}
