using Reginald.Core.IO;
using System.IO;
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

        [Fact]
        public void MakeXmlFile_ShouldMakeXmlFile()
        {
            string xml = "<?xml version=\"1.0\"?> \n" +
                "<Searches> \n" +
                "    <Namespace Name=\"dummy\" ID=\"0\">" +
                "        <Name>Math</Name> \n" +
                "        <Keyword></Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/dummy.png</Icon> \n" +
                "        <URL></URL> \n" +
                "        <Separator></Separator> \n" +
                "        <Format>{0}</Format> \n" +
                "        <Alt>Copy to clipboard</Alt> \n" +
                "    </Namespace>" +
                "</Searches>";
            string filename = "__Dummy.xml";
            FileOperations.MakeXmlFile(xml, filename);

            string path = Path.Combine(ApplicationPaths.AppDataDirectoryPath, ApplicationPaths.ApplicationName, filename);
            bool fileExists = File.Exists(path);

            Assert.True(fileExists);
            if (fileExists)
            {
                File.Delete(path);
            }
        }
    }
}
