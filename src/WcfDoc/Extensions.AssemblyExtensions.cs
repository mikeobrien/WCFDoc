using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace DBGhost.Build.Extensions
{
    public static class AssemblyExtensions
    {
        public static Stream FindManifestResourceStream(this Assembly assembly, string name)
        {
            string[] names = assembly.GetManifestResourceNames();
            if (name != null && names.Length > 0)
            {
                string resourceName = names.OrderByDescending(n => n.Length).
                    FirstOrDefault(n => n.EndsWith(name));
                if (!string.IsNullOrEmpty(resourceName))
                    return assembly.GetManifestResourceStream(resourceName);
            }
            return null;
        }
    }
}
