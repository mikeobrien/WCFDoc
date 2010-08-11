using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        private XDocument _xmlComments;

        // ────────────────────────── Constructor ──────────────────────────

        public XmlComments(IEnumerable<XDocument> xmlComments)
        {
            if (xmlComments == null) return;
            _xmlComments = new XDocument();
            XElement members = new XElement("members");
            _xmlComments.Add(new XElement("doc", members));

            foreach (XDocument document in xmlComments)
            {
                string assembly = document.XPathSelectElement("/doc/assembly/name").Value;
                IEnumerable<XElement> elements = document.XPathSelectElements("/doc/members/member");
                foreach (XElement element in elements)
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
            IEnumerable<XElement> comments = GetAllMethodComments(method);
            if (comments != null)
                return comments.FirstOrDefault(
                e => e.Name == "param" && e.Attribute("name").Value == name);
            else return null;
        }

        public XElement GetMethodReturnTypeComments(MethodInfo method)
        {
            if (_xmlComments == null || method == null) return null;
            IEnumerable<XElement> comments = GetAllMethodComments(method);
            if (comments != null) return comments.FirstOrDefault(e => e.Name == "returns");
            else return null;
        }

        public IEnumerable<XElement> GetMethodComments(MethodInfo method)
        {
            if (_xmlComments == null || method == null) return null;
            IEnumerable<XElement> comments = GetAllMethodComments(method);
            if (comments != null) 
                return comments.Where(e => e.Name != "param" && e.Name != "returns");
            else return null;
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
            else if (member is PropertyInfo)
                return GetPropertyComments((PropertyInfo)member);
            else
                throw new ArgumentException("Must be of type FieldInfo or PropertyInfo.", "member");
        }

        public void ReplaceMemberReferences(Dictionary<string, XmlComments.XmlMemberInfo> memberMapping)
        {
            if (_xmlComments == null || memberMapping == null) return;
            IEnumerable<XElement> members = _xmlComments.XPathSelectElements("/doc/members/member");
            foreach (XElement member in members)
            {
                string[] elements = new string[] { "exception", "permission", "seealso", "see"};
                foreach (string element in elements)
                {
                    IEnumerable<XElement> memberReferences = member.XPathSelectElements("//" + element);
                    foreach (XElement memberReference in memberReferences)
                    {
                        if (memberReference.Attribute("cref") != null && 
                            !string.IsNullOrEmpty(memberReference.Attribute("cref").Value))
                        {
                            KeyValuePair<string, XmlComments.XmlMemberInfo> match = memberMapping.FirstOrDefault(
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
            return new XmlMemberInfo()
            {
                Name = GetMemberName(type),
                Assembly = GetAssemblyName(type)
            };
        }

        public static XmlMemberInfo GetMemberInfo(MemberInfo member)
        {
            return new XmlMemberInfo()
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
            Type type = null;
            if (member is MethodInfo)
                type = ((MethodInfo)member).DeclaringType;
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
                MethodInfo method = (MethodInfo)member;
                ParameterInfo[] parameters = method.GetParameters();
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

                string methodSignature =
                    string.Format("{0}.{1}{2}",
                        method.DeclaringType.FullName,
                        method.Name,
                        methodParameters);

                return string.Format("M:{0}", methodSignature);
            }
            else if (member is PropertyInfo)
            {
                PropertyInfo property = (PropertyInfo)member;
                Type type;
                if (property.DeclaringType.IsGenericType)
                    type = property.DeclaringType.GetGenericTypeDefinition();
                else type = property.DeclaringType;

                return string.Format("P:{0}.{1}", type.FullName, property.Name);
            }
            else if (member is FieldInfo)
            {
                FieldInfo field = (FieldInfo)member;
                Type type;
                if (field.DeclaringType.IsGenericType)
                    type = field.DeclaringType.GetGenericTypeDefinition();
                else type = field.DeclaringType;

                return string.Format("F:{0}.{1}", type.FullName, field.Name);
            }
            else return string.Empty;
        }

        // ────────────────────────── Nested Types ──────────────────────────

        public class XmlMemberInfo
        {
            public string Name { get; set; }
            public string Assembly { get; set; }
        }
    }
}
