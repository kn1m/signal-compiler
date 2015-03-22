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
         
        private List<int> CodedTokens = new List<int>();
        private List<string> LineTokens = new List<string>();
        private List<Error> Errors = new List<Error>();
        private LexTable Table = new LexTable();

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
            


            int i = 0;
            while (i < code.Length)
            {
                var Attributes = LangElements.Attributes[code[i]];
                string PossibleToken = "";
                switch (Attributes)
                {

                    case LangElements.LangElementsTypes.Letters:

                        int j = i;
                        while (j < code.Length
                            && (LangElements.Attributes[code[j]] 
                            == LangElements.LangElementsTypes.Letters ||
                                LangElements.Attributes[code[j]]
                            == LangElements.LangElementsTypes.Digits ))
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
                        while (j < code.Length && LangElements.Attributes[code[j]]
                            == LangElements.LangElementsTypes.Digits)
                        {
                            PossibleToken += code[j];
                            j++;
                        }

                        i = j - 1;

                        LineTokens.Add(PossibleToken);

                        CodedTokens.Add(Table.RegisterConstant(PossibleToken));

                        break;

                    case LangElements.LangElementsTypes.Delimiter:
                        
                        PossibleToken += code[i];

                        LineTokens.Add(PossibleToken);
                        break;

                    case LangElements.LangElementsTypes.Whitespace:
                        break;

                    case LangElements.LangElementsTypes.CommentStart:
                        bool Comment = true;
                        i++;
                        if (code[i] == LangElements.CommentStart[1])
                        {
                            Console.WriteLine("Comment started");
                        }

                        j = i;
                        while (j < code.Length && Comment)
                        {
                            if (code[j] == LangElements.CommentEnd[1] && code[j - 1] == LangElements.CommentEnd[0])
                            {
                                Console.WriteLine("Comment ended");
                                Comment = false;
                            }
                            j++;

                        }
                        
                        i = j - 1;
                        if (i == code.Length - 1 && Comment)
                        {
                            Console.WriteLine("Comment unclosed"); // Error generate
                        }
                        break;

                    case LangElements.LangElementsTypes.Error:

                        j = i;
                        while (j < code.Length && LangElements.Attributes[code[j]]
                            == LangElements.LangElementsTypes.Error || 
                                (LangElements.Attributes[code[j]]
                            == LangElements.LangElementsTypes.Error ||
                                LangElements.Attributes[code[j]]
                            == LangElements.LangElementsTypes.Digits ||
                                LangElements.Attributes[code[j]]
                            == LangElements.LangElementsTypes.Letters))
                        {
                            PossibleToken += code[j];
                            j++;
                        }

                        i = j - 1;

                        Console.WriteLine("Error: {0} is unknown", PossibleToken); // Error generate
                        break;

                    
                }
                i++;
                
            }

        }


        public void PrintResults()
        {
            Table.PrintTable();
            Console.WriteLine();
            int z = 0;
            foreach (var Token in LineTokens)
            {
                Console.WriteLine("Token {0}: {1}", z, Token);
                z++;
            }
            
        }

    }
}
