using System;
using System.Xml.Linq;
using WcfDoc.Engine.Metadata;

namespace WcfDoc.Engine.Contract
{
    public class ContractReturnType
    {
        public ReturnTypeMetadata Metadata { get; set; }
        public Type Type { get; set; }
        public XElement Comments { get; set; }
    }
}
