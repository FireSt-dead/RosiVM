using GOLD;
using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace XmlAstGen
{
    class Program
    {
        private static readonly XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
        private static readonly XNamespace parse = "http://www.rosivm.org/2014/parse/";
     
        static void Main(string[] args)
        {
            ParseSource();
            TransformToHtml();

            Compiler comp = new Compiler();
            comp.CodeGen();
        }

        private static void ParseSource()
        {
            var parser = new Parser();
            var grammarStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("XmlAstGen.RosiLang.grammar.egt");
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
            XElement rootElement = new XElement(parse + "Global");
            rootElement.SetAttributeValue(XNamespace.Xmlns + "xsi", xsi.NamespaceName);
            rootElement.SetAttributeValue(xsi + "schemaLocation", @"http://www.rosivm.org/2014/parse/ RosiLang.parse.xsd");

            xmlDoc.Add(rootElement);
            Reduction root = (Reduction)parser.CurrentReduction;
            Print(rootElement, root);
            File.WriteAllText("../../rvm.parse.xml", xmlDoc.ToString());

            // Try to actually compile the xml...
            CompileXML(xmlDoc);
        }

        private static void TransformToHtml()
        {
            var xslt = new XslCompiledTransform();
            var transformStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("XmlAstGen.rvm.parse-html.xslt");
            xslt.Load(XmlReader.Create(transformStream));
            xslt.Transform("../../rvm.parse.xml", "../../rvm.html.md");
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

        private static void CompileXML(XDocument xmlDoc)
        {
            AssemblyName name = new AssemblyName("consoleapp");
            // Create in a new assembly
            AssemblyBuilder assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Save);
            
            ModuleBuilder module = assembly.DefineDynamicModule("consoleapp.exe");
            CompileGlobal(module, xmlDoc.Root);
            module.CreateGlobalFunctions();

            //// This should be generated by the xml...
            //MethodBuilder method = module.DefineGlobalMethod("main", MethodAttributes.HideBySig | MethodAttributes.Static | MethodAttributes.Public, typeof(void), System.Type.EmptyTypes);
            //var body = method.GetILGenerator();
            //body.EmitWriteLine("Hello, world!");
            //body.Emit(OpCodes.Ret);
            //module.CreateGlobalFunctions();
            //assembly.SetEntryPoint(method, PEFileKinds.ConsoleApplication);
            
            assembly.Save("consoleapp.exe");
        }

        private static void CompileGlobal(ModuleBuilder module, XElement global)
        {
            foreach (var child in global.Elements())
            {
                Compile(module, child);
            }
        }

        private static void Compile(ModuleBuilder module, XElement element)
        {
            if (element.Name == parse + "Function")
            {
                CompileFunction(module, element);
            }
        }

        private static void CompileFunction(ModuleBuilder module, XElement function)
        {
            XElement nameElement = function.Element(parse + "Name");
            String name = nameElement.Value;
            Console.WriteLine("function " + nameElement.Value);

            MethodAttributes methodAttributes = MethodAttributes.HideBySig | MethodAttributes.Static | MethodAttributes.Public;
            Type returnType = typeof(void);
            Type[] parameterTypes = Type.EmptyTypes;

            var method = module.DefineGlobalMethod(name, methodAttributes, returnType, parameterTypes);

            // TODO: Recursively from the StatementBlock...
            var body = method.GetILGenerator();            
            body.EmitWriteLine("Hello, world!");
            body.Emit(OpCodes.Ret);
        }
    }
}
