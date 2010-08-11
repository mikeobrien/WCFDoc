using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;

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
            if (member.TryGetAttribute<DataMemberAttribute>(ref dataMember) &&
                !string.IsNullOrEmpty(dataMember.Name)) return dataMember.Name;
            else
                return member.Name;
        }

        public static string GetEnumMemberName(this MemberInfo member)
        {
            EnumMemberAttribute enumMember = null;
            if (member.TryGetAttribute<EnumMemberAttribute>(ref enumMember) &&
                !string.IsNullOrEmpty(enumMember.Value)) return enumMember.Value;
            else
                return member.Name;
        }
    }
}
