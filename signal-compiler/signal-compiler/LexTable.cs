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
        public const int KeywordIndex = 401;
        public const int ConstantsIndex = 501;
        public const int IdentifiersIndex = 1001;
        private int CurrentIndexConstants;
        private int CurrentIndexIdentifiers;
        private int CurrentKeywordIndex;

        public LexTable()
        {
            CurrentKeywordIndex = KeywordIndex;
            CurrentIndexConstants = ConstantsIndex;
            CurrentIndexIdentifiers = IdentifiersIndex;
            foreach (var Keyword in LangElements.Keywords)
            {
                TokensTable.Add(CurrentKeywordIndex, Keyword);
                CurrentKeywordIndex++;
            }

            foreach (var Whitespace in LangElements.Whitespace)
            {
                string Symbol = "";
                Symbol += Whitespace; 
                TokensTable.Add((int)Symbol[0], Symbol);
            }

            foreach (var Delimiter in LangElements.Delimiter)
            {
                string Symbol = "";
                Symbol += Delimiter;
                TokensTable.Add((int)Symbol[0], Symbol);
            }

            foreach (var Letter in LangElements.Letters)
            {
                string Symbol = "";
                Symbol += Letter;
                TokensTable.Add((int)Symbol[0], Symbol);
            }

            foreach (var Digit in LangElements.Digits)
            {
                string Symbol = "";
                Symbol += Digit;
                TokensTable.Add((int)Symbol[0], Symbol);
            }

            foreach (var Additional in LangElements.Additional)
            {
                string Symbol = "";
                Symbol += Additional;
                TokensTable.Add((int)Symbol[0], Symbol);
            }
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

        public IDictionary<int, string> GetTokensTable()
        {
            return TokensTable;
        }
    }
}
