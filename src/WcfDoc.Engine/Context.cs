using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Xml.Linq;

namespace WcfDoc.Engine
{
    public class Context
    {
        // ────────────────────────── Constructor ──────────────────────────

        public Context(
            string[] assemblies, 
            string output, 
            string stylesheet,  
            string[] mergeFiles,
            string websitePath,
            string config,
            string[] xmlComments)
        {
            OutputPath = output;
            Assemblies = LoadAssemblies(assemblies);
            Stylesheet = LoadXmlFile(stylesheet);
            MergeDocuments = LoadXmlFiles(mergeFiles);
            ServiceWebsite = new ServiceWebsite(websitePath);
            Config = LoadXmlFile(config);
            XmlComments = LoadXmlFiles(xmlComments);
        }

        // ────────────────────────── Public Properties ──────────────────────────

        public IEnumerable<Assembly> Assemblies { get; private set; }
        public string OutputPath { get; private set; }
        public XDocument Stylesheet { get; private set; }
        public IEnumerable<XDocument> MergeDocuments { get; private set; }
        public XDocument Config { get; private set; }
        public IEnumerable<XDocument> XmlComments { get; private set; }
        public ServiceWebsite ServiceWebsite { get; private set; }

        // ────────────────────────── Private Members ──────────────────────────

        private IEnumerable<Assembly> LoadAssemblies(string[] paths)
        {
            List<Assembly> assemblies = new List<Assembly>();

            foreach (string path in paths)
            {
                try
                {
                    assemblies.Add(Assembly.LoadFrom(path));
                }
                catch (Exception exception)
                {
                    throw new Exception(string.Format("Error loading assembly '{0}': {1}", path, exception.Message));
                }
            }
            return assemblies;
        }

        private IEnumerable<XDocument> LoadXmlFiles(string[] paths)
        {
            if (paths == null || paths.Length < 1) return null;
            List<string> files = new List<string>();

            foreach (string path in paths)
            {
                if (!string.IsNullOrEmpty(path))
                {
                    if (!Directory.Exists(Path.GetDirectoryName(path)))
                        throw new Exception(string.Format("Xml files folder '{0}' does not exist!", path));

                    try
                    {
                        files.AddRange(Directory.GetFiles(Path.GetDirectoryName(path), Path.GetFileName(path)));
                    }
                    catch (Exception exception)
                    {
                        throw new Exception(string.Format("Error loading xml files '{0}': {1}", path, exception.Message));
                    }
                }
            }

            if (files == null || files.Count < 1)
                throw new Exception(string.Format("No xml file(s) found at '{0}'!", paths));

            return new List<XDocument>(from path in files select LoadXmlFile(path));
        }

        private XDocument LoadXmlFile(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;
            if (!File.Exists(path))
                throw new Exception(string.Format("Xml file '{0}' does not exist!", path));
            else
            {
                try
                {
                    return XDocument.Load(path);
                }
                catch (Exception exception)
                {
                    throw new Exception(string.Format("Error loading xml file '{0}': {1}", path, exception.Message));
                }
            }
        }
    }
}
