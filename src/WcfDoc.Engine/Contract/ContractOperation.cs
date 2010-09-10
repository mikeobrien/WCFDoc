using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Xml.Linq;
using WcfDoc.Engine.Metadata;

namespace WcfDoc.Engine.Contract
{
    public class ContractOperation
    {
        public OperationMetadata Metadata { get; set; }
        public OperationContractAttribute Properties { get; set; }
        public MethodInfo MethodInfo { get; set; }
        public WebInvokeAttribute RestfulProperties { get; set; }
        public IEnumerable<XElement> Comments { get; set; }
        public IEnumerable<ContractParameter> Parameters { get; set; }
        public ContractReturnType ReturnType { get; set; }
    }
}
