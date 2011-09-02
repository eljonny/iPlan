using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace GUIProj1
{
    class CSVParser
    {
    

        public static List<Member> ReadCSVFile(string filename)
        {
               int i = 0;
            List<Member> mem = new List<Member>();
            using (StreamReader sr = new StreamReader(filename))
            {
                string line;
            while ((line = sr.ReadLine()) != null)
                {
                    string[] parts = line.Split();
                    mem.Add(new Member(parts));
                }
            }
            return mem;
        }
   }

}
