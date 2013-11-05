using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Reflection;
using Arya.Command;

namespace Arya.Modules
{
    public class ModuleManager : ICommand
    {
        public List<IModule> Loaded { get; set; }

        public ModuleManager(string filepath)
        {
            _Commands = new List<string>();
            _Commands.Add("mm");
            _Commands.Add("modulemanager");

            if (!Directory.Exists(filepath))
            {
                Directory.CreateDirectory(filepath);
                Core.Output("Creating Module directory.");
                //Core.Output("ModuleManager: Directory does not exist\n"+filepath+);
                return;
            }

            Loaded = LoadModules(filepath);
        }

        public void Reload(string filepath)
        {
            // TODO: Unload old modules from memory.
            Loaded = LoadModules(filepath);
        }

        #region Load Modules

        private List<IModule> LoadModules(string filepath)
        {
            List<IModule> Modules = new List<IModule>();
            foreach (string file in Directory.GetFiles(filepath))
                Modules.AddRange(LoadModulesFrom(file));
            return Modules;
        }

        const string ModuleInterfaceName = "Arya.IModule";
        private List<IModule> LoadModulesFrom(string dllLocation)
        {
            // Note: To allow unloading of module after execution, need to create a new AppDomain.
            if (!File.Exists(dllLocation))
                return null;

            List<IModule> Modules = new List<IModule>();
            Assembly ASM = Assembly.LoadFrom(dllLocation);
            foreach (Type T in ASM.GetExportedTypes())
            {
                Type Interface = T.GetInterface(ModuleInterfaceName);
                if (Interface != null && (T.Attributes & TypeAttributes.Abstract) != TypeAttributes.Abstract)
                    Modules.Add((IModule)Activator.CreateInstance(T));
            }

            return Modules;
        }

        #endregion

        private List<string> _Commands;
        public List<string> Commands
        {
            get { return _Commands; }
        }

        public void ExecuteCommand(string[] args)
        {
            // TODO: Handle commands.
            if (args.Length >= 2)
            {
                switch (args[1].ToLower())
                {
                    case "lm":
                        // TODO: List modules.
                        break;
                    case "rm":
                        // TODO: Reload modules.
                        break;
                }
            }
        }
    }
}
