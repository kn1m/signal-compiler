using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace signalcompiler
{
    public class LexTable
    {
        public IDictionary<int, string> TokensTable { get; private set; }//= new SortedDictionary<int, string>();
        private int[] CodedTokens { get; set; }
        public const int KeywordIndex = 401;
        public const int ConstantsIndex = 501;
        public const int IdentifiersIndex = 1001;
        private int CurrentIndexConstants;
        private int CurrentIndexIdentifiers;
        private int CurrentKeywordIndex;

        public LexTable()
        {
            TokensTable = new SortedDictionary<int, string>();
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

        public bool isIdentifier(int LexemId)
        {
            if (LexemId > 1000)
            {
                return true;
            }
            return false;
        }

        public bool isConstant(int LexemId)
        {
            if ( (LexemId > 500 && LexemId <= 1000) || (LexemId >= 48 && LexemId < 58))
            {
                return true;
            }
            return false;
        }


        public void PrintDictionary<T1, T2>(IDictionary<T1, T2> dict)
        {
            foreach (var p in dict)
            {
                Console.WriteLine("{0}:{1} ", p.Key, p.Value);
            }
        }

        public int GetTokenId(string token)
        {
            foreach (var p in TokensTable)
            {
                if(p.Value == token)
                {
                    return p.Key;
                }
            }
            return 0;
        }

        public string GetToken(int Code)
        {
            //string Token = null;
            foreach (var p in TokensTable)
            {
                if (p.Key == Code)
                {
                    //if(Code <= 48 || Code >= 58)
                        return p.Value;
                    //Token = p.Value;
                }
            }
            return null;// Token;
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
