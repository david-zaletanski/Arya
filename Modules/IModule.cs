using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arya.Command;

namespace Arya.Modules
{
    public interface IModule
    {
        string Command { get; }
        void OnLoad();
        void Execute(string[] args);
        void OnExit();
    }
}
