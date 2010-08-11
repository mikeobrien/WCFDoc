using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using WcfDoc.Engine.Extensions;

namespace WcfDoc.Engine
{
    public class ContractMetadata
    {
        // ────────────────────────── Private Fields ──────────────────────────

        private XDocument _document;
        private IEnumerable<TypeMetadata> _types;
        private IEnumerable<ServiceMetadata> _services;

        // ────────────────────────── Constructors ──────────────────────────

        private ContractMetadata(XDocument document)
        {
            _document = document;
        }

        // ────────────────────────── Public Members ──────────────────────────

        public IEnumerable<TypeMetadata> Types
        {
            get
            {
                if (_types == null)
                    _types = (from type in _document.Element("metadata").Element("types").Elements("type")
                       select new TypeMetadata
                       {
                           Id = type.Attribute("namespace").Value.Combine("/", type.Attribute("name").Value).Hash(),
                           Name = type.Attribute("name").Value,
                           Namespace = type.Attribute("namespace").Value,
                           Fullname = type.Attribute("namespace").Value.Combine("/", type.Attribute("name").Value),
                           TypeClassification = (TypeMetadata.TypeClass)Enum.Parse(
                               typeof(TypeMetadata.TypeClass), type.Attribute("class").Value),
                           RelatedName = type.Attribute("relatedName").GetValueOrEmpty(),
                           RelatedTypeId = type.Attribute("relatedTypeNamespace").GetValueOrEmpty().Combine("/", type.Attribute("relatedType").GetValueOrEmpty()).Hash(),
                           RelatedTypeName = type.Attribute("relatedType").GetValueOrEmpty(),
                           RelatedTypeNamespace = type.Attribute("relatedTypeNamespace").GetValueOrEmpty(),
                           RelatedTypeFullname = type.Attribute("relatedTypeNamespace").GetValueOrEmpty().Combine("/", type.Attribute("relatedType").GetValueOrEmpty()),
                           Members = (from member in type.Element("members").Elements("member")
                                 select new MemberMetadata
                                 {
                                     Id = type.Attribute("namespace").Value.Combine("/", type.Attribute("name").Value).Combine("/", member.Attribute("name").Value).Hash(),
                                     Name = member.Attribute("name").Value,
                                     TypeId = member.Attribute("typeNamespace").Value.Combine("/", member.Attribute("type").Value).Hash(),
                                     TypeName = member.Attribute("type").Value,
                                     TypeNamespace = member.Attribute("typeNamespace").Value,
                                     TypeFullname = member.Attribute("typeNamespace").Value.Combine("/", member.Attribute("type").Value),
                                     Required = member.Attribute("required").Value.Convert(true)
                                 }).ToList(),
                           Options = (from option in type.Element("options").Elements("option")
                                 select new OptionMetadata
                                 {
                                     Value = option.Attribute("value").Value
                                 }).ToList()
                       }).ToList();
                return _types;
            }
        }

        public IEnumerable<ServiceMetadata> Services
        {
            get
            {
                if (_services == null)
                    _services = (from service in _document.Element("metadata").Element("services").Elements("service")
                       select new ServiceMetadata
                       {
                           TypeId = service.Attribute("namespace").Value.Combine("/", service.Attribute("name").Value).Hash(),
                           TypeName = service.Attribute("name").Value,
                           TypeNamespace = service.Attribute("namespace").Value,
                           TypeFullname = service.Attribute("namespace").Value.Combine("/", service.Attribute("name").Value),
                           Operations = (from operation in service.Elements("operation")
                                select new OperationMetadata
                                {
                                    Id = service.Attribute("namespace").Value.Combine("/", service.Attribute("name").Value).Combine("/", operation.Attribute("name").Value).Hash(),
                                    Name = operation.Attribute("name").Value,
                                    Fullname = service.Attribute("namespace").Value.Combine("/", service.Attribute("name").Value).Combine("/", operation.Attribute("name").Value),
                                    Parameters = (from parameter in operation.Element("parameters").Elements("parameter")
                                         select new ParameterMetadata
                                         {
                                             Name = parameter.Attribute("name").Value,
                                             TypeId = parameter.Attribute("typeNamespace").Value.Combine("/", parameter.Attribute("type").Value).Hash(),
                                             TypeName = parameter.Attribute("type").Value,
                                             TypeNamespace = parameter.Attribute("typeNamespace").Value,
                                             TypeFullname = parameter.Attribute("typeNamespace").Value.Combine("/", parameter.Attribute("type").Value),
                                             Nullable = parameter.Attribute("nillable").Value.Convert(false)
                                         }).ToList(),
                                    ReturnType = new ReturnTypeMetadata
                                    {
                                        TypeId = operation.Element("return").GetAttributeOrEmpty("typeNamespace").
                                                        Combine("/", operation.Element("return").GetAttributeOrEmpty("type")).Hash(),
                                        TypeName = operation.Element("return").GetAttributeOrEmpty("type"),
                                        TypeNamespace = operation.Element("return").GetAttributeOrEmpty("typeNamespace"),
                                        Nullable = operation.Element("return").GetAttributeOrEmpty("nillable").Convert(false),
                                        TypeFullname = operation.Element("return").GetAttributeOrEmpty("typeNamespace").
                                                        Combine("/", operation.Element("return").GetAttributeOrEmpty("type"))
                                    }
                                }).ToList()
                       }).ToList();
                return _services;
            }
        }

        public static ContractMetadata Generate(Type contract)
        {
            Wsdl wsdl = Wsdl.Generate(contract);

            return new ContractMetadata(wsdl.Source.Transform(
                Assembly.GetExecutingAssembly().FindManifestResourceStream("ContractMetadata.xslt")));
        }

        // ────────────────────────── Nested Types ──────────────────────────

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

        public class MemberMetadata
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string TypeId { get; set; }
            public string TypeName { get; set; }
            public string TypeNamespace { get; set; }
            public string TypeFullname { get; set; }
            public bool Required { get; set; }
        }

        public class OptionMetadata
        {
            public string Value { get; set; }
        }

        public class ServiceMetadata
        {
            public string TypeId { get; set; }
            public string TypeName { get; set; }
            public string TypeNamespace { get; set; }
            public string TypeFullname { get; set; }
            public IEnumerable<OperationMetadata> Operations { get; set; }
        }

        public class OperationMetadata
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Fullname { get; set; }
            public IEnumerable<ParameterMetadata> Parameters { get; set; }
            public ReturnTypeMetadata ReturnType { get; set; }
        }

        public class ParameterMetadata
        {
            public string Name { get; set; }
            public string TypeId { get; set; }
            public string TypeName { get; set; }
            public string TypeNamespace { get; set; }
            public string TypeFullname { get; set; }
            public bool Nullable { get; set; }
        }

        public class ReturnTypeMetadata
        {
            public string TypeId { get; set; }
            public string TypeName { get; set; }
            public string TypeNamespace { get; set; }
            public string TypeFullname { get; set; }
            public bool Nullable { get; set; }
        }
    }
}
