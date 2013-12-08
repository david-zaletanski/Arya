using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arya.Command
{
    public class CommandInterpreter
    {
        public static string[] ParseCommand(string command)
        {
            command = command.Trim();

            List<string> args = new List<string>();
            int i = 0;
            string current = "";
            bool inquote = false;
            while (i < command.Length)
            {
                if (command[i] == ' ' && !inquote)
                {
                    if (current != "")
                    {
                        args.Add(current);
                        current = "";
                    }
                }
                else if (command[i] == '"')
                {
                    if (i - 1 >= 0 && command[i - 1] != '\\')
                    {
                        inquote = !inquote;
                    }
                    else if (i-1>=0)
                    {
                        command += '"';
                    }
                }
                else
                {
                    if ((i + 1) < command.Length && command[i + 1] != '"')
                        current += command[i];
                    else if ((i + 1) == command.Length)
                        current += command[i];
                }
                i++;
            }
            if (command != "")
            {
                args.Add(current);
                current = "";
            }
            if (inquote)
            {
                Core.Output("Syntax error, uneven number of quotes");
                return new string[] { };
            }
            return args.ToArray();
        }
    }
}
