using System;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace SDL.TridionVSRazorExtension.Common.Xml
{
    public static class Service
    {
        public static XElement GetByXPath(this XElement root, string xPath, XNamespace ns)
        {
            if (root == null || String.IsNullOrEmpty(xPath))
                return null;

            xPath = xPath.Trim('/');
            if (String.IsNullOrEmpty(xPath))
                return null;

            if (xPath.Contains("/"))
            {
                xPath = "/xhtml:" + xPath.Replace("/", "/xhtml:");
                XmlNamespaceManager namespaceManager = new XmlNamespaceManager(new NameTable());
                namespaceManager.AddNamespace("xhtml", ns.ToString());
                return root.XPathSelectElement(xPath, namespaceManager);
            }

            return root.Element(ns + xPath);
        }
    }
}
