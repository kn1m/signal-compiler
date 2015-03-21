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


        public LexAnalyzer(string FileName)
        {
            /*
            if (System.IO.File.Exists(FileName))
            {
                Name = FileName;
            }
            else
            {
                throw new FileNotFoundException();
            }*/
            try
            {
                if (System.IO.File.Exists(FileName))
                {
                    Name = FileName;
                }
            }
            catch
            {
                throw new FileNotFoundException();
            }
        }

        public string[] GetCodeFromFile()
        {
            return System.IO.File.ReadAllLines(Name);
        }

        public void StartAnalyzing()
        {
            var code = string.Join("", GetCodeFromFile());
            var Table = new LexTable();
            List<string> LineTokens = new List<string>();
            
            //Console.WriteLine(line);
            int i = 0;
            while (i < code.Length)
            {
                char symbol = code[i];
                var Attributes = LangElements.Attributes[symbol];
                string PossibleToken = "";
                switch (Attributes)
                {
                    case LangElements.LangElementsTypes.Letters:
                        while (i < code.Length 
                            && (LangElements.Attributes[code[i]]
                                    == LangElements.LangElementsTypes.Letters 
                                || LangElements.Attributes[code[i]] 
                                    == LangElements.LangElementsTypes.Digits ) )
                        {

                            PossibleToken += code[i];
                            i++;
                        }


                        break;
                    case LangElements.LangElementsTypes.Digits:
                        
                        while (i < code.Length && LangElements.Attributes[code[i]]
                            == LangElements.LangElementsTypes.Digits)
                        {

                            PossibleToken += code[i];
                            i++;
                        }
                        break;

                    case LangElements.LangElementsTypes.Delimiter:

                        break;
                    case LangElements.LangElementsTypes.Whitespace:

                        break;

                }
                LineTokens.Add(PossibleToken);
  
                i++;
            }

            int z = 0;
            foreach (var Token in LineTokens)
            {

                Console.WriteLine("Token {0}: {1}", z, Token);
                i++;
            }


        }


        public void PrintResults()
        {

        }

    }
}
