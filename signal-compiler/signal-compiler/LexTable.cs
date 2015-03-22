using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace signalcompiler
{
    public class LexTable
    {
        private IDictionary<int, string> TokensTable = new SortedDictionary<int, string>();
        private int[] CodedTokens { get; set; }
        public const int ConstantsIndex = 501;
        public const int IdentifiersIndex = 1001;
        private int CurrentIndexConstants;
        private int CurrentIndexIdentifiers;

        public LexTable()
        {
            CurrentIndexConstants = ConstantsIndex;
            CurrentIndexIdentifiers = IdentifiersIndex;
        }

        public int RegisterConstant(string token)
        {
            CurrentIndexConstants++;
            TokensTable.Add(CurrentIndexConstants, token);
            return CurrentIndexConstants;
        }

        public int RegisterIdentifier(string token)
        {
            CurrentIndexIdentifiers++;
            TokensTable.Add(CurrentIndexIdentifiers, token);
            return CurrentIndexIdentifiers;
        }

        public void PrintDictionary<T1, T2>(IDictionary<T1, T2> dict)
        {
            foreach (var p in dict)
            {
                Console.WriteLine("{0}:{1}", p.Key, p.Value);
            }
        }

        public void PrintTable()
        {
            PrintDictionary(TokensTable);
        }

    }
}
