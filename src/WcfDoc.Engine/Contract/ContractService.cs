using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Xml.Linq;
using WcfDoc.Engine.Metadata;

namespace WcfDoc.Engine.Contract
{
    public class ContractService
    {
        public ServiceMetadata Metadata { get; set; }
        public Type Type { get; set; }
        public ServiceContractAttribute Properties { get; set; }
        public bool Restful { get; set; }
        public IEnumerable<XElement> Comments { get; set; }
        public IEnumerable<ContractOperation> Operations { get; set; }
    }
}
