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
        public static XmlNode GetNode(this XmlDocument doc, string value)
        {
            string xpath = $"//Searches/Namespace[@Name='{value}']";
            XmlNode foundNode = doc.SelectSingleNode(xpath);
            return foundNode;
        }
    }
}