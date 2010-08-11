using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace WcfDoc.Engine.Extensions
{
    public static class XAttributeExtensions
    {
        public static string GetValueOrEmpty(this XAttribute attribute)
        {
            if (attribute != null)
                return attribute.Value ?? string.Empty;
            else return string.Empty;
        }
    }
}
