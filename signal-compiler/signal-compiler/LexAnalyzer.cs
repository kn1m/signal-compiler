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
        private List<Lexem> Lexems = new List<Lexem>();
        private string code;

        public LexAnalyzer(string FileName)
        {
            if(System.IO.File.Exists(FileName))
            {
                Name = FileName;
            }
            else
            {
                throw new FileNotFoundException();
            }
            code = GetCodeFromFile();
        }

        public string GetCodeFromFile()
        {
            return System.IO.File.ReadAllText(Name);
        }

        public void StartAnalyzing()
        {

            bool Comment = false;

            int i = 0;
            while (i < code.Length)
            {
                var Attributes = LangElements.Attributes[code[i]];
                string PossibleToken = "";
                switch (Attributes)
                {
                    case LangElements.LangElementsTypes.Letters:

                        int j = i;

                        var Position = GenerateLexemPosition(i);

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
                            bool AlreadyRegistered = false;
                            foreach (var TableToken in Table.GetTokensTable())
                            {
                                if (TableToken.Value == PossibleToken)
                                {
                                    AlreadyRegistered = true;
                                    CodedTokens.Add(Table.GetTokenId(PossibleToken));
                                    break;
                                }
                            }
                            if (!AlreadyRegistered)
                            {
                                CodedTokens.Add(Table.RegisterIdentifier(PossibleToken));
                            }
                        }
                        else
                        {
                            CodedTokens.Add(Table.GetTokenId(PossibleToken));
                        }

                        Lexems.Add(new Lexem
                        {
                            LexemId = Table.GetTokenId(PossibleToken),
                            LexemPosition = Position
                        });

                        break;

                    case LangElements.LangElementsTypes.Digits:

                        j = i;

                        Position = GenerateLexemPosition(i);

                        while (j < code.Length && LangElements.Attributes[code[j]]
                            == LangElements.LangElementsTypes.Digits)
                        {
                            PossibleToken += code[j];
                            j++;
                        }

                        i = j - 1;

                        LineTokens.Add(PossibleToken);

                        CodedTokens.Add(Table.RegisterConstant(PossibleToken));

                        Lexems.Add(new Lexem
                        {
                            LexemId = Table.GetTokenId(PossibleToken),
                            LexemPosition = Position
                        });

                        break;

                    case LangElements.LangElementsTypes.Delimiter:
                        Position = GenerateLexemPosition(i);
                        PossibleToken += code[i];
                        CodedTokens.Add(Table.GetTokenId(PossibleToken));
                        LineTokens.Add(PossibleToken);
                        Lexems.Add(new Lexem
                        {
                            LexemId = Table.GetTokenId(PossibleToken),
                            LexemPosition = Position
                        });
                        break;

                    case LangElements.LangElementsTypes.Whitespace:
                        break;

                    case LangElements.LangElementsTypes.CommentStart:
                        int CommentStartPosition = i;
                        i++;

                        if (code[i] == LangElements.CommentStart[1] && !Comment)
                        {
                            Comment = true;
                        }
                       
                        j = i;
                        while (j < code.Length && Comment)
                        {
                            if (code[j] == LangElements.CommentEnd[1] && code[j - 1] == LangElements.CommentEnd[0] && j > i + 1)
                            {
                                Comment = false;
                            }
                            j++;
                        }
                        
                        i = j - 1;

                        if ( i == code.Length - 1 && Comment)
                        {
                            Errors.Add(ErrorGenerate(CommentStartPosition, "Unclosed comment"));
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
                        Errors.Add(ErrorGenerate(i, "Unknow symbol"));
                        i = j - 1;
                        
                        break;
                }
                i++;
                
            }

        }

        private Error ErrorGenerate(int Position, string ErrorMessage)
        {
            int i = 0, k = 0;
            int j = 1;

            while (i < Position)
            {
                if(code[i] == '\n')
                {
                    j++;
                    k = 0;
                }
                i++;
                k++;
            }

            return new Error
            {
                CodeColumnNumber = k,
                CodeErrorType = ErrorMessage,
                CodeLineNumber = j
            };
        }

        private Lexem.Position GenerateLexemPosition(int Position)
        {

            int i = 0, k = 0;
            int j = 1;

            while (i < Position)
            {
                if (code[i] == '\n')
                {
                    j++;
                    k = 0;
                }
                i++;
                k++;
            }

            return new Lexem.Position
            {
                Line = j,
                Column = k
            };            
        }



        public List<int> GetCodedTokens()
        {
            return CodedTokens;
        }

        public List<Error> GetErrorList()
        {
            return Errors;
        }

        public LexTable GetTokensTable()
        {
            return Table;
        }


        public List<Lexem> GetLexemList()
        {
            return Lexems;
        }

        public List<string> GetLineTokens()
        {
            return LineTokens;
        }

        public void PrintResults()
        {
            Table.PrintTable();
            Console.WriteLine();


            
            Console.WriteLine();
            foreach(var currentError in Errors)
            {
                Console.WriteLine("Line: {0}, Column: {1} : {2}", 
                    currentError.CodeLineNumber,
                    currentError.CodeColumnNumber,
                    currentError.CodeErrorType);
            }
            Console.WriteLine();
            foreach (var Token in CodedTokens)
            {
                Console.Write("{0} ", Token);
            }
            Console.WriteLine();
            /*
            foreach (var Token in Lexems)
            {
                Console.WriteLine(Token.ToString());
            }*/

            Console.WriteLine();
        }
    }
}
