using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;

namespace WcfDoc.Engine
{
    public class Configuration
    {
        // ────────────────────────── Constructor ──────────────────────────

        public Configuration(XDocument configuration)
        {
            try
            {
                XNode servicesNode = configuration.XPathSelectElement("configuration/system.serviceModel");
                Source = XDocument.Load(servicesNode.CreateReader());
            }
            catch (Exception exception)
            {
                throw new Exception(string.Format("Unable to open service model configuration section in file: {0}", exception.Message));
            }
        }

        // ────────────────────────── Public Members ──────────────────────────

        public XDocument Source { get; private set; }

        public IEnumerable<XElement> GetEndpoints(string service)
        {
            return Source.XPathSelectElements(string.Format("/system.serviceModel/services/service[@name='{0}']/endpoint", service));
        }
    }
}
