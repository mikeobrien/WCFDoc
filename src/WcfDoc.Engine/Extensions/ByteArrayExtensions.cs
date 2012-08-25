using System.Linq;

namespace WcfDoc.Engine.Extensions
{
    public static class ByteArrayExtensions
    {
        public static string ToHex(this byte[] bytes)
        {
          return bytes.Select(b => string.Format("{0:X2}", b)).Aggregate((a, i) => a + i);
        }
    }
}
