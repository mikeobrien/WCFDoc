using System.Collections.Generic;

namespace WcfDoc.Engine.Metadata
{
    public class TypeMetadata
    {
        public enum TypeClass
        {
            Complex,
            Collection,
            Enumeration,
            Primitive
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Namespace { get; set; }
        public string Fullname { get; set; }
        public TypeClass TypeClassification { get; set; }
        public string RelatedName { get; set; }
        public string RelatedTypeId { get; set; }
        public string RelatedTypeName { get; set; }
        public string RelatedTypeNamespace { get; set; }
        public string RelatedTypeFullname { get; set; }
        public IEnumerable<MemberMetadata> Members { get; set; }
        public IEnumerable<OptionMetadata> Options { get; set; }
    }
}
