using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arya
{
    class CommandInterpreter
    {
        public CommandInterpreter()
        {
        }

        public bool InterpretCommand(string Command)
        {
            string[] Arguments = ParseCommand(Command);
            if (Arguments.Length == 0)
                return false;

            if (Arguments[0] == "help" || Arguments[0] == "?" || Arguments[0] == "commands" || Arguments[0] == "cmds")
                HandleHelp(Arguments);
            else if (Arguments[0] == "mm" || Arguments[0] == "modulemanager")
                HandleModuleManager(Arguments);
            else if (Arguments[0] == "settings")
                HandleSettings(Arguments);

            return true;
        }

        private string[] ParseCommand(string command)
        {
            // Todo: Account for quotation marks creating strings.
            return command.Split(' ');
        }

        #region Help
        private void HandleHelp(string[] Arguments)
        {
            if (Arguments.Length == 1)
            {
                Core.Output("Arya Core Commands:");
                Core.Output("ModuleManager\tmm,modulemanager");
                Core.Output("Settings\tsettings");
            }
            else
            {
                if (Arguments[1] == "mm" || Arguments[1] == "modulemanager")
                {
                    Core.Output("ModuleManager Commands:");
                    Core.Output("Reload modules: rm,reloadmodules");
                }
                else if (Arguments[1] == "settings")
                {
                    Core.Output("Settings Commands:");
                    Core.Output("Print settings: print");
                    Core.Output("Get setting: get <name>");
                    Core.Output("Set setting: set <name> <value>");
                }
            }
        }
        #endregion

        #region ModuleManager
        private void HandleModuleManager(string[] Arguments)
        {
            if (Arguments.Length == 2)
            {
                if (Arguments[1] == "rm" || Arguments[1] == "reloadmodules")
                    Core._ModuleManager.Reload(Core._Settings.ModulePath);
            }
        }
        #endregion

        #region Settings
        private void HandleSettings(string[] Arguments)
        {
            if (Arguments.Length == 2)
            {
                if (Arguments[1] == "print")
                    Core._Settings.PrintSettings();
            }
            else if (Arguments.Length == 3)
            {
                if (Arguments[1] == "get")
                {
                    Core.Output(Core._Settings.GetCustomSetting(Arguments[2]));
                }
                else if (Arguments[1] == "set")
                {
                    Core._Settings.AddCustomSetting(Arguments[2], Arguments[3]);
                    Core.Output("Setting " + Arguments[2] + " set to " + Arguments[3]);
                }
            }
        }
        #endregion
    }
}
