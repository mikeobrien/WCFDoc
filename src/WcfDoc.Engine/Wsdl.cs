using System;
using System.Xml.Linq;
using System.IO;
using System.Xml;
using System.ServiceModel.Description;

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
            var exporter = new WsdlExporter();
            exporter.ExportContract(ContractDescription.GetContract(contract));
            var metadataSet = exporter.GetGeneratedMetadata();
            var schemaStream = new MemoryStream();
            metadataSet.WriteTo(new XmlTextWriter(new StreamWriter(schemaStream)));
            schemaStream.Position = 0;
            var source = XDocument.Load(
                new XmlTextReader(schemaStream));
            return new Wsdl(source);
        }
    }
}
