using System.Xml.Linq;

namespace WcfDoc.Engine.Extensions
{
    public static class XAttributeExtensions
    {
        public static string GetValueOrEmpty(this XAttribute attribute)
        {
            return attribute != null ? attribute.Value : string.Empty;
        }
    }
}
