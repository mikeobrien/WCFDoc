using System;
using System.Runtime.Serialization;
using System.IO;
using System.Xml.Linq;
using System.Xml;

namespace WcfDoc.Engine
{
    public class DataContractTypeInfo
    {
        public DataContractTypeInfo()
        {
            Name = string.Empty;
            TypeNamespace = string.Empty;
        }

        private DataContractTypeInfo(string name, string typeNamespace)
        {
            Name = name;
            TypeNamespace = typeNamespace;
        }

        public string Name { get; private set; }
        public string TypeNamespace { get; private set; }

        public static DataContractTypeInfo Generate(Type type)
        {
            try
            {
                if (type.FullName.StartsWith("System.")) return new DataContractTypeInfo();
                if (type.ContainsGenericParameters) return new DataContractTypeInfo();
                var serializer = new DataContractSerializer(type);
                var objectStream = new MemoryStream();
                object instance = null;
                if (type.IsArray)
                    Activator.CreateInstance(type, new object[] {0});
                else
                    Activator.CreateInstance(type);
                serializer.WriteObject(objectStream, instance);
                objectStream.Position = 0;
                var objectDocument = XDocument.Load(new XmlTextReader(objectStream));
                return new DataContractTypeInfo(objectDocument.Root.Name.LocalName,
                                                    objectDocument.Root.Name.NamespaceName);
            }
            catch (Exception exception)
            {
                throw new Exception(string.Format("Unable to determine Data Contract information for type '{0}': {1}", type.FullName, exception.Message), exception);
            }
        }
    }
}
