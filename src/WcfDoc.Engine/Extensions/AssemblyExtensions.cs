using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;

namespace WcfDoc.Engine.Extensions
{
    public static class AssemblyExtensions
    {
        public static Stream FindManifestResourceStream(this Assembly assembly, string name)
        {
            var names = assembly.GetManifestResourceNames();
            if (name != null && names.Length > 0)
            {
                var resourceName = names.OrderByDescending(n => n.Length).
                    FirstOrDefault(n => n.EndsWith(name));
                if (!string.IsNullOrEmpty(resourceName))
                    return assembly.GetManifestResourceStream(resourceName);
            }
            return null;
        }

        public class AssemblyLoadFailure : Exception
        {
            public AssemblyLoadFailure(Exception exception, Assembly assembly) :
                base(string.Format("Error loading assembly '{0}': {1}", assembly.FullName, exception.Message), exception) { }
        }

        public class AssemblyTypeLoadFailure : Exception
        {
            public AssemblyTypeLoadFailure(ReflectionTypeLoadException exception, Assembly assembly) :
                base(string.Format("Error loading types in assembly '{0}': {1}", assembly.FullName, exception.LoaderExceptions[0].Message), exception) { }
        }

        public static IEnumerable<Type> FindTypes(this IEnumerable<Assembly> assemblies, Predicate<Type> predicate)
        {
            var types = new List<Type>();

            foreach (var assembly in assemblies)
            {
                try
                {
                    types.AddRange(assembly.GetTypes().Where(t => predicate(t)));
                }
                catch (ReflectionTypeLoadException exception)
                {
                    throw new AssemblyTypeLoadFailure(exception, assembly);
                }
                catch (Exception exception)
                {
                    throw new AssemblyLoadFailure(exception, assembly);
                }
            }
            return types;
        }

        public static Type FindType(this IEnumerable<Assembly> assemblies, Predicate<Type> predicate)
        {
            foreach (var assembly in assemblies)
            {
                try
                {
                    var type = assembly.GetTypes().Where(t => predicate(t)).FirstOrDefault();
                    if (type != null) return type;
                }
                catch (ReflectionTypeLoadException exception)
                {
                    throw new AssemblyTypeLoadFailure(exception, assembly);
                }
                catch (Exception exception)
                {
                    throw new AssemblyLoadFailure(exception, assembly);
                }
            }
            return null;
        }
    }
}
