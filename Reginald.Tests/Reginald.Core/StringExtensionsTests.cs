namespace Reginald.Tests.Reginald.Core
{
    using System.Text.RegularExpressions;
    using global::Reginald.Core.Extensions;
    using NUnit.Framework;

    [TestFixture]
    public class StringExtensionsTests
    {
        [SetUp]
        public void Init()
        {
            _ = System.Windows.Application.Current;
        }

        [Test]
        [TestCase("(This is a string)", @"\(This is a string\)")]
        [TestCase("This is a string.", @"This is a string\.")]
        [TestCase(@"\", @"\\")]
        [TestCase("[]", @"\[]")]
        [TestCase(".", @"\.")]
        [TestCase("+", @"\+")]
        [TestCase("*", @"\*")]
        [TestCase("?", @"\?")]
        [TestCase("|", @"\|")]
        [TestCase("$", @"\$")]
        [Parallelizable(ParallelScope.All)]
        public void RegexClean_WhenGivenUnmatchedOrUnterminatedCharacters_ShouldCleanString(string input, string expected)
        {
            string actual = input.RegexClean();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase("This is a string", "This is a string")]
        [TestCase("`~!@#%^&-_=,<>/;:'\"{]}", "`~!@#%^&-_=,<>/;:'\"{]}")]
        [Parallelizable(ParallelScope.All)]
        public void RegexClean_WhenNotGivenUnmatchedOrUnterminatedCharacters_ShouldCleanString(string input, string expected)
        {
            string actual = input.RegexClean();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase("hello world", "hello", " ", "world")]
        [TestCase("hello", "he", "l", "lo")]
        [Parallelizable(ParallelScope.All)]
        public void Partition_WhenGivenStringFoundInString_ShouldReturnSplitString(string expression, string left, string separator, string right)
        {
            (string, string, string) partition = expression.Partition(separator);
            Assert.AreEqual((left, separator, right), partition);
        }

        [Test]
        public void Partition_WhenGivenStringNotFoundInString_ShouldReturnTupleWithEmptyStrings()
        {
            string expression = "hello world";
            string separator = "z";
            (string Left, string Separator, string Right) partition = expression.Partition(separator);
            Assert.AreEqual(expression, partition.Left);
            Assert.AreEqual(string.Empty, partition.Separator);
            Assert.AreEqual(string.Empty, partition.Right);
        }

        [Test]
        [TestCase("hello world", "hello", ' ', "world")]
        [TestCase("hello", "he", 'l', "lo")]
        [Parallelizable(ParallelScope.All)]
        public void Partition_WhenGivenCharFoundInString_ShouldReturnSplitString(string expression, string left, char separator, string right)
        {
            (string, string, string) partition = expression.Partition(separator);
            Assert.AreEqual((left, separator.ToString(), right), partition);
        }

        [Test]
        public void Partition_WhenGivenCharNotFoundInString_ShouldReturnTupleWithEmptyStrings()
        {
            string expression = "hello world";
            char separator = 'z';
            (string Left, string Separator, string Right) partition = expression.Partition(separator);
            Assert.AreEqual(expression, partition.Left);
            Assert.AreEqual(string.Empty, partition.Separator);
            Assert.AreEqual(string.Empty, partition.Right);
        }

        [Test]
        [TestCase("hello world", "hello", " ", "world")]
        [TestCase("hello", "hel", "l", "o")]
        [Parallelizable(ParallelScope.All)]
        public void RPartition_WhenGivenStringFoundInString_ShouldReturnSplitString(string expression, string left, string separator, string right)
        {
            (string, string, string) partition = expression.RPartition(separator);
            Assert.AreEqual((left, separator, right), partition);
        }

        [Test]
        public void RPartition_WhenGivenStringNotFoundInString_ShouldReturnTupleWithEmptyStrings()
        {
            string expression = "hello world";
            string separator = "z";
            (string Left, string Separator, string Right) partition = expression.RPartition(separator);
            Assert.AreEqual(string.Empty, partition.Left);
            Assert.AreEqual(string.Empty, partition.Separator);
            Assert.AreEqual(expression, partition.Right);
        }

        [Test]
        [TestCase("hello world", "hello", ' ', "world")]
        [TestCase("hello", "hel", 'l', "o")]
        [Parallelizable(ParallelScope.All)]
        public void RPartition_WhenGivenCharFoundInString_ShouldReturnSplitString(string expression, string left, char separator, string right)
        {
            (string, string, string) partition = expression.RPartition(separator);
            Assert.AreEqual((left, separator.ToString(), right), partition);
        }

        [Test]
        public void RPartition_WhenGivenCharNotFoundInString_ShouldReturnTupleWithEmptyStrings()
        {
            string expression = "hello world";
            char separator = 'z';
            (string Left, string Separator, string Right) partition = expression.RPartition(separator);
            Assert.AreEqual(string.Empty, partition.Left);
            Assert.AreEqual(string.Empty, partition.Separator);
            Assert.AreEqual(expression, partition.Right);
        }

        [Test]
        [TestCase("This is a string", true, "This+is+a+string")]
        [TestCase("Thisisastring", true, "Thisisastring")]
        [TestCase("This is a string", false, "This%20is%20a%20string")]
        [TestCase(@"`~!@#$%^&*()-_=+[{]}\|;:',<.>/?", true, "%60%7e!%40%23%24%25%5e%26*()-_%3d%2b%5b%7b%5d%7d%5c%7c%3b%3a%27%2c%3c.%3e%2f%3f")]
        [TestCase(@"`~!@#$%^&*()-_=+[{]}\|;:',<.>/?", false, "%60~%21%40%23%24%25%5E%26%2A%28%29-_%3D%2B%5B%7B%5D%7D%5C%7C%3B%3A%27%2C%3C.%3E%2F%3F")]
        [Parallelizable(ParallelScope.All)]
        public void Quote_WhitespaceCharactersShouldBeReplaced(string expression, bool useUtf8, string expected)
        {
            string actual = expression.Quote(useUtf8);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase("https://google.com")]
        [TestCase("https://google.com/")]
        [TestCase("https://www.google.com")]
        [TestCase("https://www.google.com/")]
        [TestCase("https://domains.google.com")]
        [TestCase("https://domains.google.com/")]
        [TestCase("https://www.domains.google.com")]
        [TestCase("https://www.domains.google.com/")]
        [TestCase("https://google.co.uk")]
        [TestCase("https://google.co.uk/")]
        [TestCase("https://www.google.co.uk/")]
        [TestCase("http://www.company.com/showOrder.php?order=4621047")]
        [TestCase("http://company.com/companyInfo?name=C&H Sugar")]
        [TestCase("http://host.company.com/companyInfo?name=C&H Sugar")]
        [TestCase("http://host.company.com/showCompanyInfo?name=C%26H%20Sugar")]
        [TestCase("http://www.example.com/index.html")]
        [TestCase("http://www.company.com:81/a/b/c.html?user=Alice&year=2008#p2")]
        [TestCase("https://chart.apis.google.com/chart?chs=500x500&chma=0,0,100,100&cht=p&chco=FF0000%2CFFFF00%7CFF8000%2C00FF00%7C00FF00%2C0000FF&chd=t%3A122%2C42%2C17%2C10%2C8%2C7%2C7%2C7%2C7%2C6%2C6%2C6%2C6%2C5%2C5&chl=122%7C42%7C17%7C10%7C8%7C7%7C7%7C7%7C7%7C6%7C6%7C6%7C6%7C5%7C5&chdl=android%7Cjava%7Cstack-trace%7Cbroadcastreceiver%7Candroid-ndk%7Cuser-agent%7Candroid-webview%7Cwebview%7Cbackground%7Cmultithreading%7Candroid-source%7Csms%7Cadb%7Csollections%7Cactivity|Chart")]
        [Parallelizable(ParallelScope.All)]
        public void ContainsTopLevelDomain_WhenGivenTopLevelDomain_ShouldReturnTrue(string expression)
        {
            bool condition = expression.ContainsTopLevelDomain();
            Assert.True(condition);
        }

        [Test]
        [TestCase("website")]
        [TestCase("example.c")]
        [TestCase("example.abcdefghijk")]
        [TestCase("Hello world")]
        [Parallelizable(ParallelScope.All)]
        public void ContainsTopLevelDomain_WhenNotGivenTopLevelDomain_ShouldReturnFalse(string expression)
        {
            bool condition = expression.ContainsTopLevelDomain();
            Assert.False(condition);
        }

        [Test]
        [TestCase("www.co")]
        [TestCase("google.co")]
        [TestCase("google.com")]
        [TestCase("google.com/")]
        [TestCase("google.com/h")]
        [TestCase("www.google.com/h")]
        [TestCase("google.co.uk")]
        [TestCase("company.com/showOrder.php?order=4621047")]
        [TestCase("company.com/companyInfo?name=C&H Sugar")]
        [TestCase("example.com/index.html")]
        [TestCase("host.company.com/companyInfo?name=C&H Sugar")]
        [TestCase("host.company.com/showCompanyInfo?name=C%26H%20Sugar")]
        [TestCase("www.company.com/showOrder.php?order=4621047")]
        [TestCase("www.company.com/companyInfo?name=C&H Sugar")]
        [TestCase("www.example.com/index.html")]
        [TestCase("this .ne")]
        [Parallelizable(ParallelScope.All)]
        public void ContainsTopLevelDomain_WhenGivenTopLevelDomainButInvalidUri_ShouldReturnFalse(string expression)
        {
            bool condition = expression.ContainsTopLevelDomain();
            Assert.False(condition);
        }

        [Test]
        [TestCase("https://")]
        [TestCase("http://")]
        [TestCase("HTTPS://")]
        [TestCase("HTTP://")]
        [Parallelizable(ParallelScope.All)]
        public void StartsWithScheme_WhenGivenScheme_ShouldReturnTrue(string expression)
        {
            bool condition = expression.StartsWithScheme();
            Assert.True(condition);
        }

        [Test]
        [TestCase("ftp://")]
        [TestCase("https")]
        [TestCase("https:/")]
        [TestCase("hellohttps://")]
        [Parallelizable(ParallelScope.All)]
        public void StartsWithScheme_WhenNotGivenScheme_ShouldReturnFalse(string expression)
        {
            bool condition = expression.StartsWithScheme();
            Assert.False(condition);
        }

        [Test]
        [TestCase("google.com/")]
        [TestCase("0")]
        [Parallelizable(ParallelScope.All)]
        public void PrependScheme_ShouldPrependScheme(string expression)
        {
            string result = expression.PrependScheme();
            bool condition = result.StartsWith("https://");
            Assert.True(condition);
        }

        [Test]
        public void PrependScheme_ShouldNotPrependScheme()
        {
            int expected = 1;
            string expression = "https://google.com/";
            string result = expression.PrependScheme();
            Regex rx = new(@"https://");
            MatchCollection matches = rx.Matches(result);
            int actual = matches.Count;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase("Hello there, how are there you there?", "there", "NEW", 1, "Hello NEW, how are there you there?")]
        [TestCase("Hello there, how are there you there?", "there", "NEW", 2, "Hello NEW, how are NEW you there?")]
        [TestCase("Hello there, how are there you there?", "there", "NEW", 3, "Hello NEW, how are NEW you NEW?")]
        [TestCase("Falling through wet forests", "forests", "", 1, "Falling through wet ")]
        [Parallelizable(ParallelScope.All)]
        public void Replace_WhenGiveCount_ShouldReplaceCountOccurrencesOfValue(string expression, string oldValue, string newValue, int count, string expected)
        {
            string actual = expression.Replace(oldValue, newValue, count);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase("Hello there, how are there you there?", "there", "NEW", 4, "Hello NEW, how are NEW you NEW?")]
        [TestCase("Hello", "z", "a", 1, "Hello")]
        [TestCase("PigsOnTheW", "Wing", "wig", 1, "PigsOnTheW")]
        [Parallelizable(ParallelScope.All)]
        public void Replace_WhenGivenCountGreaterThanOccurrences_ShouldNotThrow(string expression, string oldValue, string newValue, int count, string expected)
        {
            string actual = expression.Replace(oldValue, newValue, count);
            Assert.AreEqual(expected, actual);
        }
    }
}
