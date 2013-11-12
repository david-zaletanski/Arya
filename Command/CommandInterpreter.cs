using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arya.Command
{
    public class CommandInterpreter
    {
        private List<string> Commands;
        private List<ExecuteDelegate> Methods;
        private DefaultCommands DefaultCommands;
        public delegate void ExecuteDelegate(string[] Args);

        public CommandInterpreter()
        {
            Commands = new List<string>();
            Methods = new List<ExecuteDelegate>();
            DefaultCommands = new DefaultCommands();
            DefaultCommands.RegisterCommands(this);
        }

        public bool AddCommand(string command, ExecuteDelegate del)
        {
            if (Commands.Contains(command))
            {
                Core.Output("CommandInterpreter - Command " + command + " already registered.");
                return false;
            }
            Commands.Add(command);
            Methods.Add(del);
            return true;
        }
        public bool RemoveCommand(string command)
        {
            int i = Commands.IndexOf(command);
            if (i < 0)
                return false;
            Commands.RemoveAt(i);
            Methods.RemoveAt(i);
            return true;
        }

        public bool InterpretCommand(string Command)
        {
            string[] Arguments = ParseCommand(Command);
            if (Arguments.Length == 0)
                return false;

            bool found = false;
            for(int i = 0; i < Commands.Count; i++)
                if (Commands[i].Equals(Arguments[0].ToLower()))
                {
                    found = true;
                    Methods[i].Invoke(Arguments); // TODO: Make asynchronus?
                }

            return found;
        }

        private string[] ParseCommand(string command)
        {
            // Do not split if inside "s
            return command.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
