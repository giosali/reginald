using Reginald.Core.Base;
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
        public static XmlDocument GetXmlDocument(string documentName)
        {
            string path = Path.Combine(IO.ApplicationPaths.AppDataDirectoryPath, IO.ApplicationPaths.ApplicationName, documentName);
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

        public static XmlNode GetLastNode(XmlDocument doc)
        {
            return doc.SelectSingleNode(Constants.LastNodeXpath);
        }

        public static XmlNode GetCurrentNodeFromID(XmlDocument doc, int id)
        {
            string xpath = String.Format(Constants.NamespaceIDXpathFormat, id);
            return doc.SelectSingleNode(xpath);
        }
    }
}
