namespace Reginald.Tests.Reginald.Core
{
    using global::Reginald.Core.Math;
    using NUnit.Framework;

    [TestFixture]
    public class ShuntingYardAlgorithmTests
    {
        [Test]

        // Numbers
        [TestCase("0", "0")]
        [TestCase("1", "1")]
        [TestCase("10000000000000000", "10000000000000000")]
        [TestCase("100000000000000000", "1E+17")]
        [TestCase("100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", "1E+308")]
        [TestCase("100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000.123456789", "1E+308")]
        [TestCase("1000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", "∞")]
        [TestCase("-1000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", "-∞")]
        [TestCase("1000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000^2", "∞")]
        [TestCase("-1000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000^2", "-∞")]

        // Decimals
        [TestCase("1.0", "1")]
        [TestCase("1.1", "1.1")]
        [TestCase("0.1", "0.1")]
        [TestCase(".1", "0.1")]
        [TestCase("1.1234567891", "1.123456789")]
        [TestCase("0.0000000001", "0")]
        [TestCase(".4(4)", "1.6")]

        // Negatives
        [TestCase("-0", "0")]
        [TestCase("-1", "-1")]
        [TestCase("--1", "1")]
        [TestCase("---1", "-1")]
        [TestCase("----1", "1")]
        [TestCase("5 - -4", "9")]
        [TestCase("5 - -4 * 3", "17")]
        [TestCase("5 - -(4 * 3)", "17")]
        [TestCase("5 + -(2)(3 - 2 - 1 + 5)", "-5")]
        [TestCase("−0", "0")]
        [TestCase("−1", "-1")]
        [TestCase("−−1", "1")]
        [TestCase("−−−1", "-1")]
        [TestCase("−−−−1", "1")]
        [TestCase("5 − −4", "9")]
        [TestCase("5 − −4 * 3", "17")]
        [TestCase("5 − −(4 * 3)", "17")]
        [TestCase("5 + −(2)(3 − 2 − 1 + 5)", "-5")]

        // Parentheses
        [TestCase("(1)", "1")]
        [TestCase("(1.1)", "1.1")]
        [TestCase("(2)(2)", "4")]
        [TestCase("2(2 + 2) + 5", "13")]
        [TestCase("(2(2 + 2)) + 5", "13")]
        [TestCase("(3 + 5) * 6", "48")]
        [TestCase("((((((1))))))", "1")]
        [TestCase("(2 - 5)(4 / 2)", "-6")]
        [TestCase("-(1 + 1)", "-2")]

        // Subtraction
        [TestCase("2 - 2", "0")]
        [TestCase("-1 - 1", "-2")]
        [TestCase("2 − 2", "0")]
        [TestCase("-1 − 1", "-2")]

        // Addition
        [TestCase("1 + 1 + 1 + 1", "4")]
        [TestCase("0 + 1", "1")]
        [TestCase("-1 + 1", "0")]
        [TestCase("1 + -1", "0")]

        // Division
        [TestCase("1 / 1", "1")]
        [TestCase("0 / 1", "0")]
        [TestCase("1 / 0", "∞")]
        [TestCase("-1 / 0", "-∞")]
        [TestCase("3 * 3 / 3", "3")]
        [TestCase("3 / 3 * 3", "3")]
        [TestCase("1 / 3", "0.333333333")]
        [TestCase("(2 - 1)/(2 + 1)", "0.333333333")]
        [TestCase("1 ÷ 1", "1")]
        [TestCase("0 ÷ 1", "0")]
        [TestCase("1 ÷ 0", "∞")]
        [TestCase("-1 ÷ 0", "-∞")]
        [TestCase("3 * 3 ÷ 3", "3")]
        [TestCase("3 ÷ 3 * 3", "3")]
        [TestCase("1 ÷ 3", "0.333333333")]
        [TestCase("(2 - 1)÷(2 + 1)", "0.333333333")]

        // Multiplication
        [TestCase("2 * 2", "4")]
        [TestCase("2 * 0", "0")]
        [TestCase("0 * 2", "0")]
        [TestCase("1 * -2", "-2")]

