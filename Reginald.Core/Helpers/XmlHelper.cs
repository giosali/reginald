using Reginald.Core.Base;
using Reginald.Core.IO;
using System;
using System.IO;
using System.Xml;

namespace Reginald.Core.Helpers
{
    public static class XmlHelper
    {
        /// <summary>
        /// Returns a XML file from %AppData%.
        /// </summary>
        /// <param name="documentName">Name of the XML file to retrieve.</param>
        /// <returns>The corresponding XML file from %AppData%.</returns>
        public static XmlDocument GetXmlDocument(string documentName, bool isLocal = false)
        {
            string path = Path.Combine(isLocal ? AppDomain.CurrentDomain.BaseDirectory : Path.Combine(ApplicationPaths.AppDataDirectoryPath, ApplicationPaths.ApplicationName), documentName);
            //string path = isLocal ? documentName : Path.Combine(IO.ApplicationPaths.AppDataDirectoryPath, IO.ApplicationPaths.ApplicationName, documentName);
            XmlDocument doc = new();
            doc.Load(path);
            return doc;
        }

        /// <summary>
        /// Saves a XML document.
        /// </summary>
        /// <param name="doc">The XML document to save.</param>
        /// <param name="documentName">The name of the XML document in %AppData%.</param>
        public static void SaveXmlDocument(XmlDocument doc, string documentName)
        {
            string path = Path.Combine(IO.ApplicationPaths.AppDataDirectoryPath, IO.ApplicationPaths.ApplicationName, documentName);
            doc.Save(path);
        }

        /// <summary>
        /// Returns a XML node related to keywords created from a formatted string.
        /// </summary>
        /// <param name="keyword">Formats into an attribute and child node.</param>
        /// <param name="id">Formats into an attribute.</param>
        /// <param name="name">Formats into a child node.</param>
        /// <param name="icon">Formats into a child node. </param>
        /// <param name="url">Formats into a child node.</param>
        /// <param name="separator">Formats into a child node.</param>
        /// <param name="format">Formats into a child node.</param>
        /// <param name="defaultText">Formats into a child node.</param>
        /// <param name="alt">Formats into a child node.</param>
        /// <returns>A XML node.</returns>
        public static XmlNode MakeXmlNode(string keyword, int id, string name, string icon, string url,
                                           string separator, string format, string defaultText, string alt)
        {
            string xml = $"<Namespace Name=\"{keyword}\" ID=\"{id}\">" +
               $"    <Name>{name}</Name> \n" +
               $"    <Keyword>{keyword}</Keyword> \n" +
               $"    <Icon>{icon}</Icon> \n" +
               $"    <URL>{url}</URL> \n" +
               $"    <Separator>{separator}</Separator> \n" +
               $"    <Format>{format}</Format> \n" +
               $"    <DefaultText>{defaultText}</DefaultText> \n" +
               $"    <Alt>{alt}</Alt> \n" +
               $"    <IsEnabled>true</IsEnabled> \n" +
               "</Namespace>";
            XmlDocument doc = new();
            doc.LoadXml(xml);
            XmlNode node = doc.DocumentElement;
            return node;
        }

        /// <summary>
        /// Returns the final node in a XML document according to a constant xpath expression.
        /// </summary>
        /// <param name="doc">The XML document from which to take the last relevant node.</param>
        /// <returns>The last relevant XML node from a XML document.</returns>
        public static XmlNode GetLastNode(XmlDocument doc)
        {
            return doc.SelectSingleNode(Constants.LastNodeXpath);
        }

        /// <summary>
        /// Returns a XML node based on its ID attribute.
        /// </summary>
        /// <param name="doc">The XML document to parse.</param>
        /// <param name="id">The ID that should mathc a node's ID attribute.</param>
        /// <returns>The matching XML node or null.</returns>
        public static XmlNode GetCurrentNodeFromID(XmlDocument doc, int id)
        {
            string xpath = string.Format(Constants.NamespaceIDXpathFormat, id);
            return doc.SelectSingleNode(xpath);
        }
    }
}
