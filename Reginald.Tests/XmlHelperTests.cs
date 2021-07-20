using Reginald.Core.Base;
using Reginald.Core.Helpers;
using Reginald.Core.IO;
using System;
using System.IO;
using System.Xml;
using Xunit;

namespace Reginald.Tests
{
    public class XmlHelperTests
    {
        [Theory]
        [InlineData(ApplicationPaths.XmlKeywordFilename)]
        [InlineData(ApplicationPaths.XmlUserKeywordFilename)]
        public static void GetXmlDocument_ShouldReturnXmlDocument(string filename)
        {
            bool expected = true;

            var doc = XmlHelper.GetXmlDocument(filename);
            bool actual = doc is XmlDocument;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static void GetXmlDocument_ShouldThrowFileNotFoundException()
        {
            Assert.Throws<FileNotFoundException>(() =>
            {
                _ = XmlHelper.GetXmlDocument("Foo.xml");
            });
        }

        [Fact]
        public static void MakeXmlNode_ShouldReturnXmlNode()
        {
            bool expected = true;

            XmlNode node = XmlHelper.MakeXmlNode(String.Empty, 0, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty);
            bool actual = node is XmlNode;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(ApplicationPaths.XmlKeywordFilename)]
        [InlineData(ApplicationPaths.XmlUserKeywordFilename)]
        public static void GetLastNode_ShouldReturnXmlNode(string filename)
        {
            bool expected = true;

            XmlDocument doc = new();
            doc.Load(Path.Combine(ApplicationPaths.AppDataDirectoryPath, ApplicationPaths.ApplicationName, filename));
            var node = XmlHelper.GetLastNode(doc);
            bool actual = node is XmlNode;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(ApplicationPaths.XmlKeywordFilename, Constants.LastNodeXpath)]
        [InlineData(ApplicationPaths.XmlUserKeywordFilename, Constants.LastNodeXpath)]
        public static void GetLastNode_ShouldReturnLastXmlNode(string filename, string xpath)
        {
            XmlDocument doc = new();
            doc.Load(Path.Combine(ApplicationPaths.AppDataDirectoryPath, ApplicationPaths.ApplicationName, filename));
            XmlNode expected = doc.SelectSingleNode(xpath);

            XmlNode actual = XmlHelper.GetLastNode(doc);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(ApplicationPaths.XmlKeywordFilename, 2)]
        public static void GetCurrentNodeFromID_ShouldReturnXmlNode(string filename, int id)
        {
            bool expected = true;

            XmlDocument doc = new();
            doc.Load(Path.Combine(ApplicationPaths.AppDataDirectoryPath, ApplicationPaths.ApplicationName, filename));
            var node = XmlHelper.GetCurrentNodeFromID(doc, id);
            bool actual = node is XmlNode;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static void GetCurrentNodeFromID_NodeIDShouldMatch()
        {
            int expected = 1;

            XmlDocument doc = new();
            doc.Load(Path.Combine(ApplicationPaths.AppDataDirectoryPath, ApplicationPaths.ApplicationName, ApplicationPaths.XmlKeywordFilename));
            XmlNode node = XmlHelper.GetCurrentNodeFromID(doc, expected);
            int actual = int.Parse(node.Attributes["ID"].Value);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public static void GetCurrentNodeFromID_ShouldReturnNullIfIDInvalid(int id)
        {
            XmlDocument doc = new();
            doc.Load(Path.Combine(ApplicationPaths.AppDataDirectoryPath, ApplicationPaths.ApplicationName, ApplicationPaths.XmlKeywordFilename));
            XmlNode node = XmlHelper.GetCurrentNodeFromID(doc, id);

            Assert.Null(node);
        }
    }
}
