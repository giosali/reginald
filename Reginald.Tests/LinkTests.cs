namespace Reginald.Tests
{
    using Reginald.Data.Representations;
    using Xunit;

    public static class LinkTests
    {
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
        [InlineData("http://www.company.com:81/a/b/c.html?user=Alice&year=2008#p2")]
        [InlineData(" https://google.com")]
        public static async void IsLink_WhenGivenValidLinks_ShouldReturnTrue(string expression)
        {
            Link link = new();
            Assert.True(await link.IsLink(expression));
        }

        [Theory]
        [InlineData("website")]
        [InlineData("example.c")]
        [InlineData("example.abcdefghijk")]
        [InlineData("Hello world")]
        [InlineData("g$$gle.com")]
        [InlineData("ht#tps://google.com")]
        [InlineData("ht##ps://google.com")]
        [InlineData("http$://google.com")]
        [InlineData("@https://google.com")]
        [InlineData("!https://google.com")]
        [InlineData("$https://google.com")]
        [InlineData("%https://google.com")]
        [InlineData("^https://google.com")]
        [InlineData("company.com:81/a/b/c.html?user=Alice&year=2008#p2")]
        [InlineData("https://go ogle.com/")]
        [InlineData("this .ne")]
        [InlineData("hello world.com/")]
        public static async void IsLink_WhenGivenInvalidLinks_ShouldReturnFalse(string expression)
        {
            Link link = new();
            Assert.False(await link.IsLink(expression));
        }
    }
}