        // Exponentiation
        [TestCase("0^2", "0")]
        [TestCase("-2^2", "-4")]
        [TestCase("--2^2", "4")]
        [TestCase("(1 + 1)^2", "4")]
        [TestCase("2.2^2", "4.84")]
        [TestCase("(2 + 2^2)^3 / 5", "43.2")]
        [TestCase("3 + 4 * 2 / ( 1 - 5 )^2^3", "3.00012207")]
        [TestCase("3 + 4 * 2 ÷ ( 1 − 5 ) ^ 2 ^ 3", "3.00012207")]
        [TestCase("2^0", "1")]

        // Factorials
        [TestCase("0!", "1")]
        [TestCase("1!", "1")]
        [TestCase("2!", "2")]
        [TestCase("4!", "24")]
        [TestCase("5!", "120")]
        [TestCase("25!", "1.5511210043330984E+25")]
        [TestCase("171!", "∞")]
        [TestCase("-2!", "-2")]
        [TestCase("5 + 4! / 2! - 5", "12")]

        // Order of operations
        [TestCase("36 * 12 + 3 / 5 - 2", "430.6")]
        [TestCase("2 * (3 * 2^2)", "24")]
        [TestCase("12 - ((8 + 7) * 2) / 6", "7")]
        [TestCase("24 - 3 * 4 / 2 + 6", "24")]
        [TestCase("6^2 - 5^2 + 1^2", "12")]
        [TestCase("2 + 8 / 2", "6")]
        [TestCase("20 / 5 * 2", "8")]
        [TestCase("2 + 2 * 2 + 2 / 2", "7")]
        [TestCase("12 - (2 * 4 + 1)", "3")]
        [TestCase("34 * 3 + 2 / 2 - (5 + 2)", "96")]
        public void TryParse_WhenGivenValidMathExpression_ShouldReturnCorrectResult(string expression, string expected)
        {
            bool success = ShuntingYardAlgorithm.TryParse(expression, out string actual);
            Assert.True(success);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase("hello world")]
        [TestCase("2a + 2")]
        [TestCase("0 < 2")]
        [TestCase("!2")]
        [TestCase("2 + [3 -2]")]
        [TestCase("2 + {3 -2}")]
        [TestCase("2 | 3}")]
        [TestCase("+2 - 1")]
        [TestCase("++2")]
        [TestCase("*2")]
        [TestCase("/2")]
        [TestCase("^2")]
        [TestCase(")2 + 2")]
        [TestCase(" 2 + 2")]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("!")]
        [Parallelizable(ParallelScope.All)]
        public void TryParse_WhenGivenInvalidMathExpression_ShouldReturnNull(string expression)
        {
            bool success = ShuntingYardAlgorithm.TryParse(expression, out string actual);
            Assert.False(success);
            Assert.Null(actual);
        }

        [Test]
        [TestCase("(2 +2")]
        [TestCase("2 + 2)")]
        [TestCase("((2 + 2)")]
        [TestCase("2 ++ 2")]
        [TestCase("2 // 2")]
        [TestCase("2 ** 2")]
        [TestCase("2^^2")]
        [TestCase("2.2!")]
        [TestCase("2^")]
        [TestCase("2^!")]
        [TestCase("2^ ")]
        [TestCase("2^.")]
        [TestCase("2 + ...")]
        [TestCase("2 + .")]
        [TestCase(".")]
        [TestCase("2 −")]
        [TestCase("2 -")]
        [TestCase("2 +")]
        [TestCase("2 ÷")]
        [TestCase("2 /")]
        [TestCase("2 *")]
        [TestCase("2 ^")]
        [TestCase("2 + .2!")]
        [Parallelizable(ParallelScope.All)]
        public void TryParse_WhenGivenImproperMathExpression_ShouldReturnEllipsis(string expression)
        {
            bool success = ShuntingYardAlgorithm.TryParse(expression, out string actual);
            Assert.False(success);
            Assert.AreEqual("...", actual);
        }
    }
}
