namespace Reginald.Tests.Reginald
{
    using global::Reginald.Models.DataModels;
    using NUnit.Framework;

    [TestFixture]
    public class UrlTests
    {
        private Url _url;

        [Test]
        [TestCase("www.co")]
        [TestCase("google.co")]
        [TestCase("google.com")]
        [TestCase("google.com/")]
        [TestCase("google.com/h")]
        [TestCase("company.com/showOrder.php?order=4621047")]
        [TestCase("company.com/companyInfo?name=C&H Sugar")]
        [TestCase("host.company.com/companyInfo?name=C&H Sugar")]
        [TestCase("host.company.com/showCompanyInfo?name=C%26H%20Sugar")]
        [TestCase("example.com/index.html")]
        [TestCase("www.google.com/h")]
        [TestCase("www.company.com/showOrder.php?order=4621047")]
        [TestCase("www.company.com/companyInfo?name=C&H Sugar")]
        [TestCase("www.example.com/index.html")]
        [TestCase("https://google.com")]
        [TestCase("https://google.com/")]
        [TestCase("https://www.google.com")]
        [TestCase("https://www.google.com/")]
        [TestCase("https://domains.google.com")]
        [TestCase("https://domains.google.com/")]
        [TestCase("https://www.domains.google.com")]
        [TestCase("https://www.domains.google.com/")]
        [TestCase("google.co.uk")]
        [TestCase("https://google.co.uk")]
        [TestCase("https://google.co.uk/")]
        [TestCase("https://www.google.co.uk/")]
        [TestCase("http://www.company.com/showOrder.php?order=4621047")]
        [TestCase("http://company.com/companyInfo?name=C&H Sugar")]
        [TestCase("http://host.company.com/companyInfo?name=C&H Sugar")]
        [TestCase("http://host.company.com/showCompanyInfo?name=C%26H%20Sugar")]
        [TestCase("http://www.example.com/index.html")]
        [TestCase("http://www.company.com:81/a/b/c.html?user=Alice&year=2008#p2")]
        [TestCase(" https://google.com")]
        [Parallelizable(ParallelScope.All)]
        public void Check_WhenGivenValidUrls_ReturnTrue(string input)
        {
            Assert.True(_url.Check(input));
        }

        [Test]
        [TestCase("website")]
        [TestCase("example.c")]
        [TestCase("example.abcdefghijk")]
        [TestCase("Hello world")]
        [TestCase("g$$gle.com")]
        [TestCase("ht#tps://google.com")]
        [TestCase("ht##ps://google.com")]
        [TestCase("http$://google.com")]
        [TestCase("@https://google.com")]
        [TestCase("!https://google.com")]
        [TestCase("$https://google.com")]
        [TestCase("%https://google.com")]
        [TestCase("^https://google.com")]
        [TestCase("company.com:81/a/b/c.html?user=Alice&year=2008#p2")]
        [TestCase("https://go ogle.com/")]
        [TestCase("this .ne")]
        [TestCase("hello world.com/")]
        [Parallelizable(ParallelScope.All)]
        public void Check_WhenGivenInvalidUrls_ReturnFalse(string input)
        {
            Assert.False(_url.Check(input));
        }

        [SetUp]
        public void Init()
        {
            _url = new()
            {
                IsEnabled = true,
            };
            var current = System.Windows.Application.Current;
        }
    }
}
