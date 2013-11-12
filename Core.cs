using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
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
        private static StringBuilder _ExceptionLog;

        #region Command Processing

        public static void RunCommand(string command)
        {
            _Interpreter.InterpretCommand(command);
        }

        #endregion

        #region Output

        private static StringBuilder PreLoadOutput;
        public static void Output(string txt)
        {
            if (_CLIForm == null)
                PreLoadOutput.AppendLine(txt);
            else
                _CLIForm.Output(txt);
        }

        #endregion

        #region Exception Handling

        public static void HandleEx(Exception ex)
        {
            string Log = "Exception:\n" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace;
            Output(Log);
            LogException(ex);
        }
        public static string GetExceptionLog()
        {
            return _ExceptionLog.ToString();
        }
        private static void LogException(Exception ex)
        {
            string ExtLog = "Exception ("+DateTime.Now.ToString()+")\n"+ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace;
            _ExceptionLog.AppendLine(ExtLog);
        }

        #endregion

        #region Events

        public static void OnApplicationLaunch(string StartupPath)
        {
            // Initialize this classes components.
            PreLoadOutput = new StringBuilder();
            _PressedKeys = new List<Keys>();
            _ExceptionLog = new StringBuilder();
            _LLKeyboardHook = new LowLevelKeyboardHook();
            // Initialize other components.
            _Settings = new Settings(StartupPath);
            _Settings.LoadSettings(StartupPath + "\\Settings.xml");
            _CLIForm = new frmCLI();
        }

        public static void OnMainFormLoad()
        {
            // Give a hello message.
            if(PreLoadOutput.Length>0)
                Output(PreLoadOutput.ToString());
            Output("Arya online.");
            _Interpreter = new CommandInterpreter();
            _Settings.RegisterCommands(_Interpreter);
            _ModuleManager = new ModuleManager(_Settings.ModulePath);
            _ModuleManager.RegisterCommands(_Interpreter);
            _Scheduler = new TaskScheduler(_Settings.SchedulerInterval);
            _Scheduler.RegisterCommands(_Interpreter);

            // Don't want to handle keyboard input until everything else is intialized.
            _LLKeyboardHook.OnKeyDown += new LowLevelKeyboardHook.KeyPressEvent(_LLKeyboardHook_OnKeyDown);
            _LLKeyboardHook.OnKeyUp += new LowLevelKeyboardHook.KeyPressEvent(_LLKeyboardHook_OnKeyUp);
        }

        public static void OnApplicationExit()
        {
            _Settings.SaveSettings(_Settings.StartupPath + "\\Settings.xml");
            // Break all connections, save and exit.
            _CLIForm.OnExit();
        }

        #endregion

        #region Key Hook Events
        private static volatile List<Keys> _PressedKeys;
        private static void _LLKeyboardHook_OnKeyDown(System.Windows.Forms.Keys key, bool syskey)
        {
            if (!_PressedKeys.Contains(key))
                _PressedKeys.Add(key);
            ProcessKeys();
        }
        private static void _LLKeyboardHook_OnKeyUp(System.Windows.Forms.Keys key, bool syskey)
        {
            if (_PressedKeys.Contains(key))
                _PressedKeys.Remove(key);
            ProcessKeys();
        }
        private static void ProcessKeys()
        {
            /* Strange behavior:
             * -Combinations of certain keys (3 or more) do not register a single keypress.
             * -After pressing the alt key, the next keypress will not be consumed, although the keypress
             * registers.
             */
            // Toggle CLI
            if (_PressedKeys.Contains(Keys.Shift)
                && _PressedKeys.Contains(Keys.Alt)
                && _PressedKeys.Contains(Keys.Control))
            {
                if (!Core._CLIForm.Visible)
                    Core._CLIForm.Show();
                else
                    Core._CLIForm.Hide();
            }
        }

        #endregion
    }
}
