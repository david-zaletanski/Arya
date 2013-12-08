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
        const string ModuleInterfaceName = "Arya.Modules.IModule";

        /// <summary>
        /// When modules are loaded their command is placed as a key in this dictionary, and the
        /// full path to the .DLL file is the value. This is to assist with loaded module identification.
        /// </summary>
        public static Dictionary<string, string> CommandRegistry = new Dictionary<string, string>();

        /// <summary>
        /// Loads an IModule from a .DLL file.
        /// </summary>
        /// <param name="dllName">full path to the .DLL file</param>
        /// <returns>IModule if loaded successfully, null if not</returns>
        public static IModule LoadModule(string dllName)
        {
            IModule result = (IModule)StaticDllManager.Load(dllName, ModuleInterfaceName);

            // Registers the modules command with its .DLL file path.
            if(result!=null) {
                if (CommandRegistry.ContainsKey(result.Command))
                {
                    Core.HandleEx(new Exception("LoadModule could not load '" + dllName + "' because a module with the command '" + result.Command + "' has already been loaded."));
                }
                else
                {
                    CommandRegistry.Add(result.Command, dllName);
                }
            }
            return result;
        }

        /// <summary>
        /// Attempts to load all of the .DLL files in the given directory as IModules.
        /// </summary>
        /// <param name="directory">directory to search for .DLL files</param>
        /// <param name="recursive">true to check subfolders, false to not</param>
        /// <returns>a list of loaded IModules</returns>
        public static List<IModule> LoadModules(string directory, bool recursive)
        {
            if(!Directory.Exists(directory))
                return new List<IModule>();

            List<IModule> modules = new List<IModule>();
            foreach (string file in Directory.GetFiles(directory))
            {
                FileInfo finfo = new FileInfo(file);
                if (finfo.Extension.ToUpper() == ".DLL")
                {
                    IModule m = LoadModule(finfo.FullName);
                    if (m != null)
                        modules.Add(m);
                    else
                        Core.Output("Error loading module '" + finfo.FullName + "'");
                }
            }

            if (recursive)
            {
                foreach (string subdirectory in Directory.GetDirectories(directory))
                    modules.AddRange(LoadModules(subdirectory, true));
            }

            return modules;
        }

        /// <summary>
        /// Searches for a loaded instance of the module with the given the full path of .DLL
        /// </summary>
        /// <param name="dllName">full path of dll file</param>
        /// <returns>the found instance or null if not found</returns>
        public static IModule GetExistingModule(string dllName)
        {
            return (IModule)StaticDllManager.GetExistingInstance(dllName, ModuleInterfaceName);
        }

        /// <summary>
        /// Unloads all modules from address space and clears the command registry.
        /// </summary>
        public static void UnloadModules()
        {
            StaticDllManager.Unload();
            CommandRegistry.Clear();
        }
    }
}
