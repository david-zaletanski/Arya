using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;

namespace Arya.Scheduler
{
    abstract class Task
    {
        public Task()
        {

        }

        abstract public void Execute(string[] args);
    }
}
