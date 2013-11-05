using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using Arya.Command;

namespace Arya.Scheduler
{
    public class TaskScheduler : ICommand
    {
        private Timer _Timer;

        public TaskScheduler(int interval)
        {
            _Commands = new List<string>();
            _Commands.Add("ts");
            _Commands.Add("taskscheduler");
            _Timer = new Timer();
            _Timer.Interval = interval;
            _Timer.Tick += new EventHandler(_Timer_Tick);
            //_Timer.Start();
        }

        void _Timer_Tick(object sender, EventArgs e)
        {
            
        }


        private List<string> _Commands;
        public List<string> Commands
        {
            get { return _Commands; }
        }

        public void ExecuteCommand(string[] args)
        {
            // TODO: Handle commands.

        }
    }
}
