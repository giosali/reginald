namespace Reginald.Data.Keywords
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Reginald.Core.Extensions;

    public abstract partial class CommandKeyword
    {
        public static Task<IEnumerable<CommandKeyword>> Process(IEnumerable<CommandKeywordDataModel> models, bool include, string input)
        {
            IEnumerable<CommandKeyword> matches;
            if (include)
            {
                (string Keyword, string Separator, string Description) partition = input.Partition(" ");
                string cleanInput = partition.Keyword.RegexClean();
                string pattern = string.Format(CultureInfo.InvariantCulture, KeywordRegexFormat, cleanInput);
                Regex rx = new(pattern, RegexOptions.IgnoreCase);
                matches = models.Where(model => partition.Keyword.Length > 0 && rx.IsMatch(model.Keyword))
                                .SelectMany(model => KeywordFactory.CreateCommandKeywords(model, partition.Description));
            }
            else
            {
                matches = Enumerable.Empty<CommandKeyword>();
            }

            return Task.FromResult(matches);
        }
    }
}
