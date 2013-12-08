using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Arya.Storage
{
    /// <summary>
    /// Loads and saves arrays of strings that are delimited by a character.
    /// </summary>
    public class DelimitedDataFile
    {
        public List<string[]> Rows { get; private set; }
        public char Delimiter { get; private set; }

        /// <summary>
        /// Creates an empty DelimitedDataFile.
        /// </summary>
        /// <param name="delimiter">character used to separate strings when loading or saving the file</param>
        public DelimitedDataFile(char delimiter)
        {
            Rows = new List<string[]>();
            Delimiter = delimiter;
        }
        /// <summary>
        /// Creates and loads a DelimitedDataFile from the specified filename.
        /// </summary>
        /// <param name="delimiter">character used to separate strings when loading or saving the file</param>
        /// <param name="filename">full file name of to load when creating the object</param>
        public DelimitedDataFile(char delimiter, string filename)
        {
            Delimiter = delimiter;
            Load(filename);
        }

        public void Add(string[] row)
        {
            Rows.Add(row);
        }

        public void Clear()
        {
            Rows.Clear();
        }

        public string[] this[int key]
        {
            get
            {
                return Rows[key];
            }
            set
            {
                Rows[key] = value;
            }
        }

        public void Save(string filename)
        {
            var writer = new StreamWriter(filename);
            foreach (string[] row in Rows)
            {
                for (int i = 0; i < row.Length; i++)
                {
                    var val = row[i];
                    if (i == row.Length - 1)
                        writer.Write(val + "\n");
                    else
                        writer.Write(val + Delimiter);
                }
            }
            writer.Close();
        }

        public void Load(string filename)
        {
            Rows = new List<string[]>();
            var reader = new StreamReader(filename);

            var firstRow = reader.ReadLine();
            var firstRowSplit = firstRow.Split(Delimiter);

            while (reader.Peek() >= 0)
                Rows.Add(reader.ReadLine().Split(Delimiter));

            reader.Close();
        }
    }
}
