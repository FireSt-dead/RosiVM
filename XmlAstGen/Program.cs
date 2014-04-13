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
            }
            while (message == ParseMessage.Reduction || message == ParseMessage.TokenRead);

            XDocument xmlDoc = new XDocument();
            XElement rootElement = new XElement("Global");
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

            if (reduction.Count() > 1 && container.Name.LocalName != head)
            {
                XElement childContainer = new XElement(head);
                childContainer.SetAttributeValue("production", production.Text(false).Replace('<', '(').Replace('>', ')'));
                container.Add(childContainer);
                container = childContainer;
            }

            string productionText = production.Text(false);
            int tailStart = productionText.IndexOf(" ::= ") + 5;
            string tail = productionText.Substring(tailStart);
            string[] tokens = tail.Split(' ');

            for (int i = 0; i < reduction.Count(); i++)
            {
                object child = reduction.get_Data(i);
                Reduction childReduction = child as Reduction;
                if (childReduction != null)
                {
                    Print(container, childReduction);
                }
                else
                {
                    string name = tokens[i];
                    if (name.StartsWith("\'") && name.EndsWith("\'"))
                    {
                        // '<token-value>'
                        XElement token = new XElement("Token");
                        token.SetValue(child.ToString());
                        container.Add(token);
                    }
                    else if (reduction.Count() == 1)
                    {
                        // <production> ::= terminal - reuse the name of the production
                        XElement token = new XElement(reduction.Parent.Head().Name().Trim('<', '>'));
                        token.SetValue(child.ToString());
                        container.Add(token);
                    }
                    else
                    {
                        try
                        {
                            XElement token = new XElement(tokens[i].Trim('\''));
                            // token.SetValue(child.ToString());
                            container.Add(token);
                        }
                        catch (Exception e)
                        {

                        }
                    }
                    //Console.WriteLine(ident + d.ToString());
                }
            }
        }
    }
}
