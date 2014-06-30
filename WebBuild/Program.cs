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
                    using(var output = File.Create(destination))
                    {
                        transform.Transform(xDoc.Root.CreateReader(), null, output);
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
}
