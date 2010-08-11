using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WcfDoc.Initialization;

namespace WcfDoc
{
    [OptionGroup("Generator Options", "Options for generating documentation")]
    public class Options
    {
        [Option("Assemblies", "Pipe seperated paths to the assemblies containg the WCF Service and Data Contract(s).")]
        public string Assemblies { get; set; }

        [Option("Output", "Path of the output file.")]
        public string Output { get; set; }

        [Option("Stylesheet", "Optional: Path to the xml stylesheet.")]
        public string Stylesheet { get; set; }

        [Option("WebsitePath", "Optional: Path to the root of a service website.")]
        public string WebsitePath { get; set; }

        [Option("Config", "Optional: Path to the service web/app.config containing service configuration.")]
        public string Config { get; set; }

        [Option("XmlComments", "Optional: Pipe seperated paths to the xml comments files to include.")]
        public string XmlComments { get; set; }

        [Option("MergeFiles", "Optional: Pipe seperated paths to xml files to be merged with the xml documentation prior to transformation. These can be explict or wildcard paths (IE: [Path]\\*.xml).")]
        public string MergeFiles { get; set; }
    }
}
