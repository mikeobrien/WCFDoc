using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace WcfDoc.Engine.Extensions
{
    public static class MemberInfoExtensions
    {
        public static bool TryGetAttribute<T>(this MemberInfo member, ref T attribute) where T : Attribute
        {
            attribute = (T)member.GetCustomAttributes(typeof(T), true).FirstOrDefault();
            return (attribute != null);
        }

        public static T GetAttribute<T>(this MemberInfo member) where T : Attribute
        {
            return (T)member.GetCustomAttributes(typeof(T), true).FirstOrDefault();
        }

        public static string GetDataMemberName(this MemberInfo member)
        {
            DataMemberAttribute dataMember = null;
            if (member.TryGetAttribute(ref dataMember) &&
                !string.IsNullOrEmpty(dataMember.Name)) return dataMember.Name;
            return member.Name;
        }

        public static string GetEnumMemberName(this MemberInfo member)
        {
            EnumMemberAttribute enumMember = null;
            if (member.TryGetAttribute(ref enumMember) &&
                !string.IsNullOrEmpty(enumMember.Value)) return enumMember.Value;
            return member.Name;
        }
    }
}
