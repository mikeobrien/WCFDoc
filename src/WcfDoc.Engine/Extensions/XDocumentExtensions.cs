using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Xsl;
using System.Xml;
using System.Xml.Linq;

namespace WcfDoc.Engine.Extensions
{
    public static class XDocumentExtensions
    {
        public static XDocument Transform(this XDocument document, Stream stylesheet)
        {
            return Transform(document, new XmlTextReader(stylesheet));
        }
        public static XDocument Transform(this XDocument document, XDocument stylesheet)
        {
            return Transform(document, stylesheet.CreateReader());
        }

        public static XDocument Transform(this XDocument document, XmlReader stylesheet)
        {
            XslCompiledTransform xslTransformer = new XslCompiledTransform();
            xslTransformer.Load(stylesheet);

            MemoryStream documentStream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(new StreamWriter(documentStream));

            xslTransformer.Transform(document.CreateReader(), null, writer);

            documentStream.Position = 0;
            return XDocument.Load(new XmlTextReader(documentStream));
        }
    }
}
