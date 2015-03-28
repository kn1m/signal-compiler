using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace signalcompiler
{
    class SyntaxAnalyzer
    {
        private List<int> CodedTokensList;
        private IDictionary<int, string> TokensTable;
        private List<Error> Errors;

        public SyntaxAnalyzer(List<int> CodedTokens, IDictionary<int, string> Table, List<Error> ErrorList)
        {
            CodedTokensList = CodedTokens;
            TokensTable = Table;
            Errors = ErrorList;
        }

        public void Parser()
        {

        }

        private bool Feed()
        {

            return true;
        }

        public void GenerateParsingTree()
        {

        }

        public void GenerateListing()
        {

        }

        public void PrintResult()
        {
            // Print parser input

            foreach (var CodedToken in CodedTokensList)
            {
                Console.WriteLine("Coded token: {0}", CodedToken);
            }

            foreach (var p in TokensTable)
            {
                Console.WriteLine("{0}:{1}", p.Key, p.Value);
            }

        }
    }
}
