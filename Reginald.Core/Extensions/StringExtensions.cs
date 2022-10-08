namespace Reginald.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Web;
    using System.Windows;
    using Reginald.Core.IO;

    public static class StringExtensions
    {
        private static readonly HashSet<uint> _topLevelDomains = new();

        /// <summary>
        /// Returns a value indicating whether a specified substring occurs at the beginning of this string or at the beginning of a word in this string.
        /// </summary>
        /// <param name="str">The string to be evaluated.</param>
        /// <param name="phrase">The string to seek.</param>
        /// <returns><see langword="true"/> if <paramref name="str"/> occurs at the beginning of this string or at the beginning of a word in this string; otherwise, <see langword="false"/>.</returns>
        public static bool ContainsPhrase(this string str, string phrase)
        {
            if (str is null)
            {
                return false;
            }

            int index = 0;
            while ((index = str.IndexOf(phrase, index, StringComparison.OrdinalIgnoreCase)) != -1)
            {
                // Returns true if a match is found at the start of the string
                // or if the previous character isn't a letter.
                if (index == 0 || !char.IsLetter(str[index - 1]))
                {
                    return true;
                }

                index++;
            }

            return false;
        }

        /// <summary>
        /// Indicates whether the string contains a top-level domain.
        /// </summary>
        /// <param name="expression">The string to be evaluated.</param>
        /// <returns><see langword="true"/> if <paramref name="expression"/> contains a top-level domain; otherwise, <see langword="false"/>.</returns>
        public static bool ContainsTopLevelDomain(this string expression)
        {
            if (!expression.Contains('.') || !Uri.TryCreate(expression, UriKind.Absolute, out Uri uri))
            {
                return false;
            }

            if (_topLevelDomains.Count == 0)
            {
                Uri packUri = FileOperations.GetResourcePath("TopLevelDomains.txt");
                using StreamReader reader = new(Application.GetResourceStream(packUri).Stream);
                while (!reader.EndOfStream)
                {
                    _ = _topLevelDomains.Add(reader.ReadLine().ToLower().GetCrc32HashCode());
                }
            }

            return _topLevelDomains.Contains(uri.Host.RPartition(".").Right.ToLower().GetCrc32HashCode());
        }

        public static uint GetCrc32HashCode(this string s)
        {
            uint crc = 0xFFFFFFFF;
            for (int i = 0; i < s.Length; i++)
            {
                char ch = s[i];
                for (int j = 0; j < 8; j++)
                {
                    uint b = (ch ^ crc) & 1;
                    crc >>= 1;
                    if (b == 1)
                    {
                        crc ^= 0xEDB88320;
                    }

                    ch >>= 1;
                }
            }

            return ~crc;
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
            StringComparison comparison = StringComparison.OrdinalIgnoreCase;
            return expression.StartsWith("https://", comparison) || expression.StartsWith("http://", comparison) ? expression : "https://" + expression;
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

        public static string Replace(this string expression, string oldValue, string newValue, int count)
        {
            int capacity = expression.Length - (oldValue.Length * count) + (newValue.Length * count);
            StringBuilder sb = new(capacity);
            int index = 0;
            for (int i = 0; i < count; i++)
            {
                int oldValueIndex = expression.IndexOf(oldValue, index);
                if (oldValueIndex == -1)
                {
                    break;
                }

                sb.Append(expression.Substring(index, oldValueIndex - index))
                  .Append(newValue);
                index = oldValueIndex + oldValue.Length;
            }

            sb.Append(expression.Substring(index));
            return sb.ToString();
        }

        /// <summary>
        /// Attempts to return the element at a specified index in a string.
        /// </summary>
        /// <param name="str">A string to return an element from..</param>
        /// <param name="index">The zero-based index of the element to retrieve.</param>
        /// <param name="ch">When this method returns, contains the character at the specified index in the string, or the default value of <see cref="char"/>.</param>
        /// <returns><see langword="true"/> if the element was successfully retrieved; otherwise, <see langword="false"/>.</returns>
        public static bool TryElementAt(this string str, int index, out char ch)
        {
            if (str is null || index >= str.Length || index < 0)
            {
                ch = default(char);
                return false;
            }

            ch = str[index];
            return true;
        }
    }
}
