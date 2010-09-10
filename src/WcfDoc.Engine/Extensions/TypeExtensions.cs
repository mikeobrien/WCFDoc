using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;

namespace WcfDoc.Engine.Extensions
{
    public static class TypeExtensions
    {        
        public static bool HasAttribute<T>(this Type type) where T : Attribute
        {
            return (type.GetCustomAttributes(typeof(T), true).Length > 0);
        }

        public static bool TryGetAttribute<T>(this Type type, ref T attribute) where T : Attribute
        {
            attribute = (T)type.GetCustomAttributes(typeof(T), true).FirstOrDefault();
            return (attribute != null);
        }

        public static T GetAttribute<T>(this Type type) where T : Attribute
        {
            return (T)type.GetCustomAttributes(typeof(T), true).FirstOrDefault();
        }

        public static bool IsEnumerable(this Type type)
        {
            return type.GetInterfaces().FirstOrDefault(
                t => t.IsGenericType && 
                    t.GetGenericTypeDefinition() == typeof(IEnumerable<>)) != null;
        }

        public static Type GetEnumerableType(this Type type)
        {
            var enumerableInterface = type.GetInterfaces().FirstOrDefault(
                t => t.IsGenericType && 
                    t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            return enumerableInterface != null ? enumerableInterface.GetGenericArguments()[0] : null;
        }

        public static bool ImplementsInterface(this Type type, Type interfaceType)
        {
            return ImplementsInterface(type, new [] { interfaceType });
        }

        public static bool ImplementsInterface(this Type type, IEnumerable<Type> interfaces)
        {
            return interfaces.Any(interfaceType => type.FindInterfaces((t, c) => t == (Type)c, interfaceType).Length > 0);
        }

        public static string GetServiceContractName(this Type type)
        {
            ServiceContractAttribute serviceContract = null;
            if (type.TryGetAttribute(ref serviceContract) &&
                !string.IsNullOrEmpty(serviceContract.Name)) return serviceContract.Name;
            return type.Name;
        }

        public static string GetServiceContractNamespace(this Type type)
        {
            ServiceContractAttribute serviceContract = null;
            if (type.TryGetAttribute(ref serviceContract) &&
                serviceContract.Namespace != null) return serviceContract.Namespace;
            return "http://tempuri.org/";
        }

        public static string GetDataContractName(this Type type)
        {
            return DataContractTypeInfo.Generate(type).Name;
        }

        public static string GetDataContractNamespace(this Type type)
        {
            return DataContractTypeInfo.Generate(type).TypeNamespace;
        }

        public static MemberInfo FindMember(this Type type, Predicate<MemberInfo> predicate)
        {
            return type.GetMembers().Where(m => predicate(m)).FirstOrDefault();
        }

        public static T FindMember<T>(this Type type, Predicate<T> predicate) where T : MemberInfo
        {
            return (T)type.GetMembers().Where(
                m => m is T && predicate((T)m)).FirstOrDefault();
        }
    }
}
