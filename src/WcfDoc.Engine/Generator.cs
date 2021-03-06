﻿using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using WcfDoc.Engine.Extensions;

namespace WcfDoc.Engine
{
    public class Generator
    {
        // ────────────────────────── Private Fields ──────────────────────────

        private readonly Context _context;

        // ────────────────────────── Constructors ──────────────────────────

        public Generator(Context context)
        {
            _context = context;
        }

        // ────────────────────────── Public Methods ──────────────────────────

        public void Generate(ServiceType serviceType)
        {
            var document = new XDocument();
            var root = new XElement("doc");
            document.Add(root);

            var contracts = new Contracts(_context.Assemblies, new XmlComments(_context.XmlComments), serviceType);

            root.Add(GenerateServiceTypes(contracts));
            root.Add(GenerateServiceContracts(contracts));
            root.Add(GetServices(_context, contracts));
            root.Add(GetServiceModelConfiguration(_context));
            root.Add(GetMetadata(_context));

            if (_context.Stylesheet != null)
                File.WriteAllText(_context.OutputPath, document.Transform(_context.Stylesheet));
            else 
                document.Save(_context.OutputPath);
        }

        // ────────────────────────── Private Members ──────────────────────────

        private static XElement GenerateServiceContracts(Contracts contracts)
        {
            return new XElement("serviceContracts",
                from serviceContract in contracts.Services 
                select new XElement("serviceContract",
                    new XAttribute("id", serviceContract.Metadata.TypeId),
                    new XAttribute("type", serviceContract.Type.FullName),
                    new XAttribute("assembly", serviceContract.Type.Assembly.FullName),
                    new XAttribute("callbackContract", serviceContract.Properties.CallbackContract != null ? 
                                        serviceContract.Properties.CallbackContract.FullName : string.Empty),
                    new XAttribute("configurationName", serviceContract.Properties.ConfigurationName.ValueOrEmpty()),
                    new XAttribute("hasProtectionLevel", serviceContract.Properties.HasProtectionLevel),
                    new XAttribute("name", serviceContract.Metadata.TypeName),
                    new XAttribute("namespace", serviceContract.Metadata.TypeNamespace),
                    new XAttribute("protectionLevel", serviceContract.Properties.ProtectionLevel.ToString()),
                    new XAttribute("sessionMode", serviceContract.Properties.SessionMode.ToString()),
                    new XAttribute("restful", serviceContract.Restful),
                    new XElement("comments", serviceContract.Comments),
                    new XElement("operations",
                        from operation in serviceContract.Operations
                        select new XElement("operation",
                            new XAttribute("id", operation.Metadata.Id),
                            new XAttribute("methodName", operation.MethodInfo.Name),
                            new XAttribute("action", operation.Properties.Action.ValueOrEmpty()),
                            new XAttribute("asyncPattern", operation.Properties.AsyncPattern),
                            new XAttribute("hasProtectionLevel", operation.Properties.HasProtectionLevel),
                            new XAttribute("isInitiating", operation.Properties.IsInitiating),
                            new XAttribute("isOneWay", operation.Properties.IsInitiating),
                            new XAttribute("isTerminating", operation.Properties.IsTerminating),
                            new XAttribute("name", operation.Metadata.Name),
                            new XAttribute("protectionLevel", operation.Properties.ProtectionLevel.ToString()),
                            new XAttribute("replyAction", operation.Properties.ReplyAction.ValueOrEmpty()),
                            new XAttribute("restful", (operation.RestfulProperties != null)),
                            new XAttribute("restMethod", operation.RestfulProperties != null ?
                                    operation.RestfulProperties.Method : string.Empty),
                            new XAttribute("restResource", operation.RestfulProperties != null ?
                                    operation.RestfulProperties.UriTemplate.StripUrlParameters() : string.Empty),
                            new XAttribute("restUriTemplate", operation.RestfulProperties != null ? 
                                    operation.RestfulProperties.UriTemplate ?? string.Empty : string.Empty),
                            new XAttribute("restBodyStyle", operation.RestfulProperties != null ? 
                                    operation.RestfulProperties.BodyStyle.ToString() : string.Empty),
                            new XAttribute("restIsBodyStyleSetExplicitly", operation.RestfulProperties != null ? 
                                    operation.RestfulProperties.IsBodyStyleSetExplicitly.ToString() : string.Empty),
                            new XAttribute("restIsRequestFormatSetExplicitly", operation.RestfulProperties != null ? 
                                    operation.RestfulProperties.IsRequestFormatSetExplicitly.ToString() : string.Empty),
                            new XAttribute("restIsResponseFormatSetExplicitly", operation.RestfulProperties != null ? 
                                    operation.RestfulProperties.IsResponseFormatSetExplicitly.ToString() : string.Empty),
                            new XAttribute("restRequestFormat", operation.RestfulProperties != null ? 
                                    operation.RestfulProperties.RequestFormat.ToString() : string.Empty),
                            new XAttribute("restResponseFormat", operation.RestfulProperties != null ? 
                                    operation.RestfulProperties.ResponseFormat.ToString() : string.Empty),
                            new XElement("comments", operation.Comments),
                            new XElement("parameters",
                                from parameter in operation.Parameters
                                select new XElement("parameter",
                                    new XAttribute("name", parameter.ParameterInfo.Name),
                                    new XAttribute("type", parameter.ParameterInfo.ParameterType.FullName),
                                    new XAttribute("typeId", parameter.Metadata.TypeId),
                                    new XAttribute("direction", parameter.ParameterInfo.IsIn ? "In" : "Out"),
                                    new XAttribute("xmlType", parameter.Metadata.TypeName),
                                    new XAttribute("xmlTypeNamespace", parameter.Metadata.TypeNamespace),
                                    new XAttribute("restfulType", parameter.RestfulType.ValueOrEmpty()),
                                    new XElement("comments", parameter.Comments != null ? 
                                            parameter.Comments.DescendantNodes() : null)
                                )
                            ),
                            new XElement("return",
                                operation.ReturnType.Type != null ? new XAttribute("typeId", operation.ReturnType.Metadata.TypeId) : null,
                                operation.ReturnType.Type != null ? new XAttribute("type", operation.ReturnType.Type.FullName) : null,
                                operation.ReturnType.Type != null ? new XAttribute("xmlType", operation.ReturnType.Metadata.TypeName) : null,
                                operation.ReturnType.Type != null ? new XAttribute("xmlTypeNamespace", 
                                        operation.ReturnType.Metadata.TypeNamespace) : null,
                                operation.ReturnType.Type != null ? new XElement("comments", operation.ReturnType.Comments != null ? 
                                        operation.ReturnType.Comments.DescendantNodes() : null) : null
                            )
                        )
                    )
                )
            );
        }

