using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arya.Scheduler;
using Arya.Modules;

namespace Arya
{
    public class Core
    {
        public static frmCLI _CLIForm;
        public static TaskScheduler _Scheduler;
        public static ModuleManager _ModuleManager;

        #region Output

        public static void Output(string txt)
        {
            _CLIForm.Output(txt);
        }

        #endregion

        #region Exception Handling

        public static void HandleEx(Exception ex)
        {
            Output("Exception:\n" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
        }

        #endregion

        #region Events

        public static void OnApplicationLaunch()
        {
            // Load all components.
            Settings.LoadDefaultSettings();

            _ModuleManager = new ModuleManager();
            _Scheduler = new TaskScheduler();
            _CLIForm = new frmCLI();
        }

        public static void OnMainFormLoad()
        {
            // Give a hello message.
            Output("Arya online.");
        }

        public static void OnApplicationExit()
        {
            // Break all connections, save and exit.
            _CLIForm.OnExit();
        }

        #endregion
    }
}
