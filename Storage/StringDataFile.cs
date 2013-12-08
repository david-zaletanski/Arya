using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Arya.Storage
{
    public class StringDataFile
    {
        public List<string> Rows { get; private set; }

        public StringDataFile()
        {
            Rows = new List<string>();
        }
        public StringDataFile(string filename)
        {
            Load(filename);
        }

        public void Add(string row)
        {
            if (Rows == null)
                Rows = new List<string>();
            Rows.Add(row);
        }

        public void Clear()
        {
            if(Rows!=null)
                Rows.Clear();
        }

        public string this[int key]
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
                foreach (string row in Rows)
                {
                    writer.WriteLine(row);
                }
                writer.Close();
        }

        public void Load(string filename)
        {
            Rows = new List<string>();
            var reader = new StreamReader(filename);
            
            var firstRow = reader.ReadLine();

            while (reader.Peek() >= 0)
                Rows.Add(reader.ReadLine());

            reader.Close();
        }
    }
}
