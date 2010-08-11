using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace WcfDoc.Engine.Extensions
{
    public static class XElementExtensions
    {
        public static IEnumerable<XElement> GetChildren(this XElement element)
        {
            if (element != null)
                return element.Elements();
            else
                return null;
        }

        public static string GetValueOrEmpty(this XElement element)
        {
            if (element != null)
                return element.Value ?? string.Empty;
            else return string.Empty;
        }

        public static string GetAttributeOrEmpty(this XElement element, string attribute)
        {
            if (element != null)
                return element.Attribute(attribute).GetValueOrEmpty();
            else return string.Empty;
        }
    }
}
