using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arya.Command;

namespace Arya.Modules
{
    public interface IModule
    {
        string Name { get; }
        int Version { get; }
        void RegisterCommands(CommandInterpreter Interpreter);
        void UnregisterCommands(CommandInterpreter Interpreter);
        void Execute(string[] args);
    }
}
