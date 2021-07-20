using Reginald.Core.IO;
using Xunit;

namespace Reginald.Tests
{
    public class FileOperationsTests
    {
        [Fact]
        public void GetDefaultKeywordsXml_ShouldReturnString()
        {
            bool expected = true;

            var returnValue = FileOperations.GetDefaultKeywordsXml();
            bool actual = returnValue is string;

            Assert.Equal(expected, actual);
        }
    }
}
