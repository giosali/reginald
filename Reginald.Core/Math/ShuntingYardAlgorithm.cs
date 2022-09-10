namespace Reginald.Core.Math
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    public static class ShuntingYardAlgorithm
    {
        private const string Ellipsis = "...";

        public static bool TryParse(string expression, char decimalSeparator, out string result)
        {
            // [Guard]
            // Returns false if the expression is null or empty
            // or if it begins with a space.
            if (string.IsNullOrEmpty(expression) || expression.StartsWith(' '))
            {
                result = null;
                return false;
            }

            if (!TryParseInfixExpression(expression.Replace(" ", string.Empty), decimalSeparator, out string postFixExpression) && postFixExpression is null)
            {
                result = null;
                return false;
            }

            if (postFixExpression == Ellipsis)
            {
                result = postFixExpression;
                return false;
            }

            return TryParsePostfixExpression(postFixExpression, decimalSeparator.ToString(), out result);
        }

        private static bool TryParseInfixExpression(string expression, char decimalSeparator, out string postFixExpression)
        {
            postFixExpression = null;

            // [Guard]
            // Returns false if the first character isn't a number
            // and isn't one of the following:
            // -- the subtraction operator
            // -- the left parenthesis
            // -- the decimal point
            char fCh = expression[0];
            if (!char.IsDigit(fCh) && fCh != '-' && fCh != '−' && fCh != '(' && fCh != decimalSeparator)
            {
                return false;
            }

            Queue<string> output = new();
            Stack<char> operators = new();
            StringBuilder buffer = new();
            char previousToken = '\0';
            int parenthesisCount = 0;
            for (int i = 0; i < expression.Length; i++)
            {
                char token = expression[i];
                switch (token)
                {
                    case >= '0' and <= '9':
                    case ',':
                    case '.':
                        if (!char.IsDigit(token) && token != decimalSeparator)
                        {
                            return false;
                        }

                        // Exits if the previous token is an exponentiation operator
                        // and if the current token is a decimal point
                        // or if the previous token
                        // and the current token are both the decimal point.
                        if ((previousToken == '^' && token == decimalSeparator) || (previousToken == decimalSeparator && token == decimalSeparator))
                        {
                            postFixExpression = Ellipsis;
                            return false;
                        }

                        buffer.Append(token);
                        break;
                    case '(':
                        // Handles multiplication by parentheses (parentheses as operators).
                        if (previousToken == ')' || char.IsDigit(previousToken))
                        {
                            operators.Push('*');
                        }

                        FlushBuffer();
                        operators.Push(token);
                        parenthesisCount++;
                        break;
                    case ')':
                        // [Guard]
                        // Exits if a right parenthesis is encountered and the current
                        // parenthesis count is 0, meaning there's a mismatched parenthesis
                        if (parenthesisCount == 0)
                        {
                            postFixExpression = Ellipsis;
                            return false;
                        }

                        FlushBuffer();

                        while (operators.TryPop(out char stackOp) && stackOp != '(')
                        {
                            output.Enqueue(stackOp.ToString());
                        }

                        parenthesisCount--;
                        break;
                    case '^':
                    case '*':
                    case '/':
                    case '÷':
                    case '+':
                    case '-':
                    case '−':
                        // [Guard]
                        // Exits if the previous token is an operator
                        // or an exponentiation operator
                        // and if the current token isn't a subtraction operator.
                        if ((Operator.IsOperator(previousToken) || previousToken == '^') && token != '-' && token != '−')
                        {
                            postFixExpression = Ellipsis;
                            return false;
                        }

                        // [Guard]
                        // Exits if the current token is a decimal point
                        // and the previous token isn't a number
                        // and we're at the end of the expression.
                        if (token == decimalSeparator && !char.IsDigit(previousToken) && i == expression.Length - 1)
                        {
                            postFixExpression = Ellipsis;
                            return false;
                        }

                        FlushBuffer();

                        // Handles negative numbers.
                        if ((token == '-' || token == '−') && token == previousToken)
                        {
                            output.Enqueue("-1");
                            operators.Push('*');
                            break;
                        }

                        // Pops operators from the stack and enqueues them
                        // while the stack contains operators and the operators aren't the left parenthesis
                        // and have a higher precedence than the new operator
                        // or have the same precedence and the new operator is left-associative.
                        while (operators.TryPeek(out char stackOp) && stackOp != '(')
                        {
                            int stackPrecedence = Operator.GetPrecedence(stackOp), newPrecedence = Operator.GetPrecedence(token);
                            if (stackPrecedence > newPrecedence || (stackPrecedence == newPrecedence && Operator.IsLeftAssociative(token)))
                            {
                                output.Enqueue(operators.Pop().ToString());
                                continue;
                            }

                            break;
                        }

                        operators.Push(token);
                        break;
                    case '!':
                        FlushBuffer();

                        // [Guard]
                        // Exits if there is an exclamation mark but no number preceding it.
                        if (!output.TryPeek(out string n))
                        {
                            return false;
                        }

                        // [Guard]
                        // Exits if there is a factorial containing a decimal
                        // or if the previous token is an exponentiation operator.
                        if (n.Contains(decimalSeparator) || previousToken == '^')
                        {
                            postFixExpression = Ellipsis;
                            return false;
                        }

                        output.Enqueue(token.ToString());
                        break;
                    default:
                        return false;
                }

                previousToken = token;
            }

            // Exits if there are unmatched parentheses
            // or if there's an incomplete exponent.
            if (parenthesisCount != 0 || Operator.IsOperator(previousToken))
            {
                postFixExpression = Ellipsis;
                return false;
            }

            FlushBuffer();
            while (operators.TryPop(out char stackOp))
            {
                output.Enqueue(stackOp.ToString());
            }

            postFixExpression = string.Join(' ', output.ToArray());
            return true;

            void FlushBuffer()
            {
                if (buffer.Length > 0)
                {
                    output.Enqueue(buffer.ToString());
                    buffer.Clear();
                }
            }
        }

        private static bool TryParsePostfixExpression(string expression, string decimalSeparator, out string result)
        {
            result = "...";
            Stack<double> nums = new();
            string[] subs = expression.Split(' ');
            for (int i = 0; i < subs.Length; i++)
            {
                string sub = subs[i];
                if (double.TryParse(sub, NumberStyles.AllowThousands | NumberStyles.Float, new NumberFormatInfo() { NumberDecimalSeparator = decimalSeparator }, out double num))
                {
                    nums.Push(num);
                    continue;
                }

                if (!nums.TryPop(out double right))
                {
                    return false;
                }

                bool leftSuccess = nums.TryPop(out double left);
                if (!leftSuccess)
                {
                    left = 0;
                }

                switch (sub)
                {
                    case "-":
                    case "−":
                        nums.Push(left - right);
                        break;
                    case "+":
                        nums.Push(left + right);
                        break;
                    case "/":
                    case "÷":
                        if (right == 0)
                        {
                            double n = left switch
                            {
                                > 0 => double.PositiveInfinity,
                                < 0 => double.NegativeInfinity,
                                _ => double.NaN,
                            };
                            nums.Push(n);
                            break;
                        }

                        nums.Push(left / right);
                        break;
                    case "*":
                        nums.Push(left * right);
                        break;
                    case "^":
                        nums.Push(Math.Pow(left, right));
                        break;
                    case "!":
                        if (leftSuccess)
                        {
                            nums.Push(left);
                        }

                        // Exits if the factorial isn't being applied to an integer.
                        if (right % 1 != 0)
                        {
                            return false;
                        }

                        nums.Push(Factorial(right));
                        break;
                }
            }

            result = Math.Round(nums.Pop(), 9).ToString();
            return true;
        }

        private static double Factorial(double n)
        {
            if (n < 0)
            {
                return double.NaN;
            }

            if (n == 0)
            {
                return 1;
            }

            // Any number beyond this will default to ∞.
            if (n > 170)
            {
                return double.PositiveInfinity;
            }

            double c = 1;
            for (; n > 0; n--)
            {
                c *= n;
            }

            return c;
        }

        private static class Operator
        {
            internal static bool IsOperator(char ch) => ch switch
            {
                '^' or '*' or '/' or '÷' or '+' or '-' or '−' => true,
                _ => false,
            };

            internal static int GetPrecedence(char op) => op switch
            {
                '^' => 2,
                '*' or '/' or '÷' => 1,
                '+' or '-' or '−' => 0,
                _ => -1,
            };

            internal static bool IsLeftAssociative(char op) => op switch
            {
                '*' or '/' or '÷' or '+' or '-' or '−' => true,
                _ => false,
            };
        }
    }
}
