using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace WcfDoc.Engine.Extensions
{
    public static class StringExtensions
    {
        public static string SubstringBefore(this string value, string delimiter)
        {
            int position = value.IndexOf(delimiter);
            if (position > 0) return value.Substring(0, position);
            else return value;
        }

        private static readonly MD5 Md5 = MD5.Create();
        public static string Hash(this string value)
        {
            return Md5.ComputeHash(Encoding.Unicode.GetBytes(value)).ToHex();
        }

        public static string Combine(this string partA, string seperator, string partB)
        {
            if (partA == null) return string.Empty;
            else if (partB == null) return partA;
            if (partA.EndsWith(seperator)) return partA + partB;
            else return partA + seperator + partB;
        }

        public static string ValueOrEmpty(this string value)
        {
            return value ?? string.Empty;
        }
    }
}