        private static XElement GenerateServiceTypes(Contracts contracts)
        {
            return new XElement("types",
                from type in contracts.Types
                select new XElement("type",
                    new XAttribute("id", type.Metadata.Id),
                    new XAttribute("name", type.Metadata.Name),
                    new XAttribute("namespace", type.Metadata.Namespace),
                    new XAttribute("class", type.Metadata.TypeClassification),
                    new XAttribute("fullName", type.Type != null ? type.Type.FullName : string.Empty),
                    new XAttribute("assembly", type.Type != null ? type.Type.Assembly.FullName : string.Empty),
                    new XAttribute("relatedName", type.Metadata.RelatedName.ValueOrEmpty()),
                    new XAttribute("relatedTypeId", type.Metadata.RelatedTypeId.ValueOrEmpty()),
                    new XAttribute("relatedType", type.Metadata.RelatedTypeName.ValueOrEmpty()),
                    new XAttribute("relatedTypeNamespace", type.Metadata.RelatedTypeNamespace.ValueOrEmpty()),
                    type.Comments != null ? new XElement("comments", type.Comments) : null,
                new XElement("options",
                    from option in type.Options
                    select new XElement("option",
                           new XAttribute("value", option.Metadata.Value),
                           option.Comments != null ? new XElement("comments", option.Comments.DescendantNodes()) : null
                    )
                ),
                new XElement("members",
                    from member in type.Members
                    select new XElement("member", 
                        new XAttribute("name", member.Metadata.Name), 
                        new XAttribute("type", member.Metadata.TypeName),
                        new XAttribute("typeId", member.Metadata.TypeId),
                        new XAttribute("required", member.Metadata.Required),
                        new XAttribute("typeNamespace", member.Metadata.TypeNamespace),
                           member.Comments != null ? new XElement("comments", member.Comments.DescendantNodes()) : null)
                    )
                )
            );
        }

        private static XElement GetServices(Context context, Contracts contracts)
        {
            var servicesElement = new XElement("services");

            XmlComments xmlComments = null;
            if (context.XmlComments != null) 
                xmlComments = new XmlComments(context.XmlComments);

            var types = context.Assemblies.FindTypes(
                t => (t.IsClass && (from contract in contracts.Services select contract.Type).Contains(t)) || 
                     t.ImplementsInterface(from contract in contracts.Services select contract.Type));

            foreach (var type in types)
            {
                var serviceElement = new XElement("service",
                    new XAttribute("id", type.AssemblyQualifiedName.Hash()),
                    new XAttribute("type", type.FullName),
                    new XAttribute("assembly", type.Assembly.FullName));

                if (xmlComments != null)
                    serviceElement.Add(
                        new XElement("comments", 
                            xmlComments.GetTypeComments(type)));

                if (context.ServiceWebsite != null)
                    serviceElement.Add(new XElement("website",
                        from file in context.ServiceWebsite
                        where file.Type.Equals(type.FullName, StringComparison.OrdinalIgnoreCase)
                        select new XElement("path",
                                new XText(file.Uri.ToString()))));

                if (context.Config != null)
                    serviceElement.Add(new XElement("endpoints", 
                        new Configuration(context.Config).GetEndpoints(type.FullName)));

                serviceElement.Add(new XElement("contracts",
                    from contract in contracts.Services
                    where type.ImplementsInterface(contract.Type) || type == contract.Type
                    select new XElement("contract", new XText(contract.Type.FullName))));

                servicesElement.Add(serviceElement);
            }
            return servicesElement;
        }

        private static XElement GetServiceModelConfiguration(Context context)
        {
            if (context.Config == null) return null;
            var configuration = new Configuration(context.Config);
            return new XElement("configuration", 
                from element in configuration.Source.Root.Elements() select element);
        }

        private static XElement GetMetadata(Context context)
        {
            if (context.MergeDocuments == null) return null;
            return new XElement("metadata", from document in context.MergeDocuments select document.Root);
        }
    }
}
