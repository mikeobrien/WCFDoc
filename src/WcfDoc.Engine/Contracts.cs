using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Xml.Linq;
using System.ServiceModel.Web;
using WcfDoc.Engine.Extensions;

namespace WcfDoc.Engine
{
    public class Contracts
    {
        // ────────────────────────── Constructor ──────────────────────────

        public Contracts(IEnumerable<Assembly> assemblies, XmlComments xmlComments)
        {
            Types = new List<ContractType>();
            Services = new List<ContractService>();

            LoadMetadata(assemblies, Types, Services);
            LoadTypes(assemblies, Types, Services);
            LoadServices(assemblies, Services);
            
            LoadXmlComments(xmlComments, Types, Services);
        }

        // ────────────────────────── Public Members ──────────────────────────

        public List<ContractType> Types { get; private set; }
        public List<ContractService> Services { get; private set; }

        // ────────────────────────── Private Members ──────────────────────────

        private void LoadMetadata(
            IEnumerable<Assembly> assemblies, 
            List<ContractType> types,
            List<ContractService> services)
        {
            IEnumerable<Type> serviceContracts = assemblies.FindTypes(
                t => t.HasAttribute<ServiceContractAttribute>());

            foreach (Type type in serviceContracts)
            {
                ContractMetadata metadata = ContractMetadata.Generate(type);

                types.AddRange(
                    from serviceType in metadata.Types
                    where !types.Exists(t => 
                        t.Metadata.Name == serviceType.Name && 
                        t.Metadata.Namespace == serviceType.Namespace)
                    select new ContractType()
                    {
                        Metadata = serviceType,
                        Members = (from member in serviceType.Members
                                  select new ContractMember()
                                  {
                                      Metadata = member
                                  }).ToList(),
                        Options = (from option in serviceType.Options
                                  select new ContractOption()
                                  {
                                      Metadata = option 
                                  }).ToList(),
                    });

                services.AddRange(
                    from service in metadata.Services
                    select new ContractService()
                    {
                        Metadata = service,
                        Operations = (from operation in service.Operations
                                     select new ContractOperation()
                                     {
                                         Metadata = operation,
                                         Parameters = (from parameter in operation.Parameters
                                                      select new ContractParameter()
                                                      {
                                                          Metadata = parameter
                                                      }).ToList(),
                                         ReturnType = new ContractReturnType()
                                         {
                                              Metadata = operation.ReturnType
                                         }
                                     }).ToList()
                    });
            }
        }

        private void LoadTypes(
            IEnumerable<Assembly> assemblies, 
            IEnumerable<ContractType> types, 
            IEnumerable<ContractService> services)
        {
            IEnumerable<Type> contractTypes = 
                EnumerateTypes(GetTypeRoots(assemblies, services));
            foreach (ContractType type in types)
            {
                type.Type = contractTypes.FirstOrDefault(
                    t => t.GetDataContractName() == type.Metadata.Name &&
                         t.GetDataContractNamespace() == type.Metadata.Namespace);

                if (type.Type != null)
                {
                    foreach (ContractMember member in type.Members)
                    {
                        member.MemberInfo = type.Type.FindMember(
                            m => m.GetDataMemberName() == member.Metadata.Name);

                        if (member.MemberInfo != null)
                            if (member.MemberInfo is FieldInfo)
                                member.Type = ((FieldInfo)member.MemberInfo).FieldType;
                            else if (member.MemberInfo is PropertyInfo)
                                member.Type = ((PropertyInfo)member.MemberInfo).PropertyType;
                    }
                    
                    foreach (ContractOption option in type.Options)
                    {
                        option.FieldInfo = type.Type.FindMember<FieldInfo>(
                            f => f.GetEnumMemberName() == option.Metadata.Value);
                    }
                }
            }
        }

        private IEnumerable<Type> EnumerateTypes(IEnumerable<Type> types)
        {
            List<Type> enumeratedTypes = new List<Type>();
            Queue<Type> typeQueue = new Queue<Type>(types);

            Func<Type, bool> canEnqueue = t => 
                            !enumeratedTypes.Contains(t) &&
                            !typeQueue.Contains(t);

            while (typeQueue.Count > 0)
            {
                Type type = typeQueue.Dequeue();
                if (!enumeratedTypes.Contains(type)) enumeratedTypes.Add(type);

                if (type.IsGenericType)
                {
                    IEnumerable<Type> typeArguments = EnumerateGenericTypes(type);
                    foreach (Type typeArgument in typeArguments)
                        if (canEnqueue(typeArgument))
                            typeQueue.Enqueue(typeArgument);
                }

                if (type.IsEnumerable())
                {
                    Type enumerableType = type.GetEnumerableType();
                    if (canEnqueue(enumerableType))
                        typeQueue.Enqueue(enumerableType);                    
                }

                MemberInfo[] members = type.GetMembers();

                foreach (MemberInfo member in members)
                {
                    Type memberType = null;
                    if (member is FieldInfo)
                        memberType = ((FieldInfo)member).FieldType;
                    else if (member is PropertyInfo)
                        memberType = ((PropertyInfo)member).PropertyType;

                    if (memberType != null && canEnqueue(memberType))
                        typeQueue.Enqueue(memberType);
                }
            }

            return enumeratedTypes;
        }

