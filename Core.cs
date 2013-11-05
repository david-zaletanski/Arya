using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arya.Scheduler;
using Arya.Modules;
using Arya.Interface;
using Arya.Command;

namespace Arya
{
    public class Core
    {
        public static Settings _Settings;
        public static frmCLI _CLIForm;
        public static TaskScheduler _Scheduler;
        public static ModuleManager _ModuleManager;
        public static CommandInterpreter _Interpreter;

        private static LowLevelKeyboardHook _LLKeyboardHook;

        #region Command Processing

        public static void RunCommand(string command)
        {
            _Interpreter.InterpretCommand(command);
        }

        #endregion

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

        public static void OnApplicationLaunch(string StartupPath)
        {
            // Load all components.
            _Settings = new Settings(StartupPath);
            _LLKeyboardHook = new LowLevelKeyboardHook();
            _CLIForm = new frmCLI();
        }

        public static void OnMainFormLoad()
        {
            // Give a hello message.
            Output("Arya online.");
            _Interpreter = new CommandInterpreter();
            _Interpreter.AddCommand(_Settings);
            _ModuleManager = new ModuleManager(_Settings.ModulePath);
            _Interpreter.AddCommand(_ModuleManager);
            _Scheduler = new TaskScheduler(_Settings.SchedulerInterval);
            _Interpreter.AddCommand(_Scheduler);

            // Don't want to handle keyboard input until everything else is intialized.
            _LLKeyboardHook.OnKeyDown += new LowLevelKeyboardHook.KeyPressEvent(_LLKeyboardHook_OnKeyDown);
            _LLKeyboardHook.OnKeyUp += new LowLevelKeyboardHook.KeyPressEvent(_LLKeyboardHook_OnKeyUp);
        }

        public static void OnApplicationExit()
        {
            // Break all connections, save and exit.
            _CLIForm.OnExit();
        }

        #endregion

        #region Key Hook Events

        private static void _LLKeyboardHook_OnKeyDown(System.Windows.Forms.Keys key, bool syskey)
        {
            // TODO: Keep track of keys pressed in array.
            // TOOD: Process hot keys.
        }
        private static void _LLKeyboardHook_OnKeyUp(System.Windows.Forms.Keys key, bool syskey)
        {
            // TODO: Keep track of keys pressed in array.
            // TOOD: Process hot keys.
        }

        #endregion
    }
}
