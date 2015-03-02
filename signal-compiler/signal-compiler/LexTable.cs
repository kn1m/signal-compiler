using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace signalcompiler
{
    public class LexTable
    {
        private IDictionary<int, string> TokensTable;
        public const int MultiDelimitersIndex = 301;
        public const int KeywordsIndex = 401;
        public const int ConstantsIndex = 501;
        public const int IdentifiersIndex = 1001;
        //public int CurrentIndex = 0;

        public LexTable()
        {

        }

        public int RegisterToken(string token, int CurrentIndex)
        {
            CurrentIndex++;
            TokensTable.Add(CurrentIndex, token);
            return CurrentIndex;
        }

        public void PrintDictionary<T1, T2>(IDictionary<T1, T2> dict)
        {
            foreach (var p in dict)
            {
                Console.WriteLine("{0}:{1}", p.Key, p.Value);
            }
        }


    }
}