        private IEnumerable<Type> EnumerateGenericTypes(Type type)
        {
            if (type.IsGenericType)
            {
                List<Type> types = new List<Type>();
                Queue<Type> typeQueue = new Queue<Type>();
                do
                {
                    Type currentType;
                    if (typeQueue.Count == 0) currentType = type;
                    else
                    {
                        currentType = typeQueue.Dequeue();
                        if (!types.Contains(currentType)) types.Add(currentType);
                    }
                    if (currentType.IsGenericType)
                    {
                        Type[] typeArguments = currentType.GetGenericArguments();
                        foreach (Type typeArgument in typeArguments)
                            if (!types.Contains(typeArgument) &&
                                !typeQueue.Contains(typeArgument))
                                typeQueue.Enqueue(typeArgument);
                    }
                } while (typeQueue.Count > 0);

                return types;
            }
            else return null;
        }

        private IEnumerable<Type> GetTypeRoots(
            IEnumerable<Assembly> assemblies, 
            IEnumerable<ContractService> services)
        {
            List<Type> typeRoots = new List<Type>();
            foreach (ContractService service in services)
            {
                service.Type = assemblies.FindType(
                    t => t.HasAttribute<ServiceContractAttribute>() &&
                         t.GetServiceContractName() == service.Metadata.TypeName &&
                         t.GetServiceContractNamespace() == service.Metadata.TypeNamespace);

                if (service.Type != null)
                {
                    foreach (ContractOperation operation in service.Operations)
                    {
                        MethodInfo methodInfo = service.Type.FindMember<MethodInfo>(
                            m => m.GetOperationName() == operation.Metadata.Name);

                        if (methodInfo != null)
                        {
                            if (methodInfo.ReturnType != typeof(void) && !typeRoots.Contains(methodInfo.ReturnType)) 
                                typeRoots.Add(methodInfo.ReturnType);

                            ParameterInfo[] parameters = methodInfo.GetParameters();
                            typeRoots.AddRange(from parameter in parameters 
                                               where !typeRoots.Contains(parameter.ParameterType) 
                                               select parameter.ParameterType);
                        }
                    }
                }
            }
            return typeRoots;
        }

        private void LoadServices(
            IEnumerable<Assembly> assemblies, 
            IEnumerable<ContractService> services)
        {
            foreach (ContractService service in services)
            {
                service.Restful = false;
                service.Type = assemblies.FindType(
                    t => t.HasAttribute<ServiceContractAttribute>() &&
                         t.GetServiceContractName() == service.Metadata.TypeName &&
                         t.GetServiceContractNamespace() == service.Metadata.TypeNamespace);

                if (service.Type != null)
                {
                    service.Properties = service.Type.GetAttribute<ServiceContractAttribute>();

                    foreach (ContractOperation operation in service.Operations)
                    {
                        operation.MethodInfo = service.Type.FindMember<MethodInfo>(
                            m => m.GetOperationName() == operation.Metadata.Name);

                        if (operation.MethodInfo != null)
                        {
                            operation.Properties = operation.MethodInfo.GetAttribute<OperationContractAttribute>();

                            if (operation.MethodInfo.ReturnType != typeof(void))
                                operation.ReturnType.Type = operation.MethodInfo.ReturnType;

                            WebGetAttribute webGet = operation.MethodInfo.GetAttribute<WebGetAttribute>();
                            UriTemplate uriTemplate = null;
                            if (webGet != null)
                                operation.RestfulProperties = new WebInvokeAttribute()
                                {
                                    BodyStyle = webGet.BodyStyle,
                                    Method = "GET",
                                    RequestFormat = webGet.RequestFormat,
                                    ResponseFormat = webGet.ResponseFormat,
                                    UriTemplate = webGet.UriTemplate
                                };
                            else
                                operation.RestfulProperties = operation.MethodInfo.GetAttribute<WebInvokeAttribute>();

                            if (operation.RestfulProperties != null)
                            {
                                service.Restful = true;
                                uriTemplate = new UriTemplate(operation.RestfulProperties.UriTemplate);
                            }

                            foreach (ContractParameter parameter in operation.Parameters)
                            {
                                parameter.ParameterInfo = operation.MethodInfo.GetParameter(parameter.Metadata.Name);
                                if (parameter.ParameterInfo != null)
                                {
                                    parameter.Type = parameter.ParameterInfo.ParameterType;

                                    if (uriTemplate != null)
                                        if (uriTemplate.QueryValueVariableNames.FirstOrDefault(
                                            i => i.Equals(parameter.ParameterInfo.Name, StringComparison.OrdinalIgnoreCase)) != null)
                                            parameter.RestfulType = "QueryString";
                                        else if (uriTemplate.PathSegmentVariableNames.FirstOrDefault(
                                            i => i.Equals(parameter.ParameterInfo.Name, StringComparison.OrdinalIgnoreCase)) != null)
                                            parameter.RestfulType = "PathSegment";
                                        else
                                            parameter.RestfulType = "EntityBody";
                                }
                            }
                        }
                    }
                }
            }
        }

