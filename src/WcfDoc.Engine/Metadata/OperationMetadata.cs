using System.Collections.Generic;

namespace WcfDoc.Engine.Metadata
{
    public class OperationMetadata
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Fullname { get; set; }
        public IEnumerable<ParameterMetadata> Parameters { get; set; }
        public ReturnTypeMetadata ReturnType { get; set; }
    }
}
