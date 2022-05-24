namespace Reginald.Tests
{
    using Reginald.Core.Math;
    using Xunit;

    public class ShuntingYardAlgorithmTests
    {
        [Theory]

        // Numbers
        [InlineData("0", "0")]
        [InlineData("1", "1")]
        [InlineData("10000000000000000", "10000000000000000")]
        [InlineData("100000000000000000", "1E+17")]
        [InlineData("100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", "1E+308")]
        [InlineData("100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000.123456789", "1E+308")]
        [InlineData("1000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", "∞")]
        [InlineData("-1000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", "-∞")]
        [InlineData("1000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000^2", "∞")]
        [InlineData("-1000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000^2", "-∞")]

        // Decimals
        [InlineData("1.0", "1")]
        [InlineData("1.1", "1.1")]
        [InlineData("0.1", "0.1")]
        [InlineData(".1", "0.1")]
        [InlineData("1.1234567891", "1.123456789")]
        [InlineData("0.0000000001", "0")]
        [InlineData(".4(4)", "1.6")]

        // Negatives
        [InlineData("-0", "0")]
        [InlineData("-1", "-1")]
        [InlineData("--1", "1")]
        [InlineData("---1", "-1")]
        [InlineData("----1", "1")]
        [InlineData("5 - -4", "9")]
        [InlineData("5 - -4 * 3", "17")]
        [InlineData("5 - -(4 * 3)", "17")]
        [InlineData("5 + -(2)(3 - 2 - 1 + 5)", "-5")]
        [InlineData("−0", "0")]
        [InlineData("−1", "-1")]
        [InlineData("−−1", "1")]
        [InlineData("−−−1", "-1")]
        [InlineData("−−−−1", "1")]
        [InlineData("5 − −4", "9")]
        [InlineData("5 − −4 * 3", "17")]
        [InlineData("5 − −(4 * 3)", "17")]
        [InlineData("5 + −(2)(3 − 2 − 1 + 5)", "-5")]

        // Parentheses
        [InlineData("(1)", "1")]
        [InlineData("(1.1)", "1.1")]
        [InlineData("(2)(2)", "4")]
        [InlineData("2(2 + 2) + 5", "13")]
        [InlineData("(2(2 + 2)) + 5", "13")]
        [InlineData("(3 + 5) * 6", "48")]
        [InlineData("((((((1))))))", "1")]
        [InlineData("(2 - 5)(4 / 2)", "-6")]
        [InlineData("-(1 + 1)", "-2")]

        // Subtraction
        [InlineData("2 - 2", "0")]
        [InlineData("-1 - 1", "-2")]
        [InlineData("2 − 2", "0")]
        [InlineData("-1 − 1", "-2")]

        // Addition
        [InlineData("1 + 1 + 1 + 1", "4")]
        [InlineData("0 + 1", "1")]
        [InlineData("-1 + 1", "0")]
        [InlineData("1 + -1", "0")]

        // Division
        [InlineData("1 / 1", "1")]
        [InlineData("0 / 1", "0")]
        [InlineData("1 / 0", "∞")]
        [InlineData("-1 / 0", "-∞")]
        [InlineData("3 * 3 / 3", "3")]
        [InlineData("3 / 3 * 3", "3")]
        [InlineData("1 / 3", "0.333333333")]
        [InlineData("(2 - 1)/(2 + 1)", "0.333333333")]
        [InlineData("1 ÷ 1", "1")]
        [InlineData("0 ÷ 1", "0")]
        [InlineData("1 ÷ 0", "∞")]
        [InlineData("-1 ÷ 0", "-∞")]
        [InlineData("3 * 3 ÷ 3", "3")]
        [InlineData("3 ÷ 3 * 3", "3")]
        [InlineData("1 ÷ 3", "0.333333333")]
        [InlineData("(2 - 1)÷(2 + 1)", "0.333333333")]

        // Multiplication
        [InlineData("2 * 2", "4")]
        [InlineData("2 * 0", "0")]
        [InlineData("0 * 2", "0")]
        [InlineData("1 * -2", "-2")]

        // Exponentiation
        [InlineData("0^2", "0")]
        [InlineData("-2^2", "-4")]
        [InlineData("--2^2", "4")]
        [InlineData("(1 + 1)^2", "4")]
        [InlineData("2.2^2", "4.84")]
        [InlineData("(2 + 2^2)^3 / 5", "43.2")]
        [InlineData("3 + 4 * 2 / ( 1 - 5 )^2^3", "3.00012207")]
        [InlineData("3 + 4 * 2 ÷ ( 1 − 5 ) ^ 2 ^ 3", "3.00012207")]
        [InlineData("2^0", "1")]

        // Factorials
        [InlineData("0!", "1")]
        [InlineData("1!", "1")]
        [InlineData("2!", "2")]
        [InlineData("4!", "24")]
        [InlineData("5!", "120")]
        [InlineData("25!", "1.5511210043330984E+25")]
        [InlineData("171!", "∞")]
        [InlineData("-2!", "-2")]
        [InlineData("5 + 4! / 2! - 5", "12")]

        // Order of operations
        [InlineData("36 * 12 + 3 / 5 - 2", "430.6")]
        [InlineData("2 * (3 * 2^2)", "24")]
        [InlineData("12 - ((8 + 7) * 2) / 6", "7")]
        [InlineData("24 - 3 * 4 / 2 + 6", "24")]
        [InlineData("6^2 - 5^2 + 1^2", "12")]
        [InlineData("2 + 8 / 2", "6")]
        [InlineData("20 / 5 * 2", "8")]
        [InlineData("2 + 2 * 2 + 2 / 2", "7")]
        [InlineData("12 - (2 * 4 + 1)", "3")]
        [InlineData("34 * 3 + 2 / 2 - (5 + 2)", "96")]
        public void TryParse_WhenGivenValidMathExpression_ShouldReturnCorrectResult(string expression, string expected)
        {
            bool success = ShuntingYardAlgorithm.TryParse(expression, out string actual);
            Assert.True(success);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("hello world")]
        [InlineData("2a + 2")]
        [InlineData("0 < 2")]
        [InlineData("!2")]
        [InlineData("2 + [3 -2]")]
        [InlineData("2 + {3 -2}")]
        [InlineData("2 | 3}")]
        [InlineData("+2 - 1")]
        [InlineData("++2")]
        [InlineData("*2")]
        [InlineData("/2")]
        [InlineData("^2")]
        [InlineData(")2 + 2")]
        [InlineData(" 2 + 2")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("!")]
        public void TryParse_WhenGivenInvalidMathExpression_ShouldReturnNull(string expression)
        {
            bool success = ShuntingYardAlgorithm.TryParse(expression, out string actual);
            Assert.False(success);
            Assert.Null(actual);
        }

        [Theory]
        [InlineData("(2 +2")]
        [InlineData("2 + 2)")]
        [InlineData("((2 + 2)")]
        [InlineData("2 ++ 2")]
        [InlineData("2 // 2")]
        [InlineData("2 ** 2")]
        [InlineData("2^^2")]
        [InlineData("2.2!")]
        [InlineData("2^")]
        [InlineData("2^!")]
        [InlineData("2^ ")]
        [InlineData("2^.")]
        [InlineData("2 + ...")]
        [InlineData("2 + .")]
        [InlineData(".")]
        public void TryParse_WhenGivenImproperMathExpression_ShouldReturnEllipsis(string expression)
        {
            bool success = ShuntingYardAlgorithm.TryParse(expression, out string actual);
            Assert.False(success);
            Assert.Equal("...", actual);
        }
    }
}
