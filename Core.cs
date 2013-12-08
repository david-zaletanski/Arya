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
        public static Settings Settings;
        public static frmCLI Interface;
        public static TaskScheduler Scheduler;

        private static LowLevelKeyboardHook llKeyboardHook;
        private static StringBuilder exceptionLog;

        #region Command Processing

        public static void RunCommand(string command)
        {
            string[] args = CommandInterpreter.ParseCommand(command);
            if (args.Length > 0)
            {
                switch (args[0].ToLower())
                {
                    case "exit":
                        Application.Exit();
                        break;
                    case "mmrl":
                        ModuleManager.UnloadModules();
                        ModuleManager.LoadModules(Settings.ModulePath, false);
                        break;
                    case "settings":
                        string[] setargs = new string[args.Length - 1];
                        Array.Copy(args, 1, setargs, 0, setargs.Length);
                        Settings.Execute(setargs);
                        break;
                    case "task":
                        // Will create a task on a separate thread to run the input following "task"
                        // as a module command.
                        string[] modargs = new string[args.Length - 1];
                        Array.Copy(args, 1, modargs, 0, modargs.Length);
                        Scheduler.AddTask(modargs);
                        break;
                    default:
                        // Default command line action is to interpret the input as a module command
                        // and execute the module on the same thread. Modules must already be loaded to be received.
                        // Will execute all modules registered with that command in the order of loading.
                        if (args.Length > 0)
                        {
                            string cmd = args[0];
                            // See if the second argument is a module command.
                            if (ModuleManager.CommandRegistry.ContainsKey(cmd))
                            {
                                IModule mod = ModuleManager.GetExistingModule(ModuleManager.CommandRegistry[cmd]);
                                mod.OnLoad();
                                if (args.Length > 1)
                                {
                                    string[] extraargs = new string[args.Length - 1];
                                    Array.Copy(args, 1, extraargs, 0, extraargs.Length);
                                    mod.Execute(extraargs);
                                }
                                else
                                {
                                    mod.Execute(new string[] { });
                                }
                                mod.OnExit();
                            }
                            else
                            {
                                Core.Output("Command not recognized. No module loaded with registered command '" + cmd + "'.");
                            }
                        }
                        break;
                }
            }
        }

        #endregion

        #region Output

        private static StringBuilder PreLoadOutput;
        public static void Output(string txt)
        {
            if (Interface == null)
                PreLoadOutput.AppendLine(txt);
            else
                Interface.Output(txt);
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
            return exceptionLog.ToString();
        }
        private static void LogException(Exception ex)
        {
            string ExtLog = "Exception (" + DateTime.Now.ToString() + ")\n" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace;
            exceptionLog.AppendLine(ExtLog);
        }

        #endregion

        #region Application Events

        public static void OnApplicationLaunch(string StartupPath)
        {
            // Initialize this classes components.
            PreLoadOutput = new StringBuilder();
            _PressedKeys = new List<Keys>();
            exceptionLog = new StringBuilder();
            llKeyboardHook = new LowLevelKeyboardHook();

            // Initialize other components. Note: Some classes are important to load before main form is loaded.
            Settings = new Settings(StartupPath);
            Settings.LoadSettings(StartupPath + "\\Settings.xml");

            Interface = new frmCLI();
        }

        public static void OnMainFormLoad()
        {
            // Give a hello message.
            if (PreLoadOutput.Length > 0)
                Output(PreLoadOutput.ToString());
            Output("Arya online.");

            // Load modules
            ModuleManager.LoadModules(Settings.ModulePath, false);

            // Prepare task scheduler.
            Scheduler = new TaskScheduler();
            Scheduler.Start(1000);

            // Don't want to handle keyboard input until everything else is intialized.
            llKeyboardHook.OnKeyDown += new LowLevelKeyboardHook.KeyPressEvent(_LLKeyboardHook_OnKeyDown);
            llKeyboardHook.OnKeyUp += new LowLevelKeyboardHook.KeyPressEvent(_LLKeyboardHook_OnKeyUp);
        }

        public static void OnApplicationExit()
        {
            // Stop all threads.
            Scheduler.Stop();
            Scheduler.StopTasks(true);

            // Save all settings.
            Settings.SaveSettings(Settings.StartupPath + "\\Settings.xml");

            // Unload modules.
            ModuleManager.UnloadModules();

            // Prepare the interface for exit.
            Interface.OnExit();
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
                if (!Core.Interface.Visible)
                    Core.Interface.Show();
                else
                    Core.Interface.Hide();
            }
        }

        #endregion
    }
}
