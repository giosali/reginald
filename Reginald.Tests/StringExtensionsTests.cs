using Reginald.Extensions;
using System;
using System.Text.RegularExpressions;
using Xunit;

namespace Reginald.Tests
{
    public class StringExtensionsTests
    {
        // Partition(string)

        [Fact]
        public static void Partition_ShouldReturnTupleOfThreeStrings_String()
        {
            string value = "A string.";
            var result = value.Partition(String.Empty);

            Assert.True(result is (string, string, string));
        }

        [Fact]
        public static void Partition_NoMatchShouldReturnEmptyStrings_String()
        {
            string expected = String.Empty;

            string value = "This is a string";
            var result = value.Partition("test");

            Assert.Equal(expected, result.Separator);
            Assert.Equal(expected, result.Right);
        }

        [Fact]
        public static void Partition_ShouldReturnFirstHalfSeparatorSecondHalf_String()
        {
            string value = "This is a string";
            string separator = "is";
            var result = value.Partition(separator);
            string[] results = value.Split(separator, 2);

            Assert.Equal(result.Left, results[0]);
            Assert.Equal(result.Separator, separator);
            Assert.Equal(result.Right, results[1]);
        }

        // Partition(char)

        [Fact]
        public static void Partition_ShouldReturnTupleOfThreeStrings_Char()
        {
            string value = "A string.";
            var result = value.Partition('a');

            Assert.True(result is (string, string, string));
        }

        [Fact]
        public static void Partition_NoMatchShouldReturnEmptyStrings_Char()
        {
            string expected = String.Empty;

            string value = "This is a string";
            var result = value.Partition('z');

            Assert.Equal(expected, result.Separator);
            Assert.Equal(expected, result.Right);
        }

        [Fact]
        public static void Partition_ShouldReturnFirstHalfSeparatorSecondHalf_Char()
        {
            string value = "This is a string";
            char separator = 'a';
            var result = value.Partition(separator);
            string[] results = value.Split(separator, 2);

            Assert.Equal(result.Left, results[0]);
            Assert.Equal(result.Separator, separator.ToString());
            Assert.Equal(result.Right, results[1]);
        }

        // RPartition(string)

        [Fact]
        public static void RPartition_ShouldReturnTupleOfThreeStrings_String()
        {
            string value = "A string.";
            var result = value.RPartition(String.Empty);

            Assert.True(result is (string, string, string));
        }

        [Fact]
        public static void RPartition_NoMatchShouldReturnEmptyStrings_String()
        {
            string expected = String.Empty;

            string value = "This is a string";
            var result = value.RPartition("test");

            Assert.Equal(expected, result.Left);
            Assert.Equal(expected, result.Separator);
        }

        // RPartition(char)

        [Fact]
        public static void RPartition_ShouldReturnTupleOfThreeStrings_Char()
        {
            string value = "A string.";
            var result = value.RPartition('a');

            Assert.True(result is (string, string, string));
        }

        [Fact]
        public static void RPartition_NoMatchShouldReturnEmptyStrings_Char()
        {
            string expected = String.Empty;

            string value = "This is a string";
            var result = value.RPartition('z');

            Assert.Equal(expected, result.Left);
            Assert.Equal(expected, result.Separator);
        }

        [Theory]
        [InlineData("2 + 2", 4)]
        [InlineData("2 - 2", 0)]
        [InlineData("2 * 2", 4)]
        [InlineData("2 / 2", 1)]
        [InlineData("2^2", 4)]
        [InlineData("(2 + 2)", 4)]
        [InlineData("34 * 3 + 2 / 2 - (5 + 2)", 96)]
        public static void Eval_SimpleExpressionsShouldBeEvaluated(string expression, double expected)
        {
            double actual = double.Parse(expression.Eval());
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("453654632574325832748364367436473628462376426432647326426464783264264376426637644556874377674676768736746674657465634756438783856784735673653657658345736585673563857346574358365783565876543656358765765836753567674634632467264827646478236474326487346846327428647326842674628467324823647823648764784687264782364294808489473756727432804390483774374882742", "∞")]
        [InlineData("5 / 0", "+∞")]
        [InlineData("30 + 5 + (5 / 0) + 2", "∞")]
        [InlineData("-5 / 0", "-∞")]
        [InlineData("30 + 5 - (5 / 0) + 2", "-∞")]
        [InlineData("5 + (", "...")]
        [InlineData("5 > 0", "True")]
        [InlineData("5 < 0", "False")]
        [InlineData("3000.3000.3000", "...")]
        public static void Eval_ComplexExpressionsShouldReturnUndefined(string expression, string expected)
        {
            string actual = expression.Eval();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("2 + 2")]
        [InlineData("2 + (2 - 2)")]
        [InlineData("2 / 0")]
        [InlineData("2-2")]
        [InlineData("2^2")]
        [InlineData("2 * 2")]
        [InlineData("40 - 3 + 1 * (1 + 5)")]
        public static void IsMathExpression_ExpressionsShouldReturnTrue(string expression)
        {
            Assert.True(expression.IsMathExpression());
        }

