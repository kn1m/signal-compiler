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
            var SyntaxAn = new SyntaxAnalyzer(LexAn.GetLexemList(),
                                              LexAn.GetTokensTable(),
                                              LexAn.GetErrorList());

            Console.WriteLine("Syntax Analyzer: ");

            var ParsingTree = SyntaxAn.Parser();
            if (ParsingTree != null)
            {
                string TextTree = "Parsing tree: " + ParsingTree.Root.ToString() + "\n";
                System.IO.File.WriteAllText(@"C:\Users\m3sc4\Tree.txt", TextTree);
                Console.WriteLine(TextTree);
                //Console.Write("Parsing tree: " + ParsingTree.Root.ToString() + "\n");
            }
            if(SyntaxAn.GetErrors() != null)
            {
                var Errors = SyntaxAn.GetErrors();
                foreach(var Error in Errors)
                {
                    Console.WriteLine(Error.ToString());
                }
            }
        }
	}

}
