using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using WcfDoc.Engine.Extensions;

namespace WcfDoc.Initialization
{
    internal class OptionAttribute : Attribute 
    {
        // ────────────────────────── Private Fields ──────────────────────────

        private static Dictionary<string, XDocument> _documents = 
            new Dictionary<string, XDocument>();

        // ────────────────────────── Constructors ──────────────────────────

        public OptionAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public OptionAttribute(string name, string xmlResourceName, string descriptionXPath)
        {
            Name = name;

            XDocument document;

            if (!_documents.ContainsKey(xmlResourceName))
            {
                Stream documentStream =
                    Assembly.GetExecutingAssembly().FindManifestResourceStream(xmlResourceName);
                document = XDocument.Load(new XmlTextReader(documentStream));
                _documents.Add(xmlResourceName, document);
            }
            else
                document = _documents[xmlResourceName];

            XElement descriptionElement = 
                document.XPathSelectElement(string.Format(descriptionXPath.Replace("{name}", "{0}"), name));
            Description = descriptionElement != null ? descriptionElement.Value.Trim() : string.Empty;
        }

        // ────────────────────────── Public Properties ──────────────────────────

        public string Name { get; private set; }
        public string Description { get; private set; }
    }
}
