using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arya.Command
{
    public class CommandInterpreter
    {
        private List<ICommand> _Commands;

        public CommandInterpreter()
        {
            _Commands = new List<ICommand>();
            AddCommand(new DefaultCommands());
        }

        public void AddCommand(ICommand command)
        {
            _Commands.Add(command);
        }

        public bool InterpretCommand(string Command)
        {
            string[] Arguments = ParseCommand(Command);
            if (Arguments.Length == 0)
                return false;

            foreach (ICommand cmd in _Commands)
            {
                if (cmd.Commands.Contains(Arguments[0].ToLower()))
                {
                    cmd.ExecuteCommand(Arguments);
                }
            }

            return true;
        }

        private string[] ParseCommand(string command)
        {
            // Do not split if inside "s
            return command.Split(' ');
        }
    }

    public interface ICommand
    {
        List<string> Commands { get; }

        void ExecuteCommand(string[] args);
    }
}
