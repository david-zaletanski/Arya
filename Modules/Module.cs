using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.CSharp;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;

namespace Arya.Modules
{
    class Module
    {
        public string DllLocation { get; private set; }
        public IModule ModuleInterface { get; private set; }

        public Module(string DllLocation, IModule Module)
        {
            if (!File.Exists(DllLocation))
                throw new FileNotFoundException("Invalid .DLL specified!", DllLocation);

            this.DllLocation = DllLocation;
            this.ModuleInterface = Module;
        }

        public void Execute(string[] args)
        {
            ModuleInterface.Execute(args);
        }

        #region Runtime Script Compiling
        /// <summary>
        /// Compiles the C# script file at runtime. Returns the filename of the .dll assembly.
        /// </summary>
        /// <param name="ScriptLocation">Path/name of C# script</param>
        /// <returns>Path/name of the compiled assembly. Returns the error if failed.</returns>
        private string CompileScript(string ScriptLocation)
        {
            FileInfo Script = new FileInfo(ScriptLocation);
            if (!Script.Exists)
                return "Error: File doesn't exist! "+ScriptLocation;
            string ScriptText = File.ReadAllText(ScriptLocation);

            string DllLocation = Script.DirectoryName + Script.Name.Substring(0, Script.Name.Length - Script.Extension.Length) + ".dll";

            CSharpCodeProvider csp = new CSharpCodeProvider();
            ICodeCompiler cc = csp.CreateCompiler();
            string[] Assemblies = GetAssemblies();
            CompilerParameters cp = new CompilerParameters();
            cp.OutputAssembly = DllLocation;
            foreach (string s in Assemblies)
                cp.ReferencedAssemblies.Add(s);
            cp.WarningLevel = 3;
            cp.CompilerOptions = "/target:library /optimize";
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = false;

            TempFileCollection tfc = new TempFileCollection(Script.DirectoryName, false);
            CompilerResults cr = new CompilerResults(tfc);
            cr = cc.CompileAssemblyFromSource(cp, ScriptText);
            System.Collections.Specialized.StringCollection sc = cr.Output;
            foreach (string s in sc)
                Core.Output(s);

            if (cr.Errors.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (CompilerError ce in cr.Errors)
                    sb.AppendLine(ce.ErrorNumber + " " + ce.ErrorText);
                return sb.ToString();
            }

            return DllLocation;
        }

        private string[] GetAssemblies()
        {
            // Todo: Load other assemblies.
            return new string[] { "System.dll", "System.Data.dll", "System.Xml.dll", "mscorlib.dll", "System.Windows.Forms.dll", "Arya.exe" };
        }
        #endregion
    }
}
