using GOLD;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace XmlAstGen
{
    class Program
    {
        private static readonly XNamespace grammar = "http://www.rosivm.org/2014/grammar/";
        private static readonly XNamespace ast = "http://www.rosivm.org/2014/ast/";
        private static readonly XNamespace source = "http://www.rosivm.org/2014/source/";

        static void Main(string[] args)
        {
            var parser = new Parser();
            var grammarStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("XmlAstGen.RosiLang-Grammar.egt");
            var binaryReader = new System.IO.BinaryReader(grammarStream);
            parser.LoadTables(binaryReader);

            var testStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("XmlAstGen.TestCase.02.src.txt");
            var testReader = new StreamReader(testStream);
            parser.Open(testReader);

            ParseMessage message;
            do
            {
                // TODO: Error handling, reporting, and continue on error...
                message = parser.Parse();

                // parser.CurrentReduction
            }
            while (message == ParseMessage.Reduction || message == ParseMessage.TokenRead);

            XDocument xmlDoc = new XDocument();
            XElement rootElement = new XElement(ast + "Global");
            rootElement.SetAttributeValue(XNamespace.Xmlns + "grm", grammar.NamespaceName);
            rootElement.SetAttributeValue(XNamespace.Xmlns + "src", source.NamespaceName);
            
            xmlDoc.Add(rootElement);
            Reduction root = (Reduction)parser.CurrentReduction;
            Print(rootElement, root);
            Console.WriteLine(xmlDoc.ToString());
            File.WriteAllText("../../rvm.ast.xml", xmlDoc.ToString());
            Console.ReadKey();
        }

        private static void Print(XElement container, Reduction reduction)
        {
            var production = reduction.Parent;
            string head = production.Head().Name();

            if (reduction.Count() > 1 && container.Name.LocalName != head && !head.EndsWith("Members"))
            {
                XElement childContainer = new XElement(ast + head);
                childContainer.SetAttributeValue(grammar + "prod", production.Text(false).Replace('<', '(').Replace('>', ')'));
                container.Add(childContainer);
                container = childContainer;
            }

            string productionText = production.Text(false);
            int tailStart = productionText.IndexOf(" ::= ") + 5;
            string tail = productionText.Substring(tailStart);
            string[] tokens = tail.Split(' ');

            for (int i = 0; i < reduction.Count(); i++)
            {
                Token childToken = reduction[i];
                Reduction childReduction = childToken.Data as Reduction;
                if (childReduction != null)
                {
                    Print(container, childReduction);
                }
                else
                {
                    string name = tokens[i];
                    if (name.StartsWith("\'") && name.EndsWith("\'"))
                    {
                        throw new NotImplementedException("Unexpected token " + name + ". Wrap it in production.");
                    }
                    else if (reduction.Count() == 1)
                    {
                        // <production> ::= terminal - reuse the name of the production
                        XElement tokenXml = new XElement(ast + reduction.Parent.Head().Name().Trim('<', '>'));
                        tokenXml.SetValue(childToken.Data.ToString());
                        tokenXml.SetAttributeValue(source + "line", childToken.Position().Line);
                        tokenXml.SetAttributeValue(source + "column", childToken.Position().Column);
                        container.Add(tokenXml);
                    }
                    else
                    {
                        XElement tokenXml = new XElement(grammar + tokens[i].Trim('\''));
                        tokenXml.SetAttributeValue(source + "line", childToken.Position().Line);
                        tokenXml.SetAttributeValue(source + "column", childToken.Position().Column);
                        container.Add(tokenXml);
                    }
                }
            }
        }
    }
}
