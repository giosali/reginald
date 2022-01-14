namespace Reginald.Tests
{
    using Reginald.Core.Mathematics;
    using Xunit;

    public class CalculatorTests
    {
        [Theory]
        [InlineData("2 + 2", "4")]
        [InlineData("2 - 2", "0")]
        [InlineData("2 * 2", "4")]
        [InlineData("2 / 2", "1")]
        [InlineData("(2 + 2)", "4")]
        public static void Calculate_WhenGivenSimpleExpressions_ShouldEvaluate(string input, string expected)
        {
            string actual = Calculator.Calculate(input);
            Assert.Equal(expected, actual);
        }

        [Theory]
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
        public static void Calculate_ShouldObeyOrderOfOperationsAndEvaluate(string input, string expected)
        {
            string actual = Calculator.Calculate(input);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("453654632574325832748364367436473628462376426432647326426464783264264376426637644556874377674676768736746674657465634756438783856784735673653657658345736585673563857346574358365783565876543656358765765836753567674634632467264827646478236474326487346846327428647326842674628467324823647823648764784687264782364294808489473756727432804390483774374882742", "∞")]
        [InlineData("5 / 0", "+∞")]
        [InlineData("30 + 5 + (5 / 0) + 2", "∞")]
        [InlineData("-5 / 0", "-∞")]
        [InlineData("30 + 5 - (5 / 0) + 2", "-∞")]
        [InlineData("5 + (", "...")]
        [InlineData("3000.3000.3000", "...")]
        [InlineData("5 / (0)", "+∞")]
        [InlineData("5 / (1 - 1)", "+∞")]
        [InlineData("-5 / (1 - 1)", "-∞")]
        [InlineData("10783330!", "...")]
        [InlineData("5.5!", "...")]
        [InlineData("-1!", "...")]
        public static void Calculate_WhenGivenUnusualExpressions_ShouldReturnUndefined(string input, string expected)
        {
            string actual = Calculator.Calculate(input);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("5 > 0", "True")]
        [InlineData("5 < 0", "False")]
        [InlineData("5 / 2 > 3", "False")]
        public static void Calculate_WhenGivenBooleanExpressions_ShouldEvaluate(string input, string expected)
        {
            string actual = Calculator.Calculate(input);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("1!", "1")]
        [InlineData("0!", "1")]
        [InlineData("25!", "1.5511210043330984E+25")]
        [InlineData("5! + 5", "125")]
        [InlineData("4 / 2 * 4!", "48")]
        [InlineData("7! / (0! * 4!)", "210")]
        [InlineData("4! + 3!", "30")]
        public static void Calculate_WhenGivenFactorials_ShouldEvaluate(string input, string expected)
        {
            string actual = Calculator.Calculate(input);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("3.14", "3.14")]
        [InlineData("1 + 0.5", "1.5")]
        [InlineData("2.5 / 2", "1.25")]
        [InlineData("1.0 * 1.0", "1")]
        [InlineData("2.3843784734379489474", "2.384378473")]
        [InlineData("0.12345678910", "0.123456789")]
        public static void Calculate_WhenGivenDecimals_ShouldEvaluate(string input, string expected)
        {
            string actual = Calculator.Calculate(input);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("1!", "1")]
        [InlineData("0!", "1")]
        [InlineData("5!", "120")]
        [InlineData("6!", "720")]
        [InlineData("7!", "5040")]
        [InlineData("25!", "1.5511210043330984E+25")]
        public static void Factorialize_WhenGivenValidFactorials_ShouldEvaluate(string input, string expected)
        {
            string actual = Calculator.Factorialize(input);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("2.2!", "2.2!")]
        [InlineData("-4!", "-4!")]
        [InlineData("hello!", "hello!")]
        [InlineData("hello", "hello")]
        public static void Factorialize_WhenGivenInvalidFactorials_ShouldEvaluate(string input, string expected)
        {
            string actual = Calculator.Factorialize(input);
            Assert.Equal(expected, actual);
        }
    }
}
