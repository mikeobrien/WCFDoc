using System;
using System.Collections.Generic;
using System.Xml.Linq;
using WcfDoc.Engine.Metadata;

namespace WcfDoc.Engine.Contract
{
    public class ContractType
    {
        public TypeMetadata Metadata { get; set; }
        public Type Type { get; set; }
        public IEnumerable<XElement> Comments { get; set; }
        public IEnumerable<ContractMember> Members { get; set; }
        public IEnumerable<ContractOption> Options { get; set; }
    }
}
