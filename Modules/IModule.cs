using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arya.Modules
{
    public interface IModule
    {
        string Name { get; set; }
        int Version { get; set; }

        void Execute(string[] args);
    }
}
