using System;
using System.Reflection;
using System.Xml.Linq;
using WcfDoc.Engine.Metadata;

namespace WcfDoc.Engine.Contract
{
    public class ContractParameter
    {
        public ParameterMetadata Metadata { get; set; }
        public Type Type { get; set; }
        public ParameterInfo ParameterInfo { get; set; }
        public string RestfulType { get; set; }
        public XElement Comments { get; set; }
    }
}
