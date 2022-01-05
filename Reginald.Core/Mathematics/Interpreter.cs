using Reginald.Core.Base;
using Reginald.Extensions;
using System;
using System.Text.RegularExpressions;

namespace Reginald.Core.Mathematics
{
    public static class Interpreter
    {
        /// <summary>
        /// Indicates whether the string is a valid math expression.
        /// </summary>
        /// <param name="input">The string to evaluate.</param>
        /// <returns><see langword="true"/> if <paramref name="input"/> is a valid math expression; otherwise, <see langword="false"/>.</returns>
        public static bool IsMathExpression(string input)
        {
            Regex isMathRx = new(Constants.IsMathRegexPattern);
            Regex invalidDecimalRx = new(Constants.InvalidDecimalRegexPattern);
            Regex malformedExponentiationRx = new(Constants.MalformedExponentiationRegexPattern);
            return isMathRx.IsMatch(input) && !invalidDecimalRx.IsMatch(input) && !malformedExponentiationRx.IsMatch(input);
        }

        /// <summary>
        /// Attempts to successfully interpret an evaluated string that throws a DivideByZeroException. A return value indicates whether the interpretation was successful.
        /// </summary>
        /// <param name="input">The string to interpret.</param>
        /// <param name="interpretation">A return value indicating whether the interpretation was successful.</param>
        /// <returns><see langword="true"/> if <paramref name="input"/> was interpreted successfully; otherwise, <see langword="false"/></returns>
        public static bool TryInterpretDivideByZeroException(string input, out string interpretation)
        {
            // This pattern searches for any "/ 0" and "/ (0)" instances in a string.
            // If any are detected, they're replaced by an empty string.
            // We do this to see if the evaluation of a string without
            // any "divide by zero"s is positive or negative.
            Regex divideByZeroRx = new(Constants.DivideByZeroRegexPattern);
            interpretation = divideByZeroRx.Replace(input, new MatchEvaluator(_ =>
            {
                return string.Empty;
            }));

            // This pattern searches for any mathematical expressions inside
            // of parentheses. "5 / (1 - 1)" will be converted to "5 / 0".
            // We need to do this because DivideByZeroRegexPattern cannot detect
            // zeros inside of parentheses.
            //Regex parenthesesRx = new(Constants.ParenthesesRegexPattern);
            //interpretation = parenthesesRx.Replace(interpretation, new MatchEvaluator(match =>
            //{
            //    return Calculator.Calculate(match.Groups[0].Value);
            //}));

            // We get the evaluation here
            interpretation = Calculator.Calculate(interpretation);

            // Let's see if it's positive or negative
            if (double.TryParse(interpretation, out double d))
            {
                interpretation = d > 0 ? "+∞" : "-∞";
                return true;
            }
            else
            {
                interpretation = "...";
                return false;
            }
        }

        /// <summary>
        /// Attempts to successfully interpret an evaluated string that throws an EvaluateException. A return value indicates whether the interpretation was successful.
        /// </summary>
        /// <param name="input">The string to interpret.</param>
        /// <param name="interpretation">A return value indicating whether the interpretation was successful.</param>
        /// <returns><see langword="true"/> if <paramref name="input"/> was interpreted successfully; otherwise, <see langword="false"/></returns>
        public static bool TryInterpretEvaluateException(string input, out string interpretation)
        {
            Regex rx = new(Constants.ExponentiationRegexPattern);
            if (rx.IsMatch(input))
            {
                interpretation = rx.Replace(input, new MatchEvaluator(match =>
                {
                    string exponentiation = match.ToString();
                    (string b, _, string e) = exponentiation.Partition("^");
                    double bass = double.Parse(b);
                    double exponent = double.Parse(e);
                    return Math.Pow(bass, exponent).ToString();
                }));
                return true;
            }
            else
            {
                interpretation = "...";
                return false;
            }
        }

        /// <summary>
        /// Attempts to successfully interpret an evaluated string that throws an SyntaxErrorException. A return value indicates whether the interpretation was successful.
        /// </summary>
        /// <param name="input">The string to interpret.</param>
        /// <param name="interpretation">A return value indicating whether the interpretation was successful.</param>
        /// <returns><see langword="true"/> if <paramref name="input"/> was interpreted successfully; otherwise, <see langword="false"/></returns>
        public static bool TryInterpretSyntaxErrorException(string input, out string interpretation)
        {
            Regex rx = new(Constants.FactorialRegexPattern);
            if (rx.IsMatch(input))
            {
                interpretation = Calculator.Factorialize(input);
                return true;
            }
            else
            {
                interpretation = "...";
                return false;
            }
        }
    }
}