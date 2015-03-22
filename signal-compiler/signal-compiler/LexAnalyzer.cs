using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace signalcompiler
{
    class LexAnalyzer
    {
        private string Name;
         
        private List<int>  CodedTokens = new List<int>();

        public LexAnalyzer(string FileName)
        {
            if (System.IO.File.Exists(FileName))
            {
                Name = FileName;
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

        public string GetCodeFromFile()
        {
            return System.IO.File.ReadAllText(Name);
        }

        public void StartAnalyzing()
        {
            var code = GetCodeFromFile();
            var Table = new LexTable();
            List<string> LineTokens = new List<string>();
            

            int i = 0;
            while (i < code.Length)
            {
                char symbol = code[i];
                var Attributes = LangElements.Attributes[symbol];
                string PossibleToken = "";
                switch (Attributes)
                {

                    case LangElements.LangElementsTypes.Letters:

                        int j = i;
                        while (j < code.Length
                            && (LangElements.Attributes[code[j]]
                                    == LangElements.LangElementsTypes.Letters 
                                || LangElements.Attributes[code[j]] 
                                    == LangElements.LangElementsTypes.Digits ) )
                        {
                            PossibleToken += code[j];
                            j++;
                        }

                        i = j - 1;

                        LineTokens.Add(PossibleToken);

                        if (!LangElements.CheckKeyword(PossibleToken))
                        {
                            CodedTokens.Add(Table.RegisterIdentifier(PossibleToken));
                        }

                        break;

                    case LangElements.LangElementsTypes.Digits:

                        j = i;
                        while (i < code.Length && LangElements.Attributes[code[j]]
                            == LangElements.LangElementsTypes.Digits)
                        {
                            PossibleToken += code[j];
                            j++;
                        }

                        i = j - 1;

                        LineTokens.Add(PossibleToken);

                        break;

                    case LangElements.LangElementsTypes.Delimiter:
                        
                        PossibleToken += code[i];

                        LineTokens.Add(PossibleToken);
                        break;

                    case LangElements.LangElementsTypes.Whitespace:

                        break;

                    case LangElements.LangElementsTypes.CommentStart:

                        i++;
                        if (code[i] != LangElements.CommentStart[1])
                        {

                        }


                        j = i;
                        while (j < code.Length
                            && (LangElements.Attributes[code[j]]
                                    == LangElements.LangElementsTypes.Letters 
                                || LangElements.Attributes[code[j]] 
                                    == LangElements.LangElementsTypes.Digits ) )
                        {
                            PossibleToken += code[j];
                            j++;
                        }

                        i = j - 1;

                        break;

                    
                }
                i++;

                
            }

            Table.GetRes();
            Console.WriteLine();
            int z = 0;
            foreach (var Token in LineTokens)
            {
                Console.WriteLine("Token {0}: {1}", z, Token);
                z++;
            }
            

        }


        public void PrintResults()
        {

        }

    }
}
