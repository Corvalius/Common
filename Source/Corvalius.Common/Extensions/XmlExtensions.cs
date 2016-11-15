using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Corvalius.Extensions
{
    public static class XmlExtensions
    {
        public static XElement ToXElement(this XmlNode node)
        {
            var xDoc = new XDocument();
            using (XmlWriter xmlWriter = xDoc.CreateWriter())
            {
                node.WriteTo(xmlWriter);
            }
            return xDoc.Root;
        }

        public static XmlNode ToXmlNode(this XElement element)
        {
            using (XmlReader xmlReader = element.CreateReader())
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlReader);

                return xmlDoc;
            }
        }
    }
}