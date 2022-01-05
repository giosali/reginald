using Reginald.Core.Helpers;
using Xunit;

namespace Reginald.Tests
{
    public class StringHelpersTests
    {
        [Theory]
        [InlineData("(This is a string)", @"\(This is a string\)")]
        [InlineData("This is a string.", @"This is a string\.")]
        [InlineData(@"\", @"\\")]
        [InlineData("[]", @"\[]")]
        [InlineData(".", @"\.")]
        [InlineData("+", @"\+")]
        public static void RegexClean_WhenGivenUnmatchedOrUnterminatedCharacters_ShouldCleanString(string input, string expected)
        {
            string actual = StringHelper.RegexClean(input);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("This is a string", "This is a string")]
        [InlineData("`~!@#$%^&*-_=,<>/?;:'\"{]}|", "`~!@#$%^&*-_=,<>/?;:'\"{]}|")]
        public static void RegexClean_WhenNotGivenUnmatchedOrUnterminatedCharacters_ShouldCleanString(string input, string expected)
        {
            string actual = StringHelper.RegexClean(input);
            Assert.Equal(expected, actual);
        }
    }
}
