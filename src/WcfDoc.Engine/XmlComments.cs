using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using System.IO;
using System.Reflection;
using WcfDoc.Engine.Extensions;

namespace WcfDoc.Engine
{
    public class XmlComments
    {
        // ────────────────────────── Private Fields ──────────────────────────

        private readonly XDocument _xmlComments;

        // ────────────────────────── Constructor ──────────────────────────

        public XmlComments(IEnumerable<XDocument> xmlComments)
        {
            if (xmlComments == null) return;
            _xmlComments = new XDocument();
            var members = new XElement("members");
            _xmlComments.Add(new XElement("doc", members));

            foreach (var document in xmlComments)
            {
                var assembly = document.XPathSelectElement("/doc/assembly/name").Value;
                var elements = document.XPathSelectElements("/doc/members/member");
                foreach (var element in elements)
                {
                    members.Add(new XElement("member", 
                        new XAttribute("assembly", assembly),
                        element.Attributes(),
                        element.Elements()));
                }
            }
        }

        // ────────────────────────── Public Members ──────────────────────────

        public IEnumerable<XElement> GetTypeComments(Type type)
        {
            if (_xmlComments == null || type == null) return null;
            return GetMember(GetMemberName(type), GetAssemblyName(type)).GetChildren();
        }

        public XElement GetMethodParameterComments(MethodInfo method, string name)
        {
            if (_xmlComments == null || method == null || string.IsNullOrEmpty(name)) return null;
            var comments = GetAllMethodComments(method);
            if (comments != null)
                return comments.FirstOrDefault(
                e => e.Name == "param" && e.Attribute("name").Value == name);
            return null;
        }

        public XElement GetMethodReturnTypeComments(MethodInfo method)
        {
            if (_xmlComments == null || method == null) return null;
            var comments = GetAllMethodComments(method);
            return comments != null ? comments.FirstOrDefault(e => e.Name == "returns") : null;
        }

        public IEnumerable<XElement> GetMethodComments(MethodInfo method)
        {
            if (_xmlComments == null || method == null) return null;
            var comments = GetAllMethodComments(method);
            return comments != null ? comments.Where(e => e.Name != "param" && e.Name != "returns") : null;
        }

        public IEnumerable<XElement> GetAllMethodComments(MethodInfo method)
        {
            if (_xmlComments == null || method == null) return null;
            return GetMember(GetMemberName(method), GetAssemblyName(method)).GetChildren();
        }

        public IEnumerable<XElement> GetPropertyComments(PropertyInfo property)
        {
            if (_xmlComments == null || property == null) return null;
            return GetMember(GetMemberName(property), GetAssemblyName(property)).GetChildren();
        }

        public IEnumerable<XElement> GetFieldComments(FieldInfo field)
        {
            if (_xmlComments == null || field == null) return null;
            return GetMember(GetMemberName(field), GetAssemblyName(field)).GetChildren();
        }

        public IEnumerable<XElement> GetFieldOrPropertyComments(MemberInfo member)
        {
            if (_xmlComments == null || member == null) return null;
            if (member is FieldInfo)
                return GetFieldComments((FieldInfo)member);
            if (member is PropertyInfo)
                return GetPropertyComments((PropertyInfo)member);
            throw new ArgumentException("Must be of type FieldInfo or PropertyInfo.", "member");
        }

        public void ReplaceMemberReferences(Dictionary<string, XmlMemberInfo> memberMapping)
        {
            if (_xmlComments == null || memberMapping == null) return;
            var members = _xmlComments.XPathSelectElements("/doc/members/member");
            foreach (var member in members)
            {
                var elements = new [] { "exception", "permission", "seealso", "see"};
                foreach (var element in elements)
                {
                    var memberReferences = member.XPathSelectElements("//" + element);
                    foreach (var memberReference in memberReferences)
                    {
                        if (memberReference.Attribute("cref") != null && 
                            !string.IsNullOrEmpty(memberReference.Attribute("cref").Value))
                        {
                            var match = memberMapping.FirstOrDefault(
                                i => i.Value.Assembly == member.Attribute("assembly").Value && 
                                     i.Value.Name == memberReference.Attribute("cref").Value);
                            if (!string.IsNullOrEmpty(match.Key)) memberReference.Attribute("cref").Value = match.Key;
                        }
                    }
                }
            }
        }

        public static XmlMemberInfo GetMemberInfo(Type type)
        {
            return new XmlMemberInfo
            {
                Name = GetMemberName(type),
                Assembly = GetAssemblyName(type)
            };
        }

        public static XmlMemberInfo GetMemberInfo(MemberInfo member)
        {
            return new XmlMemberInfo
            {
                Name = GetMemberName(member),
                Assembly = GetAssemblyName(member)
            };
        }

        // ────────────────────────── Private Members ──────────────────────────

        private XElement GetMember(string name, string assembly)
        {
            return _xmlComments.XPathSelectElement(
                string.Format("/doc/members/member[@name='{0}' and @assembly='{1}']", name, assembly));
        }

        private static string GetAssemblyName(Type type)
        {
            if (type.IsGenericType) type = type.GetGenericTypeDefinition();
            return Path.GetFileNameWithoutExtension(type.Assembly.ManifestModule.Name);
        }

        private static string GetAssemblyName(MemberInfo member)
        {
            Type type;
            if (member is MethodInfo)
                type = member.DeclaringType;
            else if (member is PropertyInfo)
                type = ((PropertyInfo)member).PropertyType;
            else if (member is FieldInfo)
                type = ((FieldInfo)member).FieldType;
            else return string.Empty;

            return Path.GetFileNameWithoutExtension(type.Assembly.ManifestModule.Name);
        }

        private static string GetMemberName(Type type)
        {
            if (type.IsGenericType) type = type.GetGenericTypeDefinition();
            return string.Format("T:{0}", type.FullName);
        }

        private static string GetMemberName(MemberInfo member)
        {
            if (member is MethodInfo)
            {
                var method = (MethodInfo)member;
                var parameters = method.GetParameters();
                string methodParameters;
                if (parameters.Length > 0)
                    methodParameters =
                        string.Format("({0})",
                        string.Join(",", (from parameter in parameters
                          select parameter.ParameterType.IsGenericType ?
                          string.Format("{0}{{{1}}}",
                            parameter.ParameterType.GetGenericTypeDefinition().FullName.SubstringBefore("`"),
                            string.Join(",", (from type in parameter.ParameterType.GetGenericArguments() select type.FullName).ToArray())) :
                          parameter.ParameterType.FullName).ToArray()));
                else
                    methodParameters = string.Empty;

                var methodSignature =
                    string.Format("{0}.{1}{2}",
                        method.DeclaringType.FullName,
                        method.Name,
                        methodParameters);

                return string.Format("M:{0}", methodSignature);
            }

            if (member is PropertyInfo)
            {
                var property = (PropertyInfo)member;
                var type = property.DeclaringType.IsGenericType ? property.DeclaringType.GetGenericTypeDefinition() : property.DeclaringType;
                return string.Format("P:{0}.{1}", type.FullName, property.Name);
            }
            if (member is FieldInfo)
            {
                var field = (FieldInfo)member;
                var type = field.DeclaringType.IsGenericType ? field.DeclaringType.GetGenericTypeDefinition() : field.DeclaringType;
                return string.Format("F:{0}.{1}", type.FullName, field.Name);
            }
            return string.Empty;
        }

        // ────────────────────────── Nested Types ──────────────────────────

        public class XmlMemberInfo
        {
            public string Name { get; set; }
            public string Assembly { get; set; }
        }
    }
}
