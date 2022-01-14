namespace Reginald.Tests
{
    using System.Text.RegularExpressions;
    using Reginald.Extensions;
    using Xunit;

    public class StringExtensionsTests
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
            string actual = input.RegexClean();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("This is a string", "This is a string")]
        [InlineData("`~!@#$%^&*-_=,<>/?;:'\"{]}|", "`~!@#$%^&*-_=,<>/?;:'\"{]}|")]
        public static void RegexClean_WhenNotGivenUnmatchedOrUnterminatedCharacters_ShouldCleanString(string input, string expected)
        {
            string actual = input.RegexClean();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("hello world", "hello", " ", "world")]
        [InlineData("hello", "he", "l", "lo")]
        public static void Partition_WhenGivenStringFoundInString_ShouldReturnSplitString(string expression, string left, string separator, string right)
        {
            (string, string, string) partition = expression.Partition(separator);
            Assert.Equal((left, separator, right), partition);
        }

        [Fact]
        public static void Partition_WhenGivenStringNotFoundInString_ShouldReturnTupleWithEmptyStrings()
        {
            string expression = "hello world";
            string separator = "z";
            (string Left, string Separator, string Right) partition = expression.Partition(separator);
            Assert.Equal(expression, partition.Left);
            Assert.Equal(string.Empty, partition.Separator);
            Assert.Equal(string.Empty, partition.Right);
        }

        [Theory]
        [InlineData("hello world", "hello", ' ', "world")]
        [InlineData("hello", "he", 'l', "lo")]
        public static void Partition_WhenGivenCharFoundInString_ShouldReturnSplitString(string expression, string left, char separator, string right)
        {
            (string, string, string) partition = expression.Partition(separator);
            Assert.Equal((left, separator.ToString(), right), partition);
        }

        [Fact]
        public static void Partition_WhenGivenCharNotFoundInString_ShouldReturnTupleWithEmptyStrings()
        {
            string expression = "hello world";
            char separator = 'z';
            (string Left, string Separator, string Right) partition = expression.Partition(separator);
            Assert.Equal(expression, partition.Left);
            Assert.Equal(string.Empty, partition.Separator);
            Assert.Equal(string.Empty, partition.Right);
        }

        [Theory]
        [InlineData("hello world", "hello", " ", "world")]
        [InlineData("hello", "hel", "l", "o")]
        public static void RPartition_WhenGivenStringFoundInString_ShouldReturnSplitString(string expression, string left, string separator, string right)
        {
            (string, string, string) partition = expression.RPartition(separator);
            Assert.Equal((left, separator, right), partition);
        }

        [Fact]
        public static void RPartition_WhenGivenStringNotFoundInString_ShouldReturnTupleWithEmptyStrings()
        {
            string expression = "hello world";
            string separator = "z";
            (string Left, string Separator, string Right) partition = expression.RPartition(separator);
            Assert.Equal(string.Empty, partition.Left);
            Assert.Equal(string.Empty, partition.Separator);
            Assert.Equal(expression, partition.Right);
        }

        [Theory]
        [InlineData("hello world", "hello", ' ', "world")]
        [InlineData("hello", "hel", 'l', "o")]
        public static void RPartition_WhenGivenCharFoundInString_ShouldReturnSplitString(string expression, string left, char separator, string right)
        {
            (string, string, string) partition = expression.RPartition(separator);
            Assert.Equal((left, separator.ToString(), right), partition);
        }

        [Fact]
        public static void RPartition_WhenGivenCharNotFoundInString_ShouldReturnTupleWithEmptyStrings()
        {
            string expression = "hello world";
            char separator = 'z';
            (string Left, string Separator, string Right) partition = expression.RPartition(separator);
            Assert.Equal(string.Empty, partition.Left);
            Assert.Equal(string.Empty, partition.Separator);
            Assert.Equal(expression, partition.Right);
        }

        [Theory]
        [InlineData("This is a string", "+", "This+is+a+string")]
        [InlineData("Thisisastring", "+", "Thisisastring")]
        [InlineData("This is a string", " ", "This is a string")]
        public static void Quote_WhitespaceCharactersShouldBeReplaced(string expression, string replacement, string expected)
        {
            string actual = expression.Quote(replacement);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("www.co")]
        [InlineData("google.co")]
        [InlineData("google.com")]
        [InlineData("google.com/")]
        [InlineData("google.com/h")]
        [InlineData("company.com/showOrder.php?order=4621047")]
        [InlineData("company.com/companyInfo?name=C&H Sugar")]
        [InlineData("host.company.com/companyInfo?name=C&H Sugar")]
        [InlineData("host.company.com/showCompanyInfo?name=C%26H%20Sugar")]
        [InlineData("example.com/index.html")]
        [InlineData("www.google.com/h")]
        [InlineData("www.company.com/showOrder.php?order=4621047")]
        [InlineData("www.company.com/companyInfo?name=C&H Sugar")]
        [InlineData("www.example.com/index.html")]
        [InlineData("https://google.com")]
        [InlineData("https://google.com/")]
        [InlineData("https://www.google.com")]
        [InlineData("https://www.google.com/")]
        [InlineData("https://domains.google.com")]
        [InlineData("https://domains.google.com/")]
        [InlineData("https://www.domains.google.com")]
        [InlineData("https://www.domains.google.com/")]
        [InlineData("google.co.uk")]
        [InlineData("https://google.co.uk")]
        [InlineData("https://google.co.uk/")]
        [InlineData("https://www.google.co.uk/")]
        [InlineData("http://www.company.com/showOrder.php?order=4621047")]
        [InlineData("http://company.com/companyInfo?name=C&H Sugar")]
        [InlineData("http://host.company.com/companyInfo?name=C&H Sugar")]
        [InlineData("http://host.company.com/showCompanyInfo?name=C%26H%20Sugar")]
        [InlineData("http://www.example.com/index.html")]
        public static void ContainsTopLevelDomain_WhenGivenTopLevelDomain_ShouldReturnTrue(string expression)
        {
            bool condition = expression.ContainsTopLevelDomain();
            Assert.True(condition);
        }

        [Theory]
        [InlineData("website")]
        [InlineData("example.c")]
        [InlineData("example.abcdefghijk")]
        [InlineData("http://www.company.com:81/a/b/c.html?user=Alice&year=2008#p2")]
        [InlineData("Hello world")]
        public static void ContainsTopLevelDomain_WhenNotGivenTopLevelDomain_ShouldReturnFalse(string expression)
        {
            bool condition = expression.ContainsTopLevelDomain();
            Assert.False(condition);
        }

        [Theory]
        [InlineData("https://")]
        [InlineData("http://")]
        [InlineData("HTTPS://")]
        [InlineData("HTTP://")]
        public static void StartsWithScheme_WhenGivenScheme_ShouldReturnTrue(string expression)
        {
            bool condition = expression.StartsWithScheme();
            Assert.True(condition);
        }

        [Theory]
        [InlineData("ftp://")]
        [InlineData("https")]
        [InlineData("https:/")]
        [InlineData("hellohttps://")]
        public static void StartsWithScheme_WhenNotGivenScheme_ShouldReturnFalse(string expression)
        {
            bool condition = expression.StartsWithScheme();
            Assert.False(condition);
        }

        [Theory]
        [InlineData("google.com/")]
        [InlineData("0")]
        public static void PrependScheme_ShouldPrependScheme(string expression)
        {
            string result = expression.PrependScheme();
            bool condition = result.StartsWith("https://");
            Assert.True(condition);
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
    }
}
