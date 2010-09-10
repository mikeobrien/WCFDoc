using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using WcfDoc.Engine.Metadata;

namespace WcfDoc.Engine.Contract
{
    public class ContractMember
    {
        public MemberMetadata Metadata { get; set; }
        public Type Type { get; set; }
        public MemberInfo MemberInfo { get; set; }
        public IEnumerable<XElement> Comments { get; set; }
    }
}
