using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Reflection;

namespace Arya.Modules
{
    class StaticDllManager
    {
        private static int NextDomainCounter = 0;   // Used in creating unique friendly domain names.
        private static List<DllInstance> Instances = new List<DllInstance>();   // Keeps track of loaded instances.

        /// <summary>
        /// Loads an instance of a class from a DLL file.
        /// </summary>
        /// <param name="dllFile">full path to the dll file</param>
        /// <param name="typeName">type of class to instantiate</param>
        /// <returns>the loaded instance or null if there was an exception</returns>
        public static object Load(string dllFile, string typeName)
        {
            try
            {
                if (!File.Exists(dllFile))
                    return null;

                AppDomain domain = AppDomain.CreateDomain("AppDomain" + (NextDomainCounter++).ToString());
                object instance = domain.CreateInstance(dllFile, typeName).Unwrap();

                DllInstance dll = new DllInstance();
                dll.Domain = domain;
                dll.Path = dllFile;
                dll.TypeName = typeName;
                dll.Instance = instance;
                Instances.Add(dll);

                return instance;
            }
            catch (Exception ex)
            {
                Core.HandleEx(ex);
                return null;
            }
        }

        /// <summary>
        /// Returns a pre-existing, loaded, instance.
        /// </summary>
        /// <param name="dllLocation">location of dll it was loaded from</param>
        /// <returns>the first found object or null if not found</returns>
        public static object GetExistingInstance(string dllLocation, string typeName)
        {
            foreach (DllInstance dll in Instances)
                if (dll.Path == dllLocation && dll.TypeName==typeName)
                    return dll.Instance;
            return null;
        }

        /// <summary>
        /// Unloads all previously created instances and their associated domains.
        /// </summary>
        public static void Unload()
        {
            foreach (DllInstance instance in Instances)
                AppDomain.Unload(instance.Domain);
            Instances.Clear();
        }
    }

    // Holds data from a loaded assembly.
    struct DllInstance
    {
        public AppDomain Domain;
        public string TypeName;
        public string Path;
        public object Instance;
    }
}