        [Theory]
        [InlineData("2a + 2")]
        [InlineData("2 + 2 + @")]
        [InlineData("2 + #")]
        public static void IsMathExpression_ExpressionsShouldReturnFalse(string expression)
        {
            Assert.False(expression.IsMathExpression());
        }

        [Fact]
        public static void Quote_ShouldReturnString()
        {
            bool expected = true;

            string expression = "This is a string";
            bool actual = expression.Quote("+") is string;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("This is a string", "+", "This+is+a+string")]
        [InlineData("Thisisastring", "+", "Thisisastring")]
        [InlineData("This is a string", " ", "This is a string")]
        public static void Quote_WhitespaceCharactersShouldBeReplaced(string expression, string replacement, string expected)
        {
            Assert.Equal(expected, expression.Quote(replacement));
        }

        [Theory]
        [InlineData("google.com")]
        [InlineData("https://google.com/")]
        [InlineData("www.co")]
        public static void HasTopLevelDomain_ShouldReturnTrue(string expression)
        {
            bool actual = expression.HasTopLevelDomain();
            Assert.True(actual);
        }

        [Theory]
        [InlineData("website")]
        [InlineData("google.c")]
        public static void HasTopLevelDomain_ShouldReturnFalse(string expression)
        {
            bool actual = expression.HasTopLevelDomain();
            Assert.False(actual);
        }

        [Theory]
        [InlineData("https://")]
        [InlineData("http://")]
        [InlineData("HTTPS://")]
        [InlineData("HTTP://")]
        public static void HasScheme_ShouldReturnTrue(string expression)
        {
            bool actual = expression.HasScheme();
            Assert.True(actual);
        }

        [Theory]
        [InlineData("ftp://")]
        [InlineData("https")]
        [InlineData("https:/")]
        public static void HasScheme_ShouldReturnFalse(string expression)
        {
            bool actual = expression.HasScheme();
            Assert.False(actual);
        }

        [Theory]
        [InlineData("google.com/")]
        [InlineData("0")]
        public static void PrependScheme_ShouldPrependScheme(string expression)
        {
            string result = expression.PrependScheme();
            bool actual = result.StartsWith("https://");
            Assert.True(actual);
        }

        [Fact]
        public static void PrependScheme_ShouldNotPrependScheme()
        {
            int expected = 1;

            string expression = "https://google.com/";
            string result = expression.PrependScheme();
            Regex rx = new(@"https://");
            MatchCollection matches = rx.Matches(result);
            int actual = matches.Count;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("5!", "120")]
        [InlineData("4!", "24")]
        [InlineData("3!", "6")]
        [InlineData("2!", "2")]
        [InlineData("1!", "1")]
        [InlineData("0!", "1")]
        public static void Factorial_ShouldCalculateCorrectly(string expression, string expected)
        {
            Assert.Equal(expression.Factorial().Eval(), expected);
        }

        [Theory]
        [InlineData("2 + 5!", "122")]
        [InlineData("5! + 5!", "240")]
        public static void Factorial_ShouldCalculate(string expression, string expected)
        {
            Assert.Equal(expression.Factorial().Eval(), expected);
        }

        [Theory]
        [InlineData("5.5!")]
        [InlineData("5.55!")]
        [InlineData("-5!")]
        public static void Factorial_ShouldNotCalculate(string expression)
        {
            string expected = "...";
            Assert.Equal(expression.Factorial().Eval(), expected);
        }
    }
}
