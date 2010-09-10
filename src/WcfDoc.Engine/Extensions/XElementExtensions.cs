using System.Collections.Generic;
using System.Xml.Linq;

namespace WcfDoc.Engine.Extensions
{
    public static class XElementExtensions
    {
        public static IEnumerable<XElement> GetChildren(this XElement element)
        {
            return element != null ? element.Elements() : null;
        }

        public static string GetValueOrEmpty(this XElement element)
        {
            return element != null ? element.Value : string.Empty;
        }

        public static string GetAttributeOrEmpty(this XElement element, string attribute)
        {
            return element != null ? element.Attribute(attribute).GetValueOrEmpty() : string.Empty;
        }
    }
}
