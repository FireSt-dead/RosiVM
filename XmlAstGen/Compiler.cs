using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace XmlAstGen
{
    public class Compiler
    {
        ILGenerator il = null;
        Dictionary<string, LocalBuilder> symbolTable;

        public void CodeGen()
        {
            AssemblyName name = new AssemblyName("helloworld");
            AssemblyBuilder assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Save);

            ModuleBuilder module = assembly.DefineDynamicModule("helloworld.exe");
            TypeBuilder programClass = module.DefineType("Program");
            MethodBuilder main = programClass.DefineMethod("Main", MethodAttributes.HideBySig | MethodAttributes.Static | MethodAttributes.Public, typeof(void), System.Type.EmptyTypes);

            var body = main.GetILGenerator();

            body.EmitWriteLine("Hello, world!");
            body.Emit(OpCodes.Ret);

            programClass.CreateType();
            module.CreateGlobalFunctions();
            assembly.SetEntryPoint(main, PEFileKinds.ConsoleApplication);
            assembly.Save("helloworld.exe");

            // CodeGenerator
            this.il = main.GetILGenerator();
            this.symbolTable = new Dictionary<string, LocalBuilder>();
        }
    }
}
