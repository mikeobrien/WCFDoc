using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml.Linq;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.ServiceModel.Configuration;
using System.IO;
using System.Xml.XPath;
using System.ServiceModel.Web;
using System.Xml;
using System.ServiceModel.Description;
using System.Xml.Schema;
using System.Collections;
using System.Xml.Xsl;

namespace WcfDoc.Engine
{
    public class Wsdl
    {
        // ────────────────────────── Constructor ──────────────────────────

        private Wsdl(XDocument source)
        {
            Source = source;
        }

        // ────────────────────────── Public Members ──────────────────────────

        public XDocument Source { get; private set; }

        public static Wsdl Generate(Type contract)
        {
            WsdlExporter exporter = new WsdlExporter();
            exporter.ExportContract(ContractDescription.GetContract(contract));
            MetadataSet metadataSet = exporter.GetGeneratedMetadata();
            MemoryStream schemaStream = new MemoryStream();
            metadataSet.WriteTo(new XmlTextWriter(new StreamWriter(schemaStream)));
            schemaStream.Position = 0;
            XDocument source = XDocument.Load(
                new XmlTextReader(schemaStream));
            return new Wsdl(source);
        }
    }
}
