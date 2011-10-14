using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Web;
using WcfDoc.Engine.Extensions;
using WcfDoc.Engine.Contract;

namespace WcfDoc.Engine
{
    public enum ServiceType
    {
        All,
        Soap,
        Rest
    }

    public class Contracts
    {
        // ────────────────────────── Constructor ──────────────────────────

        public Contracts(IEnumerable<Assembly> assemblies, XmlComments xmlComments, ServiceType serviceType)
        {
            var types = new List<ContractType>();
            var services = new List<ContractService>();

            LoadMetadata(assemblies, types, services);
            LoadTypes(assemblies, types, services);
            LoadServices(assemblies, services);
            
            LoadXmlComments(xmlComments, types, services);

            Types = types;

            switch (serviceType)
            {
                case ServiceType.All: Services = services; break;
                case ServiceType.Soap: Services = services.Where(x => !x.Restful).ToList(); break;
                case ServiceType.Rest: Services = services.Where(x => x.Restful).ToList(); break;
            }
        }

        // ────────────────────────── Public Members ──────────────────────────

        public IList<ContractType> Types { get; private set; }
        public IList<ContractService> Services { get; private set; }

        // ────────────────────────── Private Members ──────────────────────────

        private static void LoadMetadata(
            IEnumerable<Assembly> assemblies, 
            List<ContractType> types,
            List<ContractService> services)
        {
            var serviceContracts = assemblies.FindTypes(
                t => t.HasAttribute<ServiceContractAttribute>());

            foreach (var type in serviceContracts)
            {
                var metadata = ContractMetadata.Generate(type);

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

        private static void LoadTypes(
            IEnumerable<Assembly> assemblies, 
            IEnumerable<ContractType> types, 
            IEnumerable<ContractService> services)
        {
            var contractTypes = 
                EnumerateTypes(GetTypeRoots(assemblies, services));
            foreach (var type in types)
            {
                type.Type = contractTypes.FirstOrDefault(
                    t => t.GetDataContractName() == type.Metadata.Name &&
                         t.GetDataContractNamespace() == type.Metadata.Namespace);

                if (type.Type != null)
                {
                    foreach (var member in type.Members)
                    {
                        member.MemberInfo = type.Type.FindMember(
                            m => m.GetDataMemberName() == member.Metadata.Name);

                        if (member.MemberInfo != null)
                            if (member.MemberInfo is FieldInfo)
                                member.Type = ((FieldInfo)member.MemberInfo).FieldType;
                            else if (member.MemberInfo is PropertyInfo)
                                member.Type = ((PropertyInfo)member.MemberInfo).PropertyType;
                    }
                    
                    foreach (var option in type.Options)
                    {
                        option.FieldInfo = type.Type.FindMember<FieldInfo>(
                            f => f.GetEnumMemberName() == option.Metadata.Value);
                    }
                }
            }
        }

        private static IEnumerable<Type> EnumerateTypes(IEnumerable<Type> types)
        {
            var enumeratedTypes = new List<Type>();
            var typeQueue = new Queue<Type>(types);

            Func<Type, bool> canEnqueue = t => 
                            !enumeratedTypes.Contains(t) &&
                            !typeQueue.Contains(t);

            while (typeQueue.Count > 0)
            {
                var type = typeQueue.Dequeue();
                if (!enumeratedTypes.Contains(type)) enumeratedTypes.Add(type);

                if (type.IsGenericType)
                {
                    var typeArguments = EnumerateGenericTypes(type);
                    foreach (var typeArgument in typeArguments.Where(canEnqueue))
                    {
                        typeQueue.Enqueue(typeArgument);
                    }
                }

                if (type.IsEnumerable())
                {
                    var enumerableType = type.GetEnumerableType();
                    if (canEnqueue(enumerableType))
                        typeQueue.Enqueue(enumerableType);                    
                }

                var members = type.GetMembers();

                foreach (var member in members)
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

        private static IEnumerable<Type> EnumerateGenericTypes(Type type)
        {
            if (!type.IsGenericType) return null;

            var types = new List<Type>();
            var typeQueue = new Queue<Type>();
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
                    var typeArguments = currentType.GetGenericArguments();
                    foreach (var typeArgument in typeArguments)
                        if (!types.Contains(typeArgument) &&
                            !typeQueue.Contains(typeArgument))
                            typeQueue.Enqueue(typeArgument);
                }
            } while (typeQueue.Count > 0);

            return types;
        }

        private static IEnumerable<Type> GetTypeRoots(
            IEnumerable<Assembly> assemblies, 
            IEnumerable<ContractService> services)
        {
            var typeRoots = new List<Type>();
            foreach (var service in services)
            {
                service.Type = assemblies.FindType(
                    t => t.HasAttribute<ServiceContractAttribute>() &&
                         t.GetServiceContractName() == service.Metadata.TypeName &&
                         t.GetServiceContractNamespace() == service.Metadata.TypeNamespace);

                if (service.Type != null)
                {
                    foreach (var operation in service.Operations)
                    {
                        var methodInfo = service.Type.FindMember<MethodInfo>(
                            m => m.GetOperationName() == operation.Metadata.Name);

                        if (methodInfo != null)
                        {
                            if (methodInfo.ReturnType != typeof(void) && !typeRoots.Contains(methodInfo.ReturnType)) 
                                typeRoots.Add(methodInfo.ReturnType);

                            var parameters = methodInfo.GetParameters();
                            typeRoots.AddRange(from parameter in parameters 
                                               where !typeRoots.Contains(parameter.ParameterType) 
                                               select parameter.ParameterType);
                        }
                    }
                }
            }
            return typeRoots;
        }

        private static void LoadServices(
            IEnumerable<Assembly> assemblies, 
            IEnumerable<ContractService> services)
        {
            foreach (var service in services)
            {
                service.Restful = false;
                service.Type = assemblies.FindType(
                    t => t.HasAttribute<ServiceContractAttribute>() &&
                         t.GetServiceContractName() == service.Metadata.TypeName &&
                         t.GetServiceContractNamespace() == service.Metadata.TypeNamespace);

                if (service.Type != null)
                {
                    service.Properties = service.Type.GetAttribute<ServiceContractAttribute>();

                    foreach (var operation in service.Operations)
                    {
                        operation.MethodInfo = service.Type.FindMember<MethodInfo>(
                            m => m.GetOperationName() == operation.Metadata.Name);

                        if (operation.MethodInfo != null)
                        {
                            operation.Properties = operation.MethodInfo.GetAttribute<OperationContractAttribute>();

                            if (operation.MethodInfo.ReturnType != typeof(void))
                                operation.ReturnType.Type = operation.MethodInfo.ReturnType;

                            var webGet = operation.MethodInfo.GetAttribute<WebGetAttribute>();
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

                            foreach (var parameter in operation.Parameters)
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

        private static void LoadXmlComments(
            XmlComments xmlComments,
            IEnumerable<ContractType> types,
            IEnumerable<ContractService> services)
        {
            if (xmlComments == null) return;

            var memberMapping = 
                new Dictionary<string, XmlComments.XmlMemberInfo>();

            Action<string, XmlComments.XmlMemberInfo> addMemberMapping = 
                (k, v) => {
                    if (!memberMapping.ContainsKey(k))
                        memberMapping.Add(k, v);
                    else throw new Exception(string.Format("Duplicate xml comment member found: {0}.{1}", v.Assembly, v.Name));
                };

            foreach (var type in types)
            {
                type.Comments = xmlComments.GetTypeComments(type.Type);
                if (type.Type != null)
                    addMemberMapping(type.Metadata.Id, XmlComments.GetMemberInfo(type.Type));

                foreach (var member in type.Members)
                {
                    member.Comments =
                        xmlComments.GetFieldOrPropertyComments(member.MemberInfo);
                    addMemberMapping(member.Metadata.Id, XmlComments.GetMemberInfo(member.MemberInfo));
                }

                foreach (var option in type.Options)
                    option.Comments = xmlComments.GetFieldComments(option.FieldInfo);
            }

            foreach (var service in services)
            {
                service.Comments = xmlComments.GetTypeComments(service.Type);
                addMemberMapping(service.Metadata.TypeId, XmlComments.GetMemberInfo(service.Type));

                foreach (var operation in service.Operations)
                {
                    operation.Comments = xmlComments.GetMethodComments(operation.MethodInfo);
                    addMemberMapping(operation.Metadata.Id, XmlComments.GetMemberInfo(operation.MethodInfo));

                    foreach (var parameter in operation.Parameters.Where(parameter => parameter.ParameterInfo != null))
                    {
                        parameter.Comments = 
                            xmlComments.GetMethodParameterComments(operation.MethodInfo, parameter.ParameterInfo.Name);
                    }

                    operation.ReturnType.Comments = xmlComments.GetMethodReturnTypeComments(operation.MethodInfo);
                }
            }
            xmlComments.ReplaceMemberReferences(memberMapping);
        }
    }
}
