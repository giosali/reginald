using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Reginald.Extensions
{
    public static class XmlDocumentExtensions
    {
        public static XmlNode GetNode(this XmlDocument doc, string attribute)
        {
            string xpath = $"//Searches/Namespace[@Name='{attribute}']";
            XmlNode foundNode = doc.SelectSingleNode(xpath);
            return foundNode;
        }

        public static XmlNodeList GetNodes(this XmlDocument doc, string attribute)
        {
            string xpath = $"//Searches/Namespace[@Name='{attribute}']";
            XmlNodeList foundNodes = doc.SelectNodes(xpath);
            if (foundNodes.Count == 0)
                foundNodes = null;
            return foundNodes;
        }

        public static List<string> GetNodesAttributes(this XmlDocument doc)
        {
            string xpath = $"//Searches//Namespace";
            XmlNodeList foundNodes = doc.SelectNodes(xpath);
            List<string> attributes = new();
            foreach (XmlNode node in foundNodes)
            {
                attributes.Add(node.Attributes["Name"].Value);
            }
            return attributes;
        }
    }
}