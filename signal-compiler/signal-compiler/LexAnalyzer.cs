using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace signalcompiler
{
    class LexAnalyzer
    {
        private string Name;
        
        public LexAnalyzer(string FileName)
        {
            /*
            if (System.IO.File.Exists(FileName))
            {
                Name = FileName;
            }
            else
            {
                throw new FileNotFoundException();
            }*/
            try
            {
                if (System.IO.File.Exists(FileName))
                {
                    Name = FileName;
                }
            }
            catch
            {
                throw new FileNotFoundException();
            }
        }

        public string[] GetCodeFromFile()
        {
            return System.IO.File.ReadAllLines(Name);
        }

        public void StartAnalyzing()
        {
            var code = GetCodeFromFile();

            foreach (var line in code)
            {

                foreach(var symbol in line)
                {
                    
                }


            }

        }

        public void PrintResults()
        {

        }

    }
}
