using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace signalcompiler
{
    public class CodeGenerator
    {
        private string Filename;
        private Tree ParsingTree;
        private List<Constant> Constants;
        private List<Array> ArrayDeclarations = new List<Array>();
        private List<Error> Errors = new List<Error>();

        private struct Array
        {
            public string Idenftificator;
            public string ArraySize;
            public string FirstNumber;
            public string SecondNumber;    
        }
        public CodeGenerator(Tree ParsingTree, string Filename, List<Constant> ConstantList)
        {
            this.ParsingTree = ParsingTree;
            this.Filename = Filename;
            this.Constants = ConstantList;
        }

        public void GenerateCode()
        {
            string Code = "";
            Code += ".386\n";
            
            GetFunctionDefinitions(ParsingTree.Root);
            if (ArrayDeclarations.Count != 0)
            {
                Code += "DATA SEGMENT\n";
                foreach (var FunctionDeclaration in ArrayDeclarations)
                {
                    Code += "\t" + FunctionDeclaration.Idenftificator + " dw " + FunctionDeclaration.ArraySize + " dup (" + FunctionDeclaration.FirstNumber + "," + FunctionDeclaration.SecondNumber + ")\n";
                }
                Code += "DATA ENDS\n";
            }

            Code += "CODE SEGMENT\n";
            Code += "\tnop\n";
            Code += "CODE ENDS\n";
            if (Errors.Count == 0)
            {
                System.IO.File.WriteAllText(@"C:\Users\m3sc4\GeneratedCode.asm", Code);
                Console.Write(Code);
            }
            else
            {
                foreach (var Error in Errors)
                    Console.WriteLine(Error.ToString());
            }
        }

        public Error ErrorGenerate(Constant CurrentLexem, string ErrorMessage)
        {
            return new Error
            {
                CodeLineNumber = CurrentLexem.LexemPosition.Line,
                CodeErrorType = ErrorMessage,
                CodeColumnNumber = CurrentLexem.LexemPosition.Column
            };
        }


        public void GetFunctionDefinitions(Tree.Node CurrentNode)
        {
            if (CurrentNode.Childrens != null)
            {
                var NewNode = CurrentNode.Childrens.FindAll(x => x.LexemType == "Function");
                if (NewNode != null)
                {
                    foreach (var Node in NewNode)
                    {
                        Array Function = new Array();
                        Function.Idenftificator = Node.Childrens.Find(y => y.LexemType == "Identifier").Value;
                        Function.ArraySize = Node.Childrens.Find(y => y.LexemType == "Constant").Value;
                        if(Function.ArraySize == "0")
                        {
                                Errors.Add(ErrorGenerate(Constants.Find(x => x.Value == 0), "Impossible to declare math function. '0' can't be used."));
                                return;
                        }
                        Function.FirstNumber = Node.Childrens.Find(y => y.LexemType == "Function characteristic").Childrens[0].Value;
                        Function.SecondNumber = Node.Childrens.Find(y => y.LexemType == "Function characteristic").Childrens[2].Value;
                        ArrayDeclarations.Add(Function);
                    }
                }
                foreach (var Node in CurrentNode.Childrens)
                {
                    GetFunctionDefinitions(Node);
                }
            }

        }



    }
}
