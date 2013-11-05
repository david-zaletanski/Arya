using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;

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


    }
}
