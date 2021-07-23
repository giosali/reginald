using Reginald.Core.Base;
using Reginald.Extensions;
using System.Collections.Generic;
using System.Xml;
using Xunit;

namespace Reginald.Tests
{
    public class XmlDocumentExtensionsTests
    {
        [Theory]
        [InlineData("dummy")]
        public static void GetNode_ShouldReturnXmlNode(string xpathPortion)
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
            XmlDocument doc = new();
            doc.LoadXml(xml);
            var node = doc.GetNode(string.Format(Constants.NamespaceNameXpathFormat, xpathPortion));

            Assert.True(node is XmlNode);
        }

        [Fact]
        public static void GetNode_ShouldReturnNullIfNodeNotFound()
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
            XmlDocument doc = new();
            doc.LoadXml(xml);
            XmlNode node = doc.GetNode(string.Format(Constants.NamespaceNameXpathFormat, "dumy"));

            Assert.True(node is null);
        }

        [Fact]
        public static void GetNode_ShouldReturnNodeIfNodeFound()
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
            XmlDocument doc = new();
            doc.LoadXml(xml);
            XmlNode node = doc.GetNode(string.Format(Constants.NamespaceNameXpathFormat, "dummy"));

            Assert.True(node is not null);
        }

        [Theory]
        [InlineData("dummy")]
        public static void GetNodes_ShouldReturnXmlNodeList(string xpathPortion)
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
            XmlDocument doc = new();
            doc.LoadXml(xml);
            var nodes = doc.GetNodes(string.Format(Constants.NamespaceNameXpathFormat, xpathPortion));

            Assert.True(nodes is XmlNodeList);
        }

        [Fact]
        public static void GetNodes_ShouldReturnNullIfNoneFound()
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
            XmlDocument doc = new();
            doc.LoadXml(xml);
            XmlNodeList nodes = doc.GetNodes(string.Format(Constants.NamespaceNameXpathFormat, "dumy"));

            Assert.True(nodes is null);
        }

        [Fact]
        public static void GetNodes_ShouldReturnThreeNodes()
        {
            int expected = 3;

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
                "    <Namespace Name=\"dummy\" ID=\"0\">" +
                "        <Name>Math</Name> \n" +
                "        <Keyword></Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/dummy.png</Icon> \n" +
                "        <URL></URL> \n" +
                "        <Separator></Separator> \n" +
                "        <Format>{0}</Format> \n" +
                "        <Alt>Copy to clipboard</Alt> \n" +
                "    </Namespace>" +
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
            XmlDocument doc = new();
            doc.LoadXml(xml);
            XmlNodeList nodes = doc.GetNodes(string.Format(Constants.NamespaceNameXpathFormat, "dummy"));
            int actual = nodes.Count;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static void GetNodesAttributes_ShouldReturnListOfString()
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
            XmlDocument doc = new();
            doc.LoadXml(xml);
            var attributes = doc.GetNodesAttributes(Constants.NamespacesXpath);

            Assert.True(attributes is List<string>);
        }

        [Fact]
        public static void GetNodesAttributes_ShouldReturnSameAttributeValue()
        {
            string name = "dummy";

            string xml = "<?xml version=\"1.0\"?> \n" +
                "<Searches> \n" +
                $"    <Namespace Name=\"{name}\" ID=\"0\">" +
                "        <Name>Math</Name> \n" +
                "        <Keyword></Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/dummy.png</Icon> \n" +
                "        <URL></URL> \n" +
                "        <Separator></Separator> \n" +
                "        <Format>{0}</Format> \n" +
                "        <Alt>Copy to clipboard</Alt> \n" +
                "    </Namespace>" +
                "</Searches>";
            XmlDocument doc = new();
            doc.LoadXml(xml);
            List<string> nodes = doc.GetNodesAttributes(Constants.NamespacesXpath);

            Assert.Equal(name, nodes[0]);
        }
    }
}
