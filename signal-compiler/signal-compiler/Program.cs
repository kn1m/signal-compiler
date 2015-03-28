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
            Console.WriteLine("Lexecial Analyzer: ");
            LexAn.StartAnalyzing();
            LexAn.PrintResults();
            Console.WriteLine("Syntax Analyzer: ");
            var SyntaxAn = new SyntaxAnalyzer(LexAn.GetCodedTokens(), LexAn.GetTokensTable(), LexAn.GetErrorList());
            SyntaxAn.PrintResult();
        }
	}
}
