namespace Reginald.Tests
{
    using System.Text.RegularExpressions;
    using Reginald.Core.Extensions;
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
        [InlineData("*", @"\*")]
        [InlineData("?", @"\?")]
        [InlineData("|", @"\|")]
        [InlineData("$", @"\$")]
        public static void RegexClean_WhenGivenUnmatchedOrUnterminatedCharacters_ShouldCleanString(string input, string expected)
        {
            string actual = input.RegexClean();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("This is a string", "This is a string")]
        [InlineData("`~!@#%^&-_=,<>/;:'\"{]}", "`~!@#%^&-_=,<>/;:'\"{]}")]
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
        [InlineData("This is a string", true, "This+is+a+string")]
        [InlineData("Thisisastring", true, "Thisisastring")]
        [InlineData("This is a string", false, "This%20is%20a%20string")]
        [InlineData(@"`~!@#$%^&*()-_=+[{]}\|;:',<.>/?", true, "%60%7e!%40%23%24%25%5e%26*()-_%3d%2b%5b%7b%5d%7d%5c%7c%3b%3a%27%2c%3c.%3e%2f%3f")]
        [InlineData(@"`~!@#$%^&*()-_=+[{]}\|;:',<.>/?", false, "%60~%21%40%23%24%25%5E%26%2A%28%29-_%3D%2B%5B%7B%5D%7D%5C%7C%3B%3A%27%2C%3C.%3E%2F%3F")]
        public static void Quote_WhitespaceCharactersShouldBeReplaced(string expression, bool useUtf8, string expected)
        {
            string actual = expression.Quote(useUtf8);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("https://google.com")]
        [InlineData("https://google.com/")]
        [InlineData("https://www.google.com")]
        [InlineData("https://www.google.com/")]
        [InlineData("https://domains.google.com")]
        [InlineData("https://domains.google.com/")]
        [InlineData("https://www.domains.google.com")]
        [InlineData("https://www.domains.google.com/")]
        [InlineData("https://google.co.uk")]
        [InlineData("https://google.co.uk/")]
        [InlineData("https://www.google.co.uk/")]
        [InlineData("http://www.company.com/showOrder.php?order=4621047")]
        [InlineData("http://company.com/companyInfo?name=C&H Sugar")]
        [InlineData("http://host.company.com/companyInfo?name=C&H Sugar")]
        [InlineData("http://host.company.com/showCompanyInfo?name=C%26H%20Sugar")]
        [InlineData("http://www.example.com/index.html")]
        [InlineData("http://www.company.com:81/a/b/c.html?user=Alice&year=2008#p2")]
        [InlineData("https://chart.apis.google.com/chart?chs=500x500&chma=0,0,100,100&cht=p&chco=FF0000%2CFFFF00%7CFF8000%2C00FF00%7C00FF00%2C0000FF&chd=t%3A122%2C42%2C17%2C10%2C8%2C7%2C7%2C7%2C7%2C6%2C6%2C6%2C6%2C5%2C5&chl=122%7C42%7C17%7C10%7C8%7C7%7C7%7C7%7C7%7C6%7C6%7C6%7C6%7C5%7C5&chdl=android%7Cjava%7Cstack-trace%7Cbroadcastreceiver%7Candroid-ndk%7Cuser-agent%7Candroid-webview%7Cwebview%7Cbackground%7Cmultithreading%7Candroid-source%7Csms%7Cadb%7Csollections%7Cactivity|Chart")]
        public static void ContainsTopLevelDomain_WhenGivenTopLevelDomain_ShouldReturnTrue(string expression)
        {
            bool condition = expression.ContainsTopLevelDomain();
            Assert.True(condition);
        }

        [Theory]
        [InlineData("website")]
        [InlineData("example.c")]
        [InlineData("example.abcdefghijk")]
        [InlineData("Hello world")]
        public static void ContainsTopLevelDomain_WhenNotGivenTopLevelDomain_ShouldReturnFalse(string expression)
        {
            bool condition = expression.ContainsTopLevelDomain();
            Assert.False(condition);
        }

        [Theory]
        [InlineData("www.co")]
        [InlineData("google.co")]
        [InlineData("google.com")]
        [InlineData("google.com/")]
        [InlineData("google.com/h")]
        [InlineData("www.google.com/h")]
        [InlineData("google.co.uk")]
        [InlineData("company.com/showOrder.php?order=4621047")]
        [InlineData("company.com/companyInfo?name=C&H Sugar")]
        [InlineData("example.com/index.html")]
        [InlineData("host.company.com/companyInfo?name=C&H Sugar")]
        [InlineData("host.company.com/showCompanyInfo?name=C%26H%20Sugar")]
        [InlineData("www.company.com/showOrder.php?order=4621047")]
        [InlineData("www.company.com/companyInfo?name=C&H Sugar")]
        [InlineData("www.example.com/index.html")]
        [InlineData("this .ne")]
        public static void ContainsTopLevelDomain_WhenGivenTopLevelDomainButInvalidUri_ShouldReturnFalse(string expression)
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

        [Theory]
        [InlineData("Hello there, how are there you there?", "there", "NEW", 1, "Hello NEW, how are there you there?")]
        [InlineData("Hello there, how are there you there?", "there", "NEW", 2, "Hello NEW, how are NEW you there?")]
        [InlineData("Hello there, how are there you there?", "there", "NEW", 3, "Hello NEW, how are NEW you NEW?")]
        [InlineData("Falling through wet forests", "forests", "", 1, "Falling through wet ")]
        public static void Replace_WhenGiveCount_ShouldReplaceCountOccurrencesOfValue(string expression, string oldValue, string newValue, int count, string expected)
        {
            string actual = expression.Replace(oldValue, newValue, count);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("Hello there, how are there you there?", "there", "NEW", 4, "Hello NEW, how are NEW you NEW?")]
        [InlineData("Hello", "z", "a", 1, "Hello")]
        [InlineData("PigsOnTheW", "Wing", "wig", 1, "PigsOnTheW")]
        public static void Replace_WhenGivenCountGreaterThanOccurrences_ShouldNotThrow(string expression, string oldValue, string newValue, int count, string expected)
        {
            string actual = expression.Replace(oldValue, newValue, count);
            Assert.Equal(expected, actual);
        }
    }
}
