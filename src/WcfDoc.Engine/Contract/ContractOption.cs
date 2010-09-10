using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using WcfDoc.Engine.Metadata;

namespace WcfDoc.Engine.Contract
{
    public class ContractOption
    {
        public OptionMetadata Metadata { get; set; }
        public FieldInfo FieldInfo { get; set; }
        public IEnumerable<XElement> Comments { get; set; }
    }
}
