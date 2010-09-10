using System;
using System.Reflection;
using WcfDoc.Engine;
using System.Diagnostics;

namespace WcfDoc
{
    public class Runtime
    {
        // ────────────────────────── Public Members ──────────────────────────

        public void Run(string[] args)
        {
            Console.WriteLine("WCF Documentation Generator - {0}", Assembly.GetExecutingAssembly().GetName().Version);
            Console.WriteLine();

            var options = new Options();
            Initialization.Options.Load(Environment.CommandLine, options);

            if (string.IsNullOrEmpty(options.Assemblies))
            {
                Console.WriteLine(" Ex: WcfDoc");
                Console.WriteLine();

                Initialization.Options.Display(new [] { typeof(Options) });
            }
            else
            {
                try
                {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();

                    new Generator(
                        new Context(
                        options.Assemblies.Split(new [] {'|'}), 
                        options.Output, 
                        options.Stylesheet,  
                        options.MergeFiles != null ? options.MergeFiles.Split(new [] {'|'}) : null,
                        options.WebsitePath,
                        options.Config,
                        options.XmlComments != null ? options.XmlComments.Split(new [] {'|'}) : null
                        )).Generate();

                    stopwatch.Stop();

                    Console.WriteLine("Documentation generation complete ({0}).", stopwatch.Elapsed);
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
