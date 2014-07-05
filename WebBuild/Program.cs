using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace WebBuild
{
    class Program
    {
        static void Main(string[] args)
        {
            // Hack and slash an html site using xslt here...
            // Foreach html in the Pages, load the page, and apply a template from the Templates to produce a page in the Output...
            var files = Directory.GetFiles(@"./Pages/", "*.html", SearchOption.AllDirectories);

            Dictionary<string, XslCompiledTransform> transformations = new Dictionary<string, XslCompiledTransform>();
            HtmlBuilderExtension extensions = new HtmlBuilderExtension();
            foreach(var file in files) {
                try
                {
                    System.Console.WriteLine("Applying templates to: " + file);
                    XDocument xDoc = XDocument.Load(file);
                    var templatePI = xDoc.Nodes().OfType<XProcessingInstruction>().FirstOrDefault(p => p.Target == "template");
                    var template = templatePI == null ? "index.xslt" : templatePI.Data;

                    XslCompiledTransform transform;
                    if (!transformations.TryGetValue(template, out transform))
                    {
                        transform = new XslCompiledTransform();
                        XsltSettings settings = new XsltSettings(true, true);
                        transform.Load(Path.Combine("Templates", template), settings, new HtmlBuilderResolver());
                    }

                    var destination = Path.Combine("Output", file.Substring(@"./Pages/".Length));
                    if (!Directory.Exists(Path.GetDirectoryName(Path.GetFullPath(destination))))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(destination)));
                    }
                    using(var output = File.Create(destination))
                    {
                        XsltArgumentList xsltArgs = new XsltArgumentList();
                        var page = file.Substring("./Pages/".Length);
                        xsltArgs.AddParam("page", "", page);
                        xsltArgs.AddExtensionObject("urn:web-build:xslt", extensions);
                        extensions.Page = page;
                        transform.Transform(xDoc.Root.CreateReader(), xsltArgs, output);
                    }
                }
                catch (Exception e)
                {
                    System.Console.WriteLine("Error: " + e.Message);
                }
            }

            // Copy non html and non xslt from pages and templates to output
            foreach(var file in Directory.GetFiles(@"./Templates", "*.*", SearchOption.AllDirectories).Where(NonSystemExtension))
            {
                var destination = Path.Combine("Output", file.Substring(@"./Templates/".Length));
                var destinationDir = Path.GetDirectoryName(Path.GetFullPath(destination));
                if (!Directory.Exists(destinationDir))
                {
                    Directory.CreateDirectory(destinationDir);
                }
                File.Copy(file, destination, true);
            }
        }

        private static bool NonSystemExtension(string file)
        {
            var ext = Path.GetExtension(file).ToLowerInvariant();
            return ext != ".html" && ext != ".xslt";
        }
    }

    internal class HtmlBuilderResolver : XmlResolver
    {
        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            return XmlReader.Create(absoluteUri.AbsolutePath);
        }
    }

    internal class HtmlBuilderExtension
    {
        public string Page { get; internal set; }

        public bool isinside(string page)
        {
            if (page != null)
            {
                var normalizedCurrentPageDirectory = NormalizedDirectory(this.Page);
                var normalizedCheckingPageDirectory = NormalizedDirectory(page);

                // Implement "is under" when subpages arrive...
                Console.WriteLine(normalizedCurrentPageDirectory + " -> " + normalizedCheckingPageDirectory);
                return normalizedCurrentPageDirectory == normalizedCheckingPageDirectory;
            }

            return false;
        }

        public bool isselected(string page)
        {
            if (page != null)
            {
                var normalizedCurrentPage = NormalizedPath(this.Page);
                var normalizedCheckingPage = NormalizedPath(page);
                return normalizedCurrentPage == normalizedCheckingPage;
            }

            return false;
        }

        private static string NormalizedPath(string path)
        {
            return path.Replace(@"\", @"/").Trim('/');
        }

        private static string NormalizedDirectory(string path)
        {
            path = NormalizedPath(path);
            var lastSlashIndex = path.LastIndexOf('/');
            if (lastSlashIndex >= 0)
            {
                path = path.Substring(0, lastSlashIndex);
            }
            else
            {
                path = string.Empty;
            }
            return path;
        }
    }
}
