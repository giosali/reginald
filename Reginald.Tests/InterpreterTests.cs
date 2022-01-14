namespace Reginald.Tests
{
    using Reginald.Core.Mathematics;
    using Xunit;

    public class InterpreterTests
    {
        [Theory]
        [InlineData("2")]
        [InlineData("2^2")]
        [InlineData("2^2.2")]
        [InlineData("0.2^2")]
        [InlineData("0.2^2.2")]
        public static void IsMathExpression_WhenGivenValidMathExpression_ShouldReturnTrue(string input)
        {
            bool condition = Interpreter.IsMathExpression(input);
            Assert.True(condition);
        }

        [Theory]
        [InlineData("Hello world")]
        [InlineData("Hello world 2 + 2")]
        [InlineData("2.2.2")]
        [InlineData("2^2.2.2")]
        [InlineData("0.2.2^2")]
        public static void IsMathExpression_WhenGivenInvalidMathExpression_ShouldReturnFalse(string input)
        {
            bool condition = Interpreter.IsMathExpression(input);
            Assert.False(condition);
        }

        [Theory]
        [InlineData("2 / 0", "+∞")]
        [InlineData("2 + 2", "+∞")]
        [InlineData("5 / (0)", "+∞")]
        [InlineData("-5 / (0)", "-∞")]
        [InlineData("5 / (1 - 1)", "+∞")]
        [InlineData("-5 / (1 - 1)", "-∞")]
        public static void TryInterpretDivideByZeroException_WhenGivenValidMathExpression_ShouldReturnTrue(string input, string expected)
        {
            bool condition = Interpreter.TryInterpretDivideByZeroException(input, out string actual);
            Assert.True(condition);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("Hello world")]
        [InlineData("Hello world 2 + 2")]
        [InlineData("2+2a")]
        public static void TryInterpretDivideByZeroException_WhenGivenInvalidMathExpression_ShouldReturnFalse(string input)
        {
            bool condition = Interpreter.TryInterpretDivideByZeroException(input, out string actual);
            Assert.False(condition);
            Assert.Equal("...", actual);
        }

        [Theory]
        [InlineData("2^2", "4")]
        [InlineData("2.2^2.2", "5.666695778750081")]
        public static void TryInterpretEvaluateException_WhenGivenValidMathExpression_ShouldReturnTrue(string input, string expected)
        {
            bool condition = Interpreter.TryInterpretEvaluateException(input, out string actual);
            Assert.True(condition);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("2")]
        [InlineData("2 + 2")]
        [InlineData("Hello world")]
        [InlineData("2!")]
        [InlineData("$5")]
        [InlineData("2^")]
        [InlineData("2.2^")]
        [InlineData("hello^")]
        public static void TryInterpretEvaluateException_WhenGivenInvalidMathExpression_ShouldReturnFalse(string input)
        {
            bool condition = Interpreter.TryInterpretEvaluateException(input, out string actual);
            Assert.False(condition);
            Assert.Equal("...", actual);
        }

        [Theory]
        [InlineData("1!", "1")]
        [InlineData("0!", "1")]
        [InlineData("5!", "120")]
        [InlineData("6!", "720")]
        [InlineData("7!", "5040")]
        [InlineData("25!", "1.5511210043330984E+25")]
        public static void TryInterpretSyntaxErrorException_WhenGivenValidMathExpression_ShouldReturnTrue(string input, string expected)
        {
            bool condition = Interpreter.TryInterpretSyntaxErrorException(input, out string actual);
            Assert.True(condition);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("2 + 2")]
        [InlineData("hello world")]
        [InlineData("hello!")]
        [InlineData("2.2!")]
        public static void TryInterpretSyntaxErrorException_WhenGivenInvalidMathExpression_ShouldReturnFalse(string input)
        {
            bool condition = Interpreter.TryInterpretSyntaxErrorException(input, out string actual);
            Assert.False(condition);
            Assert.Equal("...", actual);
        }
    }
}
