using GOLD;
using System;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace RosiCompiler
{
    public class PushParser
    {
        public static readonly XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
        public static readonly XNamespace parse = "http://www.rosivm.org/2014/parse/";

        private string code;
        private Parser parser;

        public PushParser()
        {
            this.parser = new Parser();
            var grammarStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("RosiCompiler.Grammar.egt");
            var binaryReader = new System.IO.BinaryReader(grammarStream);
            this.parser.LoadTables(binaryReader);
        }

        public string Code
        {
            get
            {
                return this.code;
            }

            set
            {
                this.code = value;
                this.Compile();
            }
        }

        public XDocument Tree { get; private set; }

        private void Compile()
        {
            parser.Open(ref this.code);

            ParseMessage message;
            do
            {
                // TODO: Error handling, reporting, and continue on error...
                message = parser.Parse();
            }
            while (message == ParseMessage.Reduction || message == ParseMessage.TokenRead);

            this.Tree = new XDocument();
            XElement rootElement = new XElement(parse + "Global");
            rootElement.SetAttributeValue(XNamespace.Xmlns + "xsi", xsi.NamespaceName);
            rootElement.SetAttributeValue(xsi + "schemaLocation", @"http://www.rosivm.org/2014/parse/ RosiLang.parse.xsd");
            this.Tree.Add(rootElement);

            Reduction root = parser.CurrentReduction as Reduction;
            if (root != null)
            {
                Print(rootElement, root);
                File.WriteAllText("../../rvm.parse.xml", this.Tree.ToString());
            }
        }

        private static void Print(XElement container, Reduction reduction)
        {
            var production = reduction.Parent;
            string head = production.Head().Name();

            if (reduction.Count() > 1 && container.Name.LocalName != head && !head.EndsWith("Members"))
            {
                string childContainerName;
                if (head.StartsWith(container.Name.LocalName))
                {
                    childContainerName = head.Substring(container.Name.LocalName.Length);
                }
                else
                {
                    childContainerName = head;
                }
                XElement childContainer = new XElement(parse + childContainerName);
                childContainer.SetAttributeValue("prod", production.Text(false).Replace('<', '(').Replace('>', ')'));
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
                        XElement tokenXml = new XElement(parse + reduction.Parent.Head().Name().Trim('<', '>'));
                        tokenXml.SetValue(childToken.Data.ToString());
                        tokenXml.SetAttributeValue("line", childToken.Position().Line);
                        tokenXml.SetAttributeValue("column", childToken.Position().Column);
                        container.Add(tokenXml);
                    }
                    else
                    {
                        XElement tokenXml = new XElement(parse + tokens[i].Trim('\''));
                        tokenXml.SetValue(childToken.Data.ToString());
                        tokenXml.SetAttributeValue("line", childToken.Position().Line);
                        tokenXml.SetAttributeValue("column", childToken.Position().Column);
                        container.Add(tokenXml);
                    }
                }
            }
        }
    }
}
