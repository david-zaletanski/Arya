using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Reflection;
using Arya.Command;

namespace Arya.Modules
{
    public class ModuleManager
    {
        public List<IModule> LoadedModules { get; set; }

        public ModuleManager(string filepath)
        {
            if (!Directory.Exists(filepath))
            {
                Directory.CreateDirectory(filepath);
                Core.Output("Creating Module directory:\n"+filepath);
                return;
            }

            Reload(filepath);
        }

        public void Reload(string filepath)
        {
            // TODO: Unload old modules from memory.
            LoadedModules = LoadModules(filepath);
        }

        #region Load Modules

        private List<IModule> LoadModules(string filepath)
        {
            UnloadModules();
            List<IModule> Modules = new List<IModule>();
            foreach (string file in Directory.GetFiles(filepath))
                Modules.AddRange(LoadModulesFrom(file));
            return Modules;
        }

        const string ModuleInterfaceName = "Arya.Modules.IModule";
        private List<IModule> LoadModulesFrom(string dllLocation)
        {
            List<IModule> rval = new List<IModule>();

            // Note: To allow unloading of module after execution, need to create a new AppDomain.
            FileInfo fInfo = new FileInfo(dllLocation);
            if (!fInfo.Exists || fInfo.Extension.ToLower() != ".dll")
                return rval;

            try
            {
                List<IModule> Modules = new List<IModule>();
                Assembly ASM = Assembly.LoadFrom(dllLocation);
                foreach (Type T in ASM.GetExportedTypes())
                {
                    Type Interface = T.GetInterface(ModuleInterfaceName);
                    if (Interface != null && (T.Attributes & TypeAttributes.Abstract) != TypeAttributes.Abstract)
                    {
                        IModule module = (IModule)Activator.CreateInstance(T);
                        module.RegisterCommands(Core._Interpreter);
                        rval.Add(module);
                    }
                }
            }
            catch (Exception ex)
            {
                Core.HandleEx(ex);
            }

            return rval;
        }

        public void UnloadModules()
        {
            if (LoadedModules == null)
                return;

            foreach (IModule module in LoadedModules)
                module.UnregisterCommands(Core._Interpreter);

            // TODO: Remove module from memory.
            LoadedModules.Clear();
        }

        #endregion

        public void PrintModules()
        {
            Core.Output("ModuleManager-Loaded modules:");
            foreach (IModule module in LoadedModules)
                Core.Output(module.Name);
        }

        private string[] Commands { get { return new string[] { "mm", "modulemanager" }; } }
        public void RegisterCommands(CommandInterpreter Interpreter)
        {
            CommandInterpreter.ExecuteDelegate del = new CommandInterpreter.ExecuteDelegate(Execute);
            foreach (string s in Commands)
                Interpreter.AddCommand(s, del);
        }
        public void Execute(string[] args)
        {
            if (args.Length == 1)
            {
                Core.Output("ModuleManager Commands\n");
                Core.Output("lm    List Modules");
                Core.Output("rm    Reload Modules");
            }
            if (args.Length >= 2)
            {
                switch (args[1].ToLower())
                {
                    case "lm":
                        PrintModules(); // Print loaded modules.
                        break;
                    case "rm":
                        if (args.Length == 3)
                            Reload(args[2]); // Custom path.
                        else
                            Reload(Core._Settings.ModulePath); // Default path.
                        break;
                }
            }
        }
    }
}
