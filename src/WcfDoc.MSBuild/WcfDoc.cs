using System;
using Microsoft.Build.Framework;
using WcfDoc.Engine;

namespace WcfDoc.MSBuild
{
    public class WcfDoc : Microsoft.Build.Utilities.Task
    {
        // ────────────────────────── Basic Parameters ──────────────────────────

        [Required]
        public string Assemblies { get; set; }

        [Required]
        public string Output { get; set; }

        public string Stylesheet { get; set; }
        public string MergeFiles { get; set; }
        public string WebsitePath { get; set; }
        public string Config { get; set; }
        public string XmlComments { get; set; }
        public string ServiceType { get; set; }

        // ────────────────────────── Overrided Members ──────────────────────────

        public override bool Execute()
        {
            try
            {
                new Generator(
                    new Context(
                    Assemblies != null ? Assemblies.Split(new[] {'|'}) : null, 
                    Output, 
                    Stylesheet,  
                    MergeFiles != null ? MergeFiles.Split(new[] {'|'}) : null,
                    WebsitePath,
                    Config,
                    XmlComments != null ? XmlComments.Split(new[] {'|'}) : null
                    )).Generate((ServiceType)Enum.Parse(typeof(ServiceType), ServiceType));
                return true;
            } catch (Exception exception)
            {
                Log.LogErrorFromException(exception);
                return false;
            }
        }
    }
}
