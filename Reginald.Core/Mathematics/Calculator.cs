namespace Reginald.Core.Mathematics
{
    using System;
    using System.Data;
    using System.Text.RegularExpressions;
    using Reginald.Core.Base;

    public static class Calculator
    {
        /// <summary>
        /// Interprets and evaluates a string that consists of a mathematical expression.
        /// </summary>
        /// <param name="input">A string that consists of a mathematical expression.</param>
        /// <returns>A string that contains the evaluated mathematical expression.</returns>
        public static string Calculate(string input)
        {
            string interpretation;
            while (true)
            {
                try
                {
                    DataTable table = new();
                    object computation = table.Compute($"1.0 * {input}", string.Empty);
                    if (computation is bool boolean)
                    {
                        interpretation = boolean.ToString();
                    }
                    else
                    {
                        double calculation = Math.Round(Convert.ToDouble(computation), 9) + 0;
                        interpretation = calculation.ToString();
                    }

                    if (interpretation.EndsWith(".0"))
                    {
                        interpretation = interpretation.Replace(".0", string.Empty);
                    }

                    break;
                }
                catch (DivideByZeroException)
                {
                    _ = Interpreter.TryInterpretDivideByZeroException(input, out interpretation);
                    break;
                }
                catch (OverflowException)
                {
                    interpretation = "...";
                    break;
                }
                catch (EvaluateException)
                {
                    if (Interpreter.TryInterpretEvaluateException(input, out interpretation))
                    {
                        input = interpretation;
                    }
                    else
                    {
                        break;
                    }
                }
                catch (SyntaxErrorException)
                {
                    if (Interpreter.TryInterpretSyntaxErrorException(input, out interpretation))
                    {
                        input = interpretation;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return interpretation;
        }

        /// <summary>
        /// Evaluates a string that consists of a factorial.
        /// </summary>
        /// <param name="input">A string that consists of a factorial.</param>
        /// <returns>A string that contains the evaluated factorial.</returns>
        public static string Factorialize(string input)
        {
            Regex rx = new(Constants.FactorialRegexPattern);
            string result = rx.Replace(input, new MatchEvaluator(match =>
            {
                string factorial = match.Groups[1].Value;
                int integer = int.Parse(factorial);
                double product = 1;

                for (int i = integer; i > 0; i--)
                {
                    product *= i;
                }

                return product.ToString();
            }));
            return result;
        }
    }
}
