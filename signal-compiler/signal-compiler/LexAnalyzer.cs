using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace signalcompiler
{
    class LexAnalyzer
    {
        private string Name;

        public LexAnalyzer(string FileName)
        {
            Name = FileName;
        }

        public string[] GetCodeFromFile()
        {
            return System.IO.File.ReadAllLines(Name);
        }

    }


}
