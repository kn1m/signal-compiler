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
        private string code;

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
                                    break;
                                }
                            }
                            if (!AlreadyRegistered)
                            {
                                CodedTokens.Add(Table.RegisterIdentifier(PossibleToken));
                            }
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

        public List<int> GetCodedTokens()
        {
            return CodedTokens;
        }

        public List<Error> GetErrorList()
        {
            return Errors;
        }

        public IDictionary<int, string> GetTokensTable()
        {
            return Table.GetTokensTable();
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
                Console.WriteLine("{0} ", Token);
            }
        }
    }
}
