using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Xml;
using System.Xml.XPath;
using WcfDoc.Engine;
using System.Diagnostics;

namespace WcfDoc
{
    public class Runtime
    {
        // ────────────────────────── Public Members ──────────────────────────

        public void Run(string[] args)
        {
            Console.WriteLine("WCF Documentation Generator - {0}", Assembly.GetExecutingAssembly().GetName().Version.ToString());
            Console.WriteLine();

            Options options = new Options();
            Initialization.Options.Load(Environment.CommandLine, options);

            if (string.IsNullOrEmpty(options.Assemblies))
            {
                Console.WriteLine(" Ex: WcfDoc");
                Console.WriteLine();

                Initialization.Options.Display(new Type[] { typeof(Options) });
            }
            else
            {
                try
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    new Generator(
                        new Context(
                        options.Assemblies != null ? options.Assemblies.Split(new char[] {'|'}) : null, 
                        options.Output, 
                        options.Stylesheet,  
                        options.MergeFiles != null ? options.MergeFiles.Split(new char[] {'|'}) : null,
                        options.WebsitePath,
                        options.Config,
                        options.XmlComments != null ? options.XmlComments.Split(new char[] {'|'}) : null
                        )).Generate();

                    stopwatch.Stop();

                    Console.WriteLine("Documentation generation complete ({0}).", stopwatch.Elapsed.ToString());
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }
            }

            Console.WriteLine();
        }
    }
}
