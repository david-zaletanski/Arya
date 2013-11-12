using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arya.Command
{
    class DefaultCommands
    {
        private string[] Commands { get { return new string[] { "?", "help", "cmds", "commands", "exit" }; } }
        public void RegisterCommands(CommandInterpreter Interpreter)
        {
            CommandInterpreter.ExecuteDelegate del = new CommandInterpreter.ExecuteDelegate(Execute);
            foreach (string cmd in Commands)
                Interpreter.AddCommand(cmd, del);
        }

        public void Execute(string[] args)
        {
            if (args.Length == 1)
            {
                switch (args[0].ToLower())
                {
                    case "exit":
                        System.Windows.Forms.Application.Exit();
                        break;
                    default:
                        Core.Output(getUsage());
                        break;
                }
            }
            else if (args.Length >= 2)
            {
                switch (args[1].ToLower())
                {
                    case "mm":
                        outputMM();
                        break;
                    case "modulemanager":
                        outputMM();
                        break;
                    case "settings":
                        outputSettings();
                        break;
                }
            }
        }

        private void outputMM()
        {
            Core.Output("ModuleManager Commands\n");
            Core.Output("lm    List Modules");
            Core.Output("rm    Reload Modules");
        }

        private void outputSettings()
        {
            Core.Output("Settings Commands\n");
            Core.Output("print    Outputs a list of settings.");
            Core.Output("get <name>    Outputs the value of a setting.");
            Core.Output("set <name> <value>    Sets the value of a setting.");
        }

        private string getUsage()
        {
            return "Usage: ? <arg1>\n\n<arg1> = mm,modulemanager,settings";
        }
    }
}
