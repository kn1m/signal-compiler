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
            LexAn.StartAnalyzing();
            /*
            byte[] asciiBytes = Encoding.ASCII.GetBytes(value);
            foreach (var code in asciiBytes)
            {
                Console.Write(code + " ");
            }
            */
        }
	}
}
