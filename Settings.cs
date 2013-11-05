using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

using System.Xml.Serialization;
using System.IO;
using Arya.Command;

namespace Arya
{
    public class Settings : ICommand
    {
        private Dictionary<string,string> _Settings;
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

        public Settings(string StartupPath)
        {
            _Commands = new List<string>();
            _Commands.Add("settings");
            _Settings = new Dictionary<string,string>();
            LoadDefaultSettings(StartupPath);
        }

        public void AddCustomSetting(string Name, string Value)
        {
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

        public void LoadDefaultSettings(string startupPath)
        {
            SchedulerInterval = 1000;
            StartupPath = startupPath;
            ModulePath = StartupPath + "\\Modules\\";
        }

        public void LoadSettings(string filename)
        {
            try
            {
                XmlSerializer reader = new XmlSerializer(typeof(Dictionary<string, string>));
                StreamReader file = new StreamReader(filename);
                _Settings = (Dictionary<string,string>)reader.Deserialize(file);
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
                XmlSerializer writer = new XmlSerializer(typeof(Dictionary<string,string>));
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

        private List<string> _Commands;
        public List<string> Commands
        {
            get { return _Commands; }
        }

        public void ExecuteCommand(string[] args)
        {
            if (args.Length == 2)
            {
                switch (args[1].ToLower())
                {
                    case "print":
                        PrintSettings();
                        break;
                }
            }
            else if (args.Length >= 3)
            {
                switch (args[1].ToLower())
                {
                    case "get":
                        Core.Output("Setting '" + args[2] + "' = '" + GetCustomSetting(args[2]) + "'");
                        break;
                    case "set":
                        AddCustomSetting(args[2], args[3]);
                        Core.Output("Set setting '" + args[2] + "' = '" + args[3] + "'");
                        break;
                }
            }
        }
    }
}
