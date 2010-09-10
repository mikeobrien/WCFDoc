using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;

namespace WcfDoc.Engine
{
    public class ServiceWebsite : IEnumerable<ServiceWebsite.ServiceFile>
    {
        // ────────────────────────── Private Fields ──────────────────────────

        readonly List<ServiceFile> _serviceFiles = new List<ServiceFile>();

        // ────────────────────────── Constructor ──────────────────────────

        public ServiceWebsite(string path)
        {
            if (string.IsNullOrEmpty(path)) return;
            try
            {
                var files = Directory.GetFiles(path, "*.svc", SearchOption.AllDirectories);
                var regex = new Regex(@"(?:\s*)(?<name>\w*)\s*[:|=]\s*(""((?<value>.*?)"")|(?<value>[\w]*))");

                foreach (var file in files)
                {
                    var matches = regex.Matches(File.ReadAllText(file));
                    var relativePath = file.Substring(path.Length).Replace("\\", "/");
                    if (!relativePath.StartsWith("/")) relativePath = "/" + relativePath;

                    foreach (var match in matches.Cast<Match>().Where(match => match.Groups.Count >= 2 && 
                                                                               match.Groups["name"].Value == "Service"))
                    {
                        _serviceFiles.Add(new ServiceFile(relativePath, match.Groups["value"].Value));
                        break;
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception(string.Format("Error enumerating service files '{0}': {1}", path, exception.Message));
            }
        }

        // ────────────────────────── Public Memebers ──────────────────────────

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _serviceFiles.GetEnumerator();
        }

        public IEnumerator<ServiceFile> GetEnumerator()
        {
            return _serviceFiles.GetEnumerator();
        }

        // ────────────────────────── Nested Types ──────────────────────────

        public class ServiceFile
        {
            public ServiceFile(string uri, string type)
            {
                Uri = new Uri(uri, UriKind.Relative);
                Type = type;
            }

            public Uri Uri { get; private set; }
            public string Type { get; private set; }
        }
    }
}
