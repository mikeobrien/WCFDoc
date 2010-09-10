using System.Linq;

namespace WcfDoc.Engine.Extensions
{
    public static class ByteArrayExtensions
    {
        public static string ToHex(this byte[] bytes)
        {
          return bytes.Select(b => b.ToString("{0:x2}")).Aggregate((a, i) => a + i);
        }
    }
}
