using System.Collections.Generic;
using System.Xml;

namespace Reginald.Extensions
{
    public static class XmlDocumentExtensions
    {
        /// <summary>
        /// Returns a XmlNode according to an xpath string.
        /// </summary>
        /// <param name="doc">The XmlDocument.</param>
        /// <param name="xpath">The xpath.</param>
        /// <returns>A nullable XmlNode.</returns>
        public static XmlNode GetNode(this XmlDocument doc, string xpath)
        {
            // Original implementation:
            //string xpath = $"//Searches/Namespace[@Name='{attribute}']";
            XmlNode foundNode = doc.SelectSingleNode(xpath);
            return foundNode;
        }

        /// <summary>
        /// Returns a XmlNodeList according to an xpath string.
        /// </summary>
        /// <param name="doc">The XmlDocument.</param>
        /// <param name="xpath">The xpath.</param>
        /// <returns>A XmlNodeList of nodes matching the xpath string.</returns>
        public static XmlNodeList GetNodes(this XmlDocument doc, string xpath)
        {
            // Original implementation:
            //string xpath = $"//Searches/Namespace[@Name='{attribute}']";
            XmlNodeList foundNodes = doc.SelectNodes(xpath);
            if (foundNodes.Count == 0)
                foundNodes = null;
            return foundNodes;
        }

        /// <summary>
        /// Returns the values from nodes containing the attribute "Name" and their value in a list of strings.
        /// </summary>
        /// <param name="doc">The XmlDocument.</param>
        /// <param name="xpath">The xpath.</param>
        /// <returns>A list of strings of values from attributes in nodes.</returns>
        public static List<string> GetNodesAttributes(this XmlDocument doc, string xpath)
        {
            // Original implementation:
            //string xpath = $"//Searches//Namespace";
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