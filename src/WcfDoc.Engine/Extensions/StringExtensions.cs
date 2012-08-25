using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace WcfDoc.Engine.Extensions
{
    public static class StringExtensions
    {
        public static string SubstringBefore(this string value, string delimiter)
        {
            var position = value.IndexOf(delimiter);
            return position > 0 ? value.Substring(0, position) : value;
        }

        private static readonly MD5 Md5 = MD5.Create();
        public static string Hash(this string value)
        {
            return Md5.ComputeHash(Encoding.Unicode.GetBytes(value)).ToHex();
        }

        public static string Combine(this string partA, string seperator, string partB)
        {
            if (partA == null) return string.Empty;
            if (partB == null) return partA;
            if (partA.EndsWith(seperator)) return partA + partB;
            return partA + seperator + partB;
        }

        public static string ValueOrEmpty(this string value)
        {
            return value ?? string.Empty;
        }

        public static string StripUrlParameters(this string url)
        {
            return Regex.Replace(url, "/*\\{.*?\\}", "").Trim('/');
        }
    }
}