        private void LoadXmlComments(
            XmlComments xmlComments,
            List<ContractType> types,
            List<ContractService> services)
        {
            if (xmlComments == null) return;

            Dictionary<string, XmlComments.XmlMemberInfo> memberMapping = 
                new Dictionary<string, XmlComments.XmlMemberInfo>();

            Action<string, XmlComments.XmlMemberInfo> addMemberMapping = 
                (k, v) => {
                    if (!memberMapping.ContainsKey(k))
                        memberMapping.Add(k, v);
                    else throw new Exception(string.Format("Duplicate xml comment member found: {0}.{1}", v.Assembly, v.Name));
                };

            foreach (ContractType type in types)
            {
                type.Comments = xmlComments.GetTypeComments(type.Type);
                if (type.Type != null)
                    addMemberMapping(type.Metadata.Id, XmlComments.GetMemberInfo(type.Type));

                foreach (ContractMember member in type.Members)
                {
                    member.Comments =
                        xmlComments.GetFieldOrPropertyComments(member.MemberInfo);
                    addMemberMapping(member.Metadata.Id, XmlComments.GetMemberInfo(member.MemberInfo));
                }

                foreach (ContractOption option in type.Options)
                    option.Comments = xmlComments.GetFieldComments(option.FieldInfo);
            }

            foreach (ContractService service in services)
            {
                service.Comments = xmlComments.GetTypeComments(service.Type);
                addMemberMapping(service.Metadata.TypeId, XmlComments.GetMemberInfo(service.Type));

                foreach (ContractOperation operation in service.Operations)
                {
                    operation.Comments = xmlComments.GetMethodComments(operation.MethodInfo);
                    addMemberMapping(operation.Metadata.Id, XmlComments.GetMemberInfo(operation.MethodInfo));

                    foreach (ContractParameter parameter in operation.Parameters)
                        if (parameter.ParameterInfo != null)
                            parameter.Comments = 
                                xmlComments.GetMethodParameterComments(operation.MethodInfo, parameter.ParameterInfo.Name);

                    operation.ReturnType.Comments = xmlComments.GetMethodReturnTypeComments(operation.MethodInfo);
                }
            }
            xmlComments.ReplaceMemberReferences(memberMapping);
        }

        // ────────────────────────── Nested Types ──────────────────────────

        public class ContractType
        {
            public ContractMetadata.TypeMetadata Metadata { get; set; }
            public Type Type { get; set; }
            public IEnumerable<XElement> Comments { get; set; }
            public IEnumerable<ContractMember> Members { get; set; }
            public IEnumerable<ContractOption> Options { get; set; }
        }

        public class ContractOption
        {
            public ContractMetadata.OptionMetadata Metadata { get; set; }
            public FieldInfo FieldInfo { get; set; }
            public IEnumerable<XElement> Comments { get; set; }
        }

        public class ContractMember
        {
            public ContractMetadata.MemberMetadata Metadata { get; set; }
            public Type Type { get; set; }
            public MemberInfo MemberInfo { get; set; }
            public IEnumerable<XElement> Comments { get; set; }
        }

        public class ContractService
        {
            public ContractMetadata.ServiceMetadata Metadata { get; set; }
            public Type Type { get; set; }
            public ServiceContractAttribute Properties { get; set; }
            public bool Restful { get; set; }
            public IEnumerable<XElement> Comments { get; set; }
            public IEnumerable<ContractOperation> Operations { get; set; }
        }

        public class ContractOperation
        {
            public ContractMetadata.OperationMetadata Metadata { get; set; }
            public OperationContractAttribute Properties { get; set; }
            public MethodInfo MethodInfo { get; set; }
            public WebInvokeAttribute RestfulProperties { get; set; }
            public IEnumerable<XElement> Comments { get; set; }
            public IEnumerable<ContractParameter> Parameters { get; set; }
            public ContractReturnType ReturnType { get; set; }
        }

        public class ContractParameter
        {
            public ContractMetadata.ParameterMetadata Metadata { get; set; }
            public Type Type { get; set; }
            public ParameterInfo ParameterInfo { get; set; }
            public string RestfulType { get; set; }
            public XElement Comments { get; set; }
        }

        public class ContractReturnType
        {
            public ContractMetadata.ReturnTypeMetadata Metadata { get; set; }
            public Type Type { get; set; }
            public XElement Comments { get; set; }
        }
    }
}
