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
        private List<Array> ArrayDeclarations = new List<Array>();

        private struct Array
        {
            public string Idenftificator;
            public string ArraySize;
            public string FirstNumber;
            public string SecondNumber;
        }


        public CodeGenerator(Tree ParsingTree, string Filename)
        {
            this.ParsingTree = ParsingTree;
            this.Filename = Filename;
        }

        public void GenerateCode()
        {
            string Code = "";
            Code += ".386\n";
            Code += "DATA SEGMENT\n";
            GetFunctionDefinitions(ParsingTree.Root);
            foreach(var FunctionDeclaration in ArrayDeclarations)
            {
                Code += "\t" + FunctionDeclaration.Idenftificator + " dw " + FunctionDeclaration.ArraySize + " dup (" + FunctionDeclaration.FirstNumber + "," + FunctionDeclaration.SecondNumber + ")\n"; 
            }
            Code += "DATA ENDS\n";
            Code += "CODE SEGMENT\n";
            Code += "\tnop\n";
            Code += "CODE ENDS\n";
            System.IO.File.WriteAllText(@"C:\Users\m3sc4\GeneratedCode.asm", Code);
            Console.Write(Code);
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
