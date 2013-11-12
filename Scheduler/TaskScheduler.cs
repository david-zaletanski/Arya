using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using Arya.Command;

namespace Arya.Scheduler
{
    public class TaskScheduler
    {
        private Timer _Timer;

        public TaskScheduler(int interval)
        {
            _Timer = new Timer();
            _Timer.Interval = interval;
            _Timer.Tick += new EventHandler(_Timer_Tick);
            //_Timer.Start();
        }

        void _Timer_Tick(object sender, EventArgs e)
        {
            
        }


        private string[] Commands { get { return new string[] { "ts", "taskscheduler" }; } }
        public void RegisterCommands(CommandInterpreter Interpreter)
        {
            CommandInterpreter.ExecuteDelegate del = new CommandInterpreter.ExecuteDelegate(Execute);
            foreach (string cmd in Commands)
                Interpreter.AddCommand(cmd, del);
        }
        public void Execute(string[] args)
        {
            // TODO: Handle commands.
        }
    }
}
