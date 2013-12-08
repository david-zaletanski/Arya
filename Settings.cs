using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

using Microsoft.Win32;
using System.Xml.Serialization;
using System.IO;
using Arya.Command;

namespace Arya
{
    public class Settings
    {
        /// <summary>
        /// How often the scheduler checks for jobs that need to be executed.
        /// </summary>
        public int SchedulerInterval { get { return Convert.ToInt32(_Settings["SchedulerInterval"]); } set { _Settings["SchedulerInterval"] = value.ToString(); } }
        /// <summary>
        /// Startup path of the application, populated before the CLI form launches.
        /// </summary>
        public string StartupPath { get { return _Settings["StartupPath"]; } set { _Settings["StartupPath"] = value; } }
        /// <summary>
        /// Location to search for modules to load.
        /// </summary>
        public string ModulePath { get { return _Settings["ModulePath"]; } set { _Settings["ModulePath"] = value; } }

        #region Settings Object
        private SerializableDictionary<string, string> _Settings;

        public Settings(string StartupPath)
        {
            _Settings = new SerializableDictionary<string,string>();
            LoadDefaultSettings(StartupPath);
        }

        public void SetCustomSetting(string Name, string Value)
        {
            if(_Settings.ContainsKey(Name))
                _Settings[Name] = Value;
            else
                _Settings.Add(Name, Value);
        }

        public void PrintSettings()
        {
            if (_Settings.Keys.Count == 0)
                return;
            Core.Output("Arya Settings:");
            foreach(string setting in _Settings.Keys.ToList<string>())
                Core.Output(setting + "=" + _Settings[setting]);
        }

        public string GetCustomSetting(string Name)
        {
            string value="";
            if (!_Settings.TryGetValue(Name, out value))
            {
                Core.Output("WARNING: Could not find custom setting " + Name);
                return "";
            }
            return value;
        }

        public bool DoesSettingExist(string Name)
        {
            return _Settings.ContainsKey(Name);
        }

        public void RemoveCustomSetting(string Name)
        {
            _Settings.Remove(Name);
        }

        public void LoadDefaultSettings(string startupPath)
        {
            SchedulerInterval = 1000;
            StartupPath = startupPath;
            ModulePath = StartupPath + "\\Modules\\";
        }

        #endregion

        #region Windows Startup
        public void AddWindowsStartupKey()
        {
            try
            {
                RegistryKey startupKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                startupKey.SetValue("Arya", "\"" + StartupPath + "\\Arya.exe\"");
            }
            catch (Exception ex)
            {
                Core.HandleEx(ex);
            }
        }
        public void RemoveWindowsStartupKey()
        {
            try
            {
                RegistryKey startupKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                startupKey.DeleteValue("Arya");
            }
            catch (Exception ex)
            {
                Core.HandleEx(ex);
            }
        }

        #endregion

        #region Save/Load Settings File

        public void LoadSettings(string filename)
        {
            FileInfo settingsFile = new FileInfo(filename);
            if (!settingsFile.Exists)
            {
                Core.Output("Settings file '" + filename + "' does not exist.");
                return;
            }
            try
            {
                XmlSerializer reader = new XmlSerializer(typeof(SerializableDictionary<string, string>));
                StreamReader file = new StreamReader(filename);
                _Settings = (SerializableDictionary<string,string>)reader.Deserialize(file);
                file.Close();
            }
            catch (Exception ex)
            {
                Core.Output("WARNING: Exception occured while loading settings from\n"+filename);
                Core.HandleEx(ex);
            }
        }

        public void SaveSettings(string filename)
        {
            try
            {
                XmlSerializer writer = new XmlSerializer(typeof(SerializableDictionary<string,string>));
                StreamWriter file = new StreamWriter(filename, false);
                writer.Serialize(file, _Settings);
                file.Close();
            }
            catch (Exception ex)
            {
                Core.Output("WARNING: Exception occured while saving settings from\n"+filename);
                Core.HandleEx(ex);
            }
        }

        #endregion

        #region Commands

        public void Execute(string[] args)
        {
            if (args.Length == 0)
            {
                Core.Output("Settings Commands\n");
                Core.Output("print    Prints out all settings.");
                Core.Output("get <name>    Returns the value of <name>.");
                Core.Output("set <name> <value>    Sets the value of <name> to <value>.");
                Core.Output("unset <name>    Removes the setting <name>.");
                Core.Output("addstartupkey    Adds an application start up key to the Windows registry (Arya will start when Windows starts).");
                Core.Output("removestartupkey    Removes an application start up key from the Windows registery (Arya will not start when Windows starts).");
            }
            if (args.Length == 1)
            {
                switch (args[0].ToLower())
                {
                    case "print":
                        PrintSettings();
                        break;
                    case "addstartupkey":
                        AddWindowsStartupKey();
                        break;
                    case "removestartupkey":
                        RemoveWindowsStartupKey();
                        break;
                }
            }
            else if (args.Length >= 2)
            {
                switch (args[0].ToLower())
                {
                    case "get":
                        Core.Output("Setting '" + args[1] + "' = '" + GetCustomSetting(args[1]) + "'");
                        break;
                    case "set":
                        if (args.Length != 3)
                        {
                            Core.Output("Usage: settings set <name> <value>");
                        }
                        else
                        {
                            SetCustomSetting(args[1], args[2]);
                            Core.Output("Set setting '" + args[1] + "' = '" + args[2] + "'");
                        }
                        break;
                    case "unset":
                        RemoveCustomSetting(args[1]);
                        Core.Output("Removed setting '" + args[1] + "'");
                        break;
                }
            }
        }

        #endregion
    }
}
