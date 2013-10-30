using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Serialization;
using System.IO;

namespace Arya
{
    class Settings
    {
        /// <summary>
        /// How often the scheduler checks for jobs that need to be executed.
        /// </summary>
        public static int SchedulerInterval { get; set; }
        /// <summary>
        /// Startup path of the application, populated before the CLI form launches.
        /// </summary>
        public static string StartupPath { get; set; }

        public static void LoadDefaultSettings()
        {
            SchedulerInterval = 1000;
            StartupPath = "";
        }
        public static void LoadSettings(string filename)
        {
            
        }
        public static void SaveSettings(string filename)
        {

        }
    }
}
