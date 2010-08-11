using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfDoc.Engine.Extensions
{
    public static class ByteArrayExtensions
    {
        public static string ToHex(this byte[] bytes)
        {
          StringBuilder hex = new StringBuilder(bytes.Length * 2);
          foreach (byte b in bytes)
            hex.AppendFormat("{0:x2}", b);
          return hex.ToString();
        }
    }
}
