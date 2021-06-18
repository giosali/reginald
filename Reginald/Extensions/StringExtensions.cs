using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Reginald.Extensions
{
    public static class StringExtensions
    {
        public static string[] Partition(this String str, char separator)
        {
            return str.Split(new char[] { separator }, 2);
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
                catch (System.OverflowException)
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
                            string[] strs = x.Partition('^');
                            string b = strs[0];
                            int power = Convert.ToInt32(strs[1]);
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
            Regex rx = new Regex("^[^A-Za-z]+$");
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
            string[] strs = expression.Partition('.');
            if (strs.Length > 1)
            {
                string domain = strs[0];
                string tld = strs[1];
                if (tld.Length > 1)
                {
                    Regex rx = new Regex(@"\s");
                    MatchCollection matches = rx.Matches(domain);
                    if (matches.Count == 0)
                        return true;
                }
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