using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Arya
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Core.OnApplicationLaunch();
            Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
            Application.Run(Core._CLIForm);
        }

        static void Application_ApplicationExit(object sender, EventArgs e)
        {
            Core.OnApplicationExit();
        }
    }
}
