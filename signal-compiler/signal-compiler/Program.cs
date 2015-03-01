using System;

namespace signalcompiler
{
	class MainClass
	{
        public static void Main(string[] args)
        {
            var LexAn = new LexAnalyzer(@"C:\Users\m3sc4\test.sg");
            string[] lines = LexAn.GetCodeFromFile();
            foreach(var line in lines) {
                Console.WriteLine(line);
            }
        }
	}
}
