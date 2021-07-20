using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Reginald.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Splits a string into a tuple consisting of three strings: the separator, the left side, and the right side. If there is no match, the last two strings in the tuple will be set to String.Empty.
        /// </summary>
        /// <param name="expression">The string to split.</param>
        /// <param name="separator">The text upon which the expression should be split.</param>
        /// <returns>A tuple respectively containing three named strings: Left, Separator, Right.</returns>
        /// <example>
        /// <code>
        /// string expression = "This is a string";
        /// string separator = "is";
        /// var result = expression.Partition(separator);
        /// Console.WriteLine(result.Left);
        /// Console.WriteLine(result.Separator);
        /// Console.WriteLine(result.Right);
        /// </code>
        /// </example>
        public static (string Left, string Separator, string Right) Partition(this String expression, string separator)
        {
            string[] results = expression.Split(separator, 2);
            if (results.Length > 1)
                return (results[0], separator, results[1]);
            return (expression, String.Empty, String.Empty);
        }

        /// <summary>
        /// Splits a string into a tuple consisting of three strings: the separator, the left side, and the right side. If there is no match, the last two strings in the tuple will be set to String.Empty.
        /// </summary>
        /// <param name="expression">The string to split.</param>
        /// <param name="separator">The character upon which the expression should be split.</param>
        /// <returns>A tuple respectively containing three named strings: Left, Separator, Right.</returns>
        /// <example>
        /// <code>
        /// string expression = "This is a string";
        /// char separator = 'a';
        /// var result = expression.Partition(separator);
        /// Console.WriteLine(result.Left);
        /// Console.WriteLine(result.Separator);
        /// Console.WriteLine(result.Right);
        /// </code>
        /// </example>
        public static (string Left, string Separator, string Right) Partition(this String expression, char separator)
        {
            string[] results = expression.Split(separator, 2);
            if (results.Length > 1)
                return (results[0], separator.ToString(), results[1]);
            return (expression, String.Empty, String.Empty);
        }

        /// <summary>
        /// Splits a string from the *right* side into a tuple consisting of three strings: the separator, the left side, and the right side. If there is no match, the first two strings in the tuple will be set to String.Empty.
        /// </summary>
        /// <param name="expression">The string to split.</param>
        /// <param name="separator">The text upon which the expression should be split.</param>
        /// <returns>A tuple respectively containing three named strings: Left, Separator, Right.</returns>
        /// <example>
        /// <code>
        /// string expression = "This is a string";
        /// string separator = "is";
        /// var result = expression.Partition(separator);
        /// Console.WriteLine(result.Left);
        /// Console.WriteLine(result.Separator);
        /// Console.WriteLine(result.Right);
        /// </code>
        /// </example>
        public static (string Left, string Separator, string Right) RPartition(this String expression, string separator)
        {
            string[] results = expression.Split(separator);
            if (results.Length > 1)
            {
                string left = String.Empty;
                for (int i = 0; i < results.Length - 1; i++)
                {
                    left += results[i];
                }
                return (left, separator, results[^1]);
            }
            return (String.Empty, String.Empty, expression);
        }

        /// <summary>
        /// Splits a string from the *right* side into a tuple consisting of three strings: the separator, the left side, and the right side. If there is no match, the first two strings in the tuple will be set to String.Empty.
        /// </summary>
        /// <param name="expression">The string to split.</param>
        /// <param name="separator">The character upon which the expression should be split.</param>
        /// <returns>A tuple respectively containing three named strings: Left, Separator, Right.</returns>
        /// <example>
        /// <code>
        /// string expression = "This is a string";
        /// char separator = 'a';
        /// var result = expression.Partition(separator);
        /// Console.WriteLine(result.Left);
        /// Console.WriteLine(result.Separator);
        /// Console.WriteLine(result.Right);
        /// </code>
        /// </example>
        public static (string Left, string Separator, string Right) RPartition(this String expression, char separator)
        {
            string[] results = expression.Split(separator);
            if (results.Length > 1)
            {
                string left = String.Empty;
                for (int i = 0; i < results.Length - 1; i++)
                {
                    left += results[i];
                }
                return (left, separator.ToString(), results[^1]);
            }
            return (String.Empty, String.Empty, expression);
        }

        /// <summary>
        /// Evaluates and returns the result of a mathematical expression in a string.
        /// </summary>
        /// <param name="expression">The string consisting of a mathematical expression.</param>
        /// <returns>The result of the mathematical expression.</returns>
        /// <example>
        /// <code>
        /// string expression = "2 + 2^4 - (5 / 2)";
        /// string result = expression.Eval();
        /// Console.WriteLine(result);
        /// </code>
        /// </example>
        public static string Eval(this String expression)
        {
            string result;
            while (true)
            {
                try
                {
                    System.Data.DataTable table = new();
                    result = Convert.ToString(table.Compute("1.0 * " + expression, String.Empty));
                    if (result.EndsWith(".0"))
                        result = result.Replace(".0", String.Empty);
                    break;
                }
                catch (System.Data.SyntaxErrorException)
                {
                    result = "...";
                    break;
                }
                catch (OverflowException)
                {
                    result = "...";
                    break;
                }
                catch (DivideByZeroException)
                {
                    Regex rx = new Regex(@"/\s*0");
                    result = rx.Replace(expression, new MatchEvaluator(m =>
                    {
                        return String.Empty;
                    }));
                    result = result.Eval();
                    if (double.TryParse(result, out double d))
                    {
                        if (d > 0)
                            result = "+∞";
                        else
                            result = "-∞";
                    }
                    break;
                }
                catch (System.Data.EvaluateException)
                {
                    Regex rx = new Regex(@"\d+\^-?(\d+)");
                    MatchCollection matches = rx.Matches(expression);
                    if (matches.Count > 0)
                    {
                        result = rx.Replace(expression, new MatchEvaluator(m =>
                        {
                            string x = m.ToString();
                            (string Left, string Separator, string Right) partition = x.Partition("^");
                            string b = partition.Left;
                            int power = Convert.ToInt32(partition.Right);
                            string concat = String.Concat(Enumerable.Repeat(b + "*", Math.Abs(power)));

                            concat = concat.Remove(concat.Length - 1);
                            if (power < 0)
                                concat = "1 / (" + concat + ")";

                            return concat;
                        }));
                        expression = result;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Indicates whether the string is a math expression.
        /// </summary>
        /// <param name="expression">The string to evaluate.</param>
        /// <returns>True if the string is a math expression; otherwise, false.</returns>
        public static bool IsMathExpression(this string expression)
        {
            Regex rx = new Regex(@"^[0-9\s+-/^*()><]+$");
            MatchCollection matches = rx.Matches(expression);
            if (matches.Count > 0)
                return true;
            return false;
        }

        /// <summary>
        /// Returns a string with whitespace characters replaced by a specified character.
        /// </summary>
        /// <param name="expression">The string with whitespace characters.</param>
        /// <param name="replacement">The replacement character.</param>
        /// <returns>The string with whitespace characters replaced.</returns>
        public static string Quote(this string expression, string replacement)
        {
            Regex rx = new Regex(@"\s");
            string result = rx.Replace(expression, new MatchEvaluator(m =>
            {
                return replacement;
            }));
            return result;
        }

        /// <summary>
        /// Indicates whether the string contains a top-level domain.
        /// </summary>
        /// <param name="expression">The string to be evaluated.</param>
        /// <returns>True if the string contains a top-level domain; otherwise, false.</returns>
        public static bool HasTopLevelDomain(this string expression)
        {
            (string domain, _, string tld) = expression.Partition(".");
            if (tld.Length > 1)
            {
                if (!char.IsDigit(tld[0]))
                {
                    Regex rx = new Regex(@"\s");
                    MatchCollection matches = rx.Matches(domain);
                    if (matches.Count == 0)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Indicates whether the string contains an HTTP scheme.
        /// </summary>
        /// <param name="expression">The string to evaluate.</param>
        /// <returns>True if the string contains an HTTP scheme; otherwise, false.</returns>
        public static bool HasScheme(this string expression)
        {
            StringComparison comparison = StringComparison.InvariantCultureIgnoreCase;
            if (expression.StartsWith("https://", comparison) || expression.StartsWith("http://", comparison))
                return true;
            return false;
        }

        /// <summary>
        /// Prepends "https://" to a string.
        /// </summary>
        /// <param name="expression">The string to have "https://" prepended.</param>
        /// <returns>The string with "https://" prepended.</returns>
        public static string PrependScheme(this string expression)
        {
            Regex rx = new Regex("https*://", RegexOptions.IgnoreCase);
            MatchCollection matches = rx.Matches(expression);
            if (matches.Count == 0)
                expression = "https://" + expression;
            return expression;
        }
    }
}