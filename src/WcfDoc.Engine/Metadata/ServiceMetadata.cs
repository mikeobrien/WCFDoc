using System.Collections.Generic;

namespace WcfDoc.Engine.Metadata
{
    public class ServiceMetadata
    {
        public string TypeId { get; set; }
        public string TypeName { get; set; }
        public string TypeNamespace { get; set; }
        public string TypeFullname { get; set; }
        public IEnumerable<OperationMetadata> Operations { get; set; }
    }
}
