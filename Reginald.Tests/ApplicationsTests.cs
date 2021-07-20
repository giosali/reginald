using Reginald.Core.Base;
using System.Collections.Generic;
using Xunit;

namespace Reginald.Tests
{
    public class ApplicationsTests
    {
        [Fact]
        public static void MakeDictionary_ShouldReturnDictionary()
        {
            var dict = Applications.MakeDictionary();
            Assert.True(dict is Dictionary<string, string>);
        }
    }
}
