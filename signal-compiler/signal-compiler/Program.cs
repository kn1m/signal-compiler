using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace signalcompiler
{
	class MainClass
	{
        public static void Main(string[] args)
        {
            var LexAn = new LexAnalyzer(@"C:\Users\m3sc4\test.sg");
            string[] lines = LexAn.GetCodeFromFile();
            LexAn.StartAnalyzing();
            /*
            string value = "\n\t \r\n";

            foreach(var line in lines)
            {
                Console.WriteLine(line); 
            }
            

            byte[] asciiBytes = Encoding.ASCII.GetBytes(value);
            foreach (var code in asciiBytes)
            {
                Console.Write(code + " ");
            }
            */
        }
	}
}
