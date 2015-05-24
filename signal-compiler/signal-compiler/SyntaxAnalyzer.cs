using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace signalcompiler
{
    class SyntaxAnalyzer
    {
        private List<Lexem> CodedTokensList;
        private LexTable TokensTable;
        private List<Error> Errors;
        private Tree.Node CurrentNode;
        private List<int> AlreadyPassedIdentifier = new List<int>();
        private int StackCount = 0;
        private List<Constant> Constants = new List<Constant>();

        public SyntaxAnalyzer(List<Lexem> LexemList, LexTable Table,  List<Error> ErrorList)
        {
            CodedTokensList = LexemList;
            TokensTable = Table;
            Errors = ErrorList;
        }

        public Tree Parser()
        {
            if (Errors.Count == 0)
            {
                return Program(CodedTokensList[0]);
            }
            return null;
        }

        private Tree Program(Lexem CurrentToken)
        {
            var ParsingTree = new Tree
            {
                Root = new Tree.Node
                {
                    LexemType = "Signal program",
                    Value = "",
                    Childrens = new List<Tree.Node>()
                }
            };
            if(TokensTable.GetToken(CurrentToken.LexemId) == "PROGRAM")
            {
                ParsingTree.Root.Childrens.Add(new Tree.Node
                {
                    LexemType = "Keyword",
                    Value = "Program",
                    Childrens = null
                });
                CurrentToken = CodedTokensList.ElementAt(CodedTokensList.IndexOf(CurrentToken) + 1);
                if(AddNode(Identifier(CurrentToken)))
                {
                    ParsingTree.Root.Childrens.Add(CurrentNode);
                    CurrentToken = CodedTokensList.ElementAt(CodedTokensList.IndexOf(CurrentToken) + 1);

                    if (TokensTable.GetToken(CurrentToken.LexemId) == ";")
                    {
                        ParsingTree.Root.Childrens.Add(new Tree.Node
                        {
                            LexemType = "Delimiter",
                            Value = ";",
                            Childrens = null
                        });
                        CurrentToken = CodedTokensList.ElementAt(CodedTokensList.IndexOf(CurrentToken) + 1);
                        if (AddNode(Block(CurrentToken)))
                        {
                            ParsingTree.Root.Childrens.Add(CurrentNode);
                            CurrentToken = GetStackLexem(CodedTokensList.IndexOf(CurrentToken));
                            if (TokensTable.GetToken(CurrentToken.LexemId) == ".")
                            {
                                ParsingTree.Root.Childrens.Add(new Tree.Node
                                {
                                    LexemType = "Deleimiter",
                                    Value = ".",
                                    Childrens = null
                                });
                                return ParsingTree;
                            }
                            Errors.Add(ErrorGenerate(CurrentToken, "'.' expected, but" + TokensTable.GetToken(CurrentToken.LexemId) + " found."));
                            return null;
                        }
                        Errors.Add(ErrorGenerate(CurrentToken, "Block expected."));
                        return null;
                    }
                    Errors.Add(ErrorGenerate(CurrentToken, "';' expected, but" + TokensTable.GetToken(CurrentToken.LexemId) + " found."));
                    return null;
                }
                return null;
            }
            Errors.Add(ErrorGenerate(CurrentToken, "'PROGRAM' expected."));
            return null;           
        }

        private Tree.Node Block(Lexem CurrentLexem)
        {
            var ResultNode = new Tree.Node
            {
                LexemType = "Block",
                Value = "",
                Childrens = null 
            };
            if (AddNode(Declarations(CurrentLexem)))
            {
                ResultNode.Childrens = new List<Tree.Node> { CurrentNode };
                if (StackCount == 0 || CurrentNode.Childrens == null)
                {
                    ResultNode.Childrens = new List<Tree.Node>();
                }                  
                CurrentLexem = GetStackLexem(CodedTokensList.IndexOf(CurrentLexem));
                if (TokensTable.GetToken(CurrentLexem.LexemId) == "BEGIN")
                {
                    
                    ResultNode.Childrens.Add(new Tree.Node
                    {
                        LexemType = "Keyword",
                        Value = "BEGIN",
                        Childrens = null
                    });
                    CurrentLexem = CodedTokensList.ElementAt(CodedTokensList.IndexOf(CurrentLexem) + 1);
                    if (TokensTable.GetToken(CurrentLexem.LexemId) == "END")
                    {
                        ResultNode.Childrens.Add(new Tree.Node
                        {
                            LexemType = "Keyword",
                            Value = "END",
                            Childrens = null
                        });
                        StackCount += 2;
                        return ResultNode;
                    }
                    Errors.Add(ErrorGenerate(CurrentLexem, "Unexpected symbol: '" + TokensTable.GetToken(CurrentLexem.LexemId) + "': 'END' expected."));
                    return null;
                }
                Errors.Add(ErrorGenerate(CurrentLexem, "Unexpected symbol: " + TokensTable.GetToken(CurrentLexem.LexemId)));
                return null;
            }
            Errors.Add(ErrorGenerate(CurrentLexem, "Unexpected symbol: " + TokensTable.GetToken(CurrentLexem.LexemId) + " 'BEGIN' expected."));
            return null;
        }

        private Tree.Node Identifier(Lexem CurrentLexem)
        {
            if (TokensTable.isIdentifier(CurrentLexem.LexemId))
            {
                return new Tree.Node
                {
                    LexemType = "Identifier",
                    Value = TokensTable.GetToken(CurrentLexem.LexemId),
                    Childrens = null
                };
            }
            Errors.Add(ErrorGenerate(CurrentLexem, "Identifier expected, but '" + TokensTable.GetToken(CurrentLexem.LexemId) + "' found."));
            return null;
        }

        private Tree.Node Declarations(Lexem CurrentLexem)
        {
            var ResultNode = new Tree.Node
            {
                LexemType = "Declarations",
                Value = "",
                Childrens = null
            };
            if (TokensTable.GetToken(CurrentLexem.LexemId) == "DEFFUNC")
            {
                CurrentLexem = CodedTokensList.ElementAt(CodedTokensList.IndexOf(CurrentLexem) + 1);
                ResultNode.Childrens = new List<Tree.Node>();
                ResultNode.Childrens.Add(new Tree.Node
                {
                    LexemType = "Keyword",
                    Value = "DEFFUNC",
                    Childrens = null
                });
                var Functions = new Tree.Node
                {
                    LexemType = "Function list",
                    Value = "",
                    Childrens = new List<Tree.Node>()
                };
                if (AddNode(FunctionList(CurrentLexem, Functions)))
                {
                    ResultNode.Childrens.Add(Functions);
                    StackCount += 1;
                    return ResultNode;
                }
                return null;
            }
            if(TokensTable.GetToken(CurrentLexem.LexemId) == "BEGIN")
            {
                return ResultNode;
            }
            return null;
        }
        
        private Tree.Node FunctionList(Lexem CurrentLexem, Tree.Node CurrentFunctions)
        {
            var ResultNode = new Tree.Node
            {
                LexemType = "Function list",
                Value = "",
                Childrens = null
            };
            if(AddNode(Function(CurrentLexem)))
            {
                CurrentFunctions.Childrens.Add(CurrentNode);
                CurrentLexem = CodedTokensList.ElementAt(CodedTokensList.IndexOf(CurrentLexem) + 8); // Jump over function declaration to next token
                if (AddNode(FunctionList(CurrentLexem, CurrentFunctions)))
                {
                    return CurrentFunctions;
                }
            }
            if (TokensTable.GetToken(CurrentLexem.LexemId) == "BEGIN")
            {
                return CurrentFunctions;
            }
            return null;
        }

        private Tree.Node FunctionCharacteristic(Lexem CurrentLexem)
        {
            if (AddNode(Constant(CurrentLexem)))
            {
                var ResultNode = new Tree.Node
                {
                    LexemType = "Function characteristic",
                    Value = "",
                    Childrens = new List<Tree.Node> { CurrentNode }
                };
                CurrentLexem = CodedTokensList.ElementAt(CodedTokensList.IndexOf(CurrentLexem) + 1);
                if (TokensTable.GetToken(CurrentLexem.LexemId) == ",")
                {
                    ResultNode.Childrens.Add(new Tree.Node
                    {
                        LexemType = "Delimiter",
                        Value = ",",
                        Childrens = null
                    });
                    CurrentLexem = CodedTokensList.ElementAt(CodedTokensList.IndexOf(CurrentLexem) + 1);
                    if(AddNode(Constant(CurrentLexem)))
                    {
                        ResultNode.Childrens.Add(CurrentNode);
                        return ResultNode;
                    }
                    return null;
                }
                Errors.Add(ErrorGenerate(CurrentLexem, "',' expected, but '" + TokensTable.GetToken(CurrentLexem.LexemId) + "' found."));
                return null;
            }
            return null;
        }

        private Tree.Node Constant(Lexem CurrentLexem)
        {
            if (TokensTable.isConstant(CurrentLexem.LexemId))
            {
                Constants.Add(new Constant
                {
                    Value = Convert.ToInt32(TokensTable.GetToken(CurrentLexem.LexemId)),
                    LexemPosition = new Constant.Position
                    {
                        Line = CurrentLexem.LexemPosition.Line,
                        Column = CurrentLexem.LexemPosition.Column
                    }
                });
                return new Tree.Node
                {
                    LexemType = "Constant",
                    Value = TokensTable.GetToken(CurrentLexem.LexemId),
                    Childrens = null
                };
            }
            Errors.Add(ErrorGenerate(CurrentLexem, "Constant expected, but '" + TokensTable.GetToken(CurrentLexem.LexemId) + "' found."));
            return null;
        }

        private Tree.Node Function(Lexem CurrentLexem)
        {
            if(TokensTable.GetToken(CurrentLexem.LexemId) == "BEGIN")
            {
                return null;
            }
            if(AddNode(Identifier(CurrentLexem)))
            {   
                
                if(AlreadyPassedIdentifier.Contains(CurrentLexem.LexemId))
                {
                    Errors.Add(ErrorGenerate(CurrentLexem, "'" + TokensTable.GetToken(CurrentLexem.LexemId) + "' already defined"));
                    return null;
                }
                AlreadyPassedIdentifier.Add(CurrentLexem.LexemId);
                var ResultNode = new Tree.Node
                {
                    LexemType = "Function",
                    Value = "",
                    Childrens = new List<Tree.Node> { CurrentNode }
                };
                CurrentLexem = CodedTokensList.ElementAt(CodedTokensList.IndexOf(CurrentLexem) + 1);
                if (TokensTable.GetToken(CurrentLexem.LexemId) == "=")
                {
                    ResultNode.Childrens.Add(new Tree.Node
                    {
                        LexemType = "Delimiter",
                        Value = "=",
                        Childrens = null
                    });
                    CurrentLexem = CodedTokensList.ElementAt(CodedTokensList.IndexOf(CurrentLexem) + 1);
                    if(AddNode(Constant(CurrentLexem)))
                    {
                        ResultNode.Childrens.Add(CurrentNode);
                        CurrentLexem = CodedTokensList.ElementAt(CodedTokensList.IndexOf(CurrentLexem) + 1);
                        if (TokensTable.GetToken(CurrentLexem.LexemId) == "\\")
                        {
                            ResultNode.Childrens.Add(new Tree.Node
                            {
                                LexemType = "Delimiter",
                                Value = "\\",
                                Childrens = null
                            });
                            CurrentLexem = CodedTokensList.ElementAt(CodedTokensList.IndexOf(CurrentLexem) + 1);
                            if(!AddNode(FunctionCharacteristic(CurrentLexem)))
                            {
                                return null;
                            }
                            ResultNode.Childrens.Add(CurrentNode);
                            CurrentLexem = CodedTokensList.ElementAt(CodedTokensList.IndexOf(CurrentLexem) + 3); //Jump over function characteristic to next token
                            if(TokensTable.GetToken(CurrentLexem.LexemId) == ";")
                            {
                                ResultNode.Childrens.Add(new Tree.Node
                                {
                                    LexemType = "Delimiter",
                                    Value = ";",
                                    Childrens = null
                                });
                                StackCount += 8;
                                return ResultNode;
                            }
                            Errors.Add(ErrorGenerate(CurrentLexem, "';' expected, but " + TokensTable.GetToken(CurrentLexem.LexemId) + " found."));
                            return null;
                        }
                    }
                    return null;
                }
                Errors.Add(ErrorGenerate(CurrentLexem, "'=' expected, but " + TokensTable.GetToken(CurrentLexem.LexemId) + " found."));
                return null;
            }
            return null;
        }

        public bool AddNode(Tree.Node CurrentNode)
        {
            if(CurrentNode != null)
            {
                this.CurrentNode = CurrentNode;
                return true;
            }
            return false;
        }

        public Error ErrorGenerate(Lexem CurrentLexem, string ErrorMessage)
        {
            return new Error
            {
                CodeLineNumber = CurrentLexem.LexemPosition.Line,
                CodeErrorType = ErrorMessage,
                CodeColumnNumber = CurrentLexem.LexemPosition.Column
            };
        }

        public Lexem GetStackLexem(int Index)
        {
            if (Index + StackCount >= CodedTokensList.Count)
                return CodedTokensList[CodedTokensList.Count - 1];
            return CodedTokensList[Index + StackCount];
        }

        public List<Error> GetErrors()
        {   
            return Errors;
        }

        public List<Constant> GetConstants()
        {
            return Constants;
        }
    }
}
