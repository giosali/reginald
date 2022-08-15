namespace Reginald.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Web;
    using Reginald.Core.IO;

    public static class StringExtensions
    {
        private static readonly string[] _regexCharacters = { @"\", "[", "(", ")", ".", "+", "*", "?", "|", "$" };

        private static HashSet<string> _topLevelDomains = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Indicates whether the string contains a top-level domain.
        /// </summary>
        /// <param name="expression">The string to be evaluated.</param>
        /// <returns><see langword="true"/> if <paramref name="expression"/> contains a top-level domain; otherwise, <see langword="false"/>.</returns>
        public static bool ContainsTopLevelDomain(this string expression)
        {
            if (!expression.Contains('.'))
            {
                return false;
            }

            if (!Uri.TryCreate(expression, UriKind.Absolute, out Uri uri))
            {
                return false;
            }

            if (_topLevelDomains.Count == 0)
            {
                foreach (string line in File.ReadLines(FileOperations.GetFilePath("TopLevelDomains.txt", true)))
                {
                    _ = _topLevelDomains.Add(line);
                }
            }

            return _topLevelDomains.Contains(uri.Host.RPartition(".").Right);
        }

        /// <summary>
        /// Splits a string at the first occurrence of <paramref name="separator"/> into a 3-tuple that consists solely of strings in the following order: the part preceding the separator, the separator itself, and the part proceeding the separator.
        /// </summary>
        /// <param name="expression">The string to split.</param>
        /// <param name="separator">The text to search for in the string whose location will be the split point.</param>
        /// <returns>A 3-tuple that consists of the part preceding the separator, the separator, and the part proceeding the separator. If no occurrence of <paramref name="separator"/> is found, the 3-tuple will consist of the following from left to right: <paramref name="expression"/>, <see cref="string.Empty"/>, and <see cref="string.Empty"/>.</returns>
        public static (string Left, string Separator, string Right) Partition(this string expression, string separator)
        {
            if (expression is null)
            {
                throw new ArgumentNullException(expression);
            }

            string[] substrings = expression.Split(separator, 2);
            return substrings.Length > 1 ? (substrings[0], separator, substrings[1]) : (expression, string.Empty, string.Empty);
        }

        /// <summary>
        /// Splits a string at the first occurrence of <paramref name="separator"/> into a 3-tuple that consists solely of strings in the following order: the part preceding the separator, the separator itself, and the part proceeding the separator.
        /// </summary>
        /// <param name="expression">The string to split.</param>
        /// <param name="separator">The character to search for in the string whose location will be the split point.</param>
        /// <returns>A 3-tuple that consists of the part preceding the separator, the separator, and the part proceeding the separator. If no occurrence of <paramref name="separator"/> is found, the 3-tuple will consist of the following from left to right: <paramref name="expression"/>, <see cref="string.Empty"/>, and <see cref="string.Empty"/>.</returns>
        public static (string Left, string Separator, string Right) Partition(this string expression, char separator)
        {
            if (expression is null)
            {
                throw new ArgumentNullException(expression);
            }

            string[] substrings = expression.Split(separator, 2);
            return substrings.Length > 1 ? (substrings[0], separator.ToString(), substrings[1]) : (expression, string.Empty, string.Empty);
        }

        /// <summary>
        /// Returns a new string with an HTTP scheme prepended to the string if the string does not start with an HTTP scheme.
        /// </summary>
        /// <param name="expression">The string to have "https://" prepended.</param>
        /// <returns>If <paramref name="expression"/> does not start with an HTTP scheme, a new string with "https://" prepended to <paramref name="expression"/> will be returned. Otherwise, <paramref name="expression"/> will be returned.</returns>
        public static string PrependScheme(this string expression)
        {
            return expression.StartsWithScheme() ? expression : "https://" + expression;
        }

        /// <summary>
        /// Splits a string at the first occurrence of <paramref name="separator"/> from the right side into a 3-tuple that consists solely of strings in the following order: the part preceding the separator, the separator itself, and the part proceeding the separator.
        /// </summary>
        /// <param name="expression">The string to split.</param>
        /// <param name="separator">The text to search for in the string whose location will be the split point.</param>
        /// <returns>A 3-tuple that consists of the part preceding the separator, the separator, and the part proceeding the separator. If no occurrence of <paramref name="separator"/> is found, the 3-tuple will consist of the following from left to right: <see cref="string.Empty"/>, <see cref="string.Empty"/>, and <paramref name="expression"/>.</returns>
        public static (string Left, string Separator, string Right) RPartition(this string expression, string separator)
        {
            if (expression is null)
            {
                throw new ArgumentNullException(expression);
            }

            int separatorIndex = expression.LastIndexOf(separator);
            return separatorIndex > -1 ? (expression.Substring(0, separatorIndex), separator, expression[(separatorIndex + separator.Length)..]) : (string.Empty, string.Empty, expression);
        }

        /// <summary>
        /// Splits a string at the first occurrence of <paramref name="separator"/> from the right side into a 3-tuple that consists solely of strings in the following order: the part preceding the separator, the separator itself, and the part proceeding the separator.
        /// </summary>
        /// <param name="expression">The string to split.</param>
        /// <param name="separator">The character to search for in the string whose location will be the split point.</param>
        /// <returns>A 3-tuple that consists of the part preceding the separator, the separator, and the part proceeding the separator. If no occurrence of <paramref name="separator"/> is found, the 3-tuple will consist of the following from left to right: <see cref="string.Empty"/>, <see cref="string.Empty"/>, and <paramref name="expression"/>.</returns>
        public static (string Left, string Separator, string Right) RPartition(this string expression, char separator)
        {
            if (expression is null)
            {
                throw new ArgumentNullException(expression);
            }
            
            int separatorIndex = expression.LastIndexOf(separator);
            return separatorIndex > -1 ? (expression.Substring(0, separatorIndex), separator.ToString(), expression[(separatorIndex + 1)..]) : (string.Empty, string.Empty, expression);
        }

        /// <summary>
        /// Returns a new string in which all whitespace characters are replaced by a specified string.
        /// </summary>
        /// <param name="expression">The string with whitespace characters.</param>
        /// /// <param name="useUtf8">Indicates whether or not to encode the expression with UTF-8 encoding.</param>
        /// <returns>A new string encoded using UTF-8 encoding if <paramref name="useUtf8"/> is true; otherwise, an escaped representation of <paramref name="expression"/>.</returns>
        public static string Quote(this string expression, bool useUtf8)
        {
            return useUtf8 ? HttpUtility.UrlEncode(expression) : Uri.EscapeDataString(expression);
        }

        /// <summary>
        /// Receives a string and returns a new string with all unmatched or unterminated characters escaped.
        /// </summary>
        /// <param name="expression">The string to clean.</param>
        /// <returns>A new string with all unmatched or unterminated characters in <paramref name="expression"/> escaped.</returns>
        public static string RegexClean(this string expression)
        {
            for (int i = 0; i < _regexCharacters.Length; i++)
            {
                string ch = _regexCharacters[i];
                expression = expression.Replace(ch, @$"\{ch}");
            }

            return expression;
        }

        public static string Replace(this string expression, string oldValue, string newValue, int count)
        {
            int capacity = expression.Length - (oldValue.Length * count) + (newValue.Length * count);
            StringBuilder sb = new(capacity);
            int index = 0;
            for (int i = 0; i < count; i++)
            {
                int oldValueIndex = expression.IndexOf(oldValue, index);
                sb.Append(expression.Substring(index, oldValueIndex - index))
                  .Append(newValue);
                index = oldValueIndex + oldValue.Length;
            }

            sb.Append(expression.Substring(index));
            return sb.ToString();
        }

        /// <summary>
        /// Indicates whether the string starts with an HTTP scheme.
        /// </summary>
        /// <param name="expression">The string to evaluate.</param>
        /// <returns><see langword="true"/> if <paramref name="expression"/> starts with an HTTP scheme; otherwise, <see langword="false"/>.</returns>
        public static bool StartsWithScheme(this string expression)
        {
            StringComparison comparison = StringComparison.InvariantCultureIgnoreCase;
            return expression.StartsWith("https://", comparison) || expression.StartsWith("http://", comparison);
        }
    }
}
