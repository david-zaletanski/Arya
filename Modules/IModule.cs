using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arya.Modules
{
    public interface IModule
    {
        string Name { get; }
        int Version { get; }

        string CommandName { get; }

        void Execute(string[] args);
    }
}
