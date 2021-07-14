using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Reginald.Extensions
{
    public static class StringExtensions
    {
        public static (string Left, string Separator, string Right) Partition(this String expression, string separator)
        {
            string[] results = expression.Split(separator, 2);
            if (results.Length > 1)
                return (results[0], separator, results[1]);
            return (expression, String.Empty, String.Empty);
        }

        public static (string Left, string Separator, string Right) Partition(this String expression, char separator)
        {
            string[] results = expression.Split(separator, 2);
            if (results.Length > 1)
                return (results[0], separator.ToString(), results[1]);
            return (expression, String.Empty, String.Empty);
        }

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
            return (expression, String.Empty, String.Empty);
        }

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

        public static bool IsMathExpression(this string expression)
        {
            Regex rx = new Regex(@"^[^A-Za-z]+$");
            MatchCollection matches = rx.Matches(expression);
            if (matches.Count > 0)
                return true;
            return false;
        }

        public static string Quote(this string expression, string replacement)
        {
            Regex rx = new Regex(@"\s");
            string result = rx.Replace(expression, new MatchEvaluator(m =>
            {
                return replacement;
            }));
            return result;
        }

        public static bool HasTopLevelDomain(this string expression)
        {
            (string domain, _, string tld) = expression.Partition(".");
            if (tld.Length > 1)
            {
                Regex rx = new Regex(@"\s");
                MatchCollection matches = rx.Matches(domain);
                if (matches.Count == 0)
                    return true;
            }
            return false;
        }

        public static bool HasScheme(this string expression)
        {
            StringComparison comparison = StringComparison.InvariantCultureIgnoreCase;
            if (expression.StartsWith("https://", comparison) || expression.StartsWith("http://", comparison))
                return true;
            return false;
        }

        public static string MakeValidScheme(this string expression)
        {
            Regex rx = new Regex("https*://", RegexOptions.IgnoreCase);
            MatchCollection matches = rx.Matches(expression);
            if (matches.Count == 0)
                expression = "https://" + expression;
            return expression;
        }
    }
}