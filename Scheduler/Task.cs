using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arya;
using Arya.Command;
using Arya.Modules;
using System.Threading;
using Arya.Storage;
using Arya.Threading;

namespace Arya.Scheduler
{
    public class Task : PulseThread
    {
        private static int NextTaskID = 0;

        public string Name { get; private set; }
        private IModule Module;
        private string[] Arguments;

        public static Task CreateTask(string[] args)
        {
            if (args.Length > 0)
            {
                string cmd = args[0];
                if (ModuleManager.CommandRegistry.ContainsKey(cmd))
                {
                    if (args.Length > 1)
                    {
                        string[] modargs = new string[args.Length - 1];
                        Array.Copy(args, 1, modargs, 0, modargs.Length);
                        return new Task(ModuleManager.CommandRegistry[cmd], modargs); // Task given module name and args.
                    }
                    return new Task(ModuleManager.CommandRegistry[cmd],new string[] {}); // Task given module name with no additional arguments.
                }
                return null; // Task not given a module name that is loaded.
            }
            else
            {
                return null; // Task not given any module command.
            }
        }

        private Task(string dllLocation, string[] args) : base("Task"+(NextTaskID++).ToString())
        {
            // Tasks always load a fresh instance of the module.
            Module = ModuleManager.LoadModule(dllLocation);
            Arguments = args;
        }

        protected override bool Pulse()
        {
            Module.Execute(Arguments);
            return false; // Do not keep executing.
        }

        protected override void OnStart()
        {
            Module.OnLoad();
        }

        protected override void OnStop()
        {
            Module.OnExit();
        }
    }
}
