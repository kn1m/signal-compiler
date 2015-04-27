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
        private Tree.Node Temp = null;
        private Tree.Node Res = new Tree.Node
        {
            LexemType = "Function list",
            Value = "",
            Childrens = new List<Tree.Node>()
        };

        public SyntaxAnalyzer(List<Lexem> LexemList,
                              LexTable Table,
                              List<Error> ErrorList)
        {
            CodedTokensList = LexemList;
            TokensTable = Table;
            Errors = ErrorList;
        }

        public Tree Parser()
        {
            if(CheckForErrors())
            {
                return Program(CodedTokensList[0]);
            }
            else
            {
                return null;
            }
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
                    LexemType = "Lexem",
                    Value = "Program",
                    Childrens = null
                }
                );
                CurrentToken = GetCurrentLexem(CodedTokensList.IndexOf(CurrentToken));
                if(AddNode(Identifier(CurrentToken)))
                {
                    ParsingTree.Root.Childrens.Add(CurrentNode);
                    CurrentToken = GetCurrentLexem(CodedTokensList.IndexOf(CurrentToken));
                    if (TokensTable.GetToken(CurrentToken.LexemId) == ";")
                    {
                        ParsingTree.Root.Childrens.Add(new Tree.Node
                        {
                            LexemType = "Deleimiter",
                            Value = ";",
                            Childrens = null
                        }
                        );
                        CurrentToken = GetCurrentLexem(CodedTokensList.IndexOf(CurrentToken));
                        if (AddNode(Block(CurrentToken)))
                        {
                            ParsingTree.Root.Childrens.Add(CurrentNode);
                            CurrentToken = GetStackLexem(CodedTokensList.IndexOf(CurrentToken)); //asd
                            if (TokensTable.GetToken(CurrentToken.LexemId) == ".")
                            {
                                ParsingTree.Root.Childrens.Add(new Tree.Node
                                {
                                    LexemType = "Deleimiter",
                                    Value = ".",
                                    Childrens = null
                                }
                                );
                                return ParsingTree;
                            }
                            else
                            {
                                Errors.Add(ErrorGenerate(CurrentToken, "'.' expected, but" + TokensTable.GetToken(CurrentToken.LexemId) + " found."));
                                return null;
                            }
                        }
                        else
                        {
                            Errors.Add(ErrorGenerate(CurrentToken, "Block expected.")); //, but" + TokensTable.GetToken(CurrentToken.LexemId) + " found."));
                            return null;
                        }
                    }
                    else
                    {
                        Errors.Add(ErrorGenerate(CurrentToken, "';' expected, but" + TokensTable.GetToken(CurrentToken.LexemId) + " found."));
                        return null;
                    }
                }
                return null;
            }
            else
            {
                Errors.Add(ErrorGenerate(CurrentToken, "'PROGRAM expected'"));
                return null;   
            }           
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
                if (StackCount == 0 && CurrentNode.Childrens == null)
                {
                    if (ResultNode.Childrens.Any(x => x.LexemType == "Declarations"))
                    {
                        ResultNode.Childrens = null;
                    }
                    if (TokensTable.GetToken(CurrentLexem.LexemId) == "BEGIN")
                    {
                        ResultNode.Childrens = new List<Tree.Node>();
                        CurrentLexem = GetCurrentLexem(CodedTokensList.IndexOf(CurrentLexem));
                        ResultNode.Childrens.Add(new Tree.Node
                        {
                            LexemType = "Lexem",
                            Value = "BEGIN",
                            Childrens = null
                        });
                        if (TokensTable.GetToken(CurrentLexem.LexemId) == "END")
                        {
                            ResultNode.Childrens.Add(new Tree.Node
                            {
                                LexemType = "Lexem",
                                Value = "END",
                                Childrens = null
                            });
                        }
                    }
                    StackCount += 2;
                    return ResultNode;
                }
                CurrentLexem = GetStackLexem(CodedTokensList.IndexOf(CurrentLexem));//???                
                if (TokensTable.GetToken(CurrentLexem.LexemId) == "BEGIN")
                {
                    CurrentLexem = GetCurrentLexem(CodedTokensList.IndexOf(CurrentLexem));
                    ResultNode.Childrens.Add(new Tree.Node
                    {
                        LexemType = "Lexem",
                        Value = "BEGIN",
                        Childrens = null
                    });
                    if (TokensTable.GetToken(CurrentLexem.LexemId) == "END")
                    {
                        ResultNode.Childrens.Add(new Tree.Node
                        {
                            LexemType = "Lexem",
                            Value = "END",
                            Childrens = null
                        });
                        StackCount += 2;
                        return ResultNode;
                    }
                    else
                    {
                        Errors.Add(ErrorGenerate(CurrentLexem, "Unexpected symbol: " + TokensTable.GetToken(CurrentLexem.LexemId)));
                        return null;
                    }
                }
                else
                {
                    Errors.Add(ErrorGenerate(CurrentLexem, "Unexpected symbol: " + TokensTable.GetToken(CurrentLexem.LexemId) + "'BEGIN' expected."));
                    return null;
                }
            }
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
            else
            {
                Errors.Add(ErrorGenerate(CurrentLexem, "Identifier expected, but '" + TokensTable.GetToken(CurrentLexem.LexemId) + "' found."));
                return null;
            }
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

                CurrentLexem = GetCurrentLexem(CodedTokensList.IndexOf(CurrentLexem));
                ResultNode.Childrens = new List<Tree.Node>();
                ResultNode.Childrens.Add(new Tree.Node
                {
                    LexemType = "Lexem",
                    Value = "DEFFUNC",
                    Childrens = null
                });
                if(AddNode(FunctionList(CurrentLexem)))
                {
                    ResultNode.Childrens.Add(Res);//CurrentNode);
                    StackCount += 1;
                    return ResultNode;
                }
                else
                {
                    
                    return null;//??
                }
            }
            else
            {
                if(TokensTable.GetToken(CurrentLexem.LexemId) == "BEGIN")
                {
                    return ResultNode;
                }
                else
                {
                    //??
                    return null;
                }

            }

        }
        
        //??
        private Tree.Node FunctionList(Lexem CurrentLexem)
        {

            var ResultNode = new Tree.Node
            {
                LexemType = "Function list",
                Value = "",
                Childrens = null
            };

            if(Temp == null)
            {
                Temp = new Tree.Node();
                Temp = ResultNode;
            }
            //else
            //{
                /*
                if(TokensTable.GetToken(CurrentLexem.LexemId) != "BEGIN")
                {
                    StackCount += 1;
                    CurrentLexem = GetCurrentLexem(CodedTokensList.IndexOf(CurrentLexem));
                }*/
            //}

            if(AddNode(Function(CurrentLexem)))
            {
                Res.Childrens.Add(CurrentNode); // = new List<Tree.Node> { CurrentNode };//??

                CurrentLexem = GetCurrentLexem(CodedTokensList.IndexOf(CurrentLexem)+ 7);
                ResultNode.Childrens = new List<Tree.Node> { CurrentNode };

                if(AddNode(FunctionList(CurrentLexem)))
                {
                    ResultNode.Childrens.Add(CurrentNode);
                    
                    if (TokensTable.GetToken(CurrentLexem.LexemId) == "BEGIN")
                    {
                        return ResultNode;
                    }
                     return ResultNode;
                }
                else
                {
                    if (TokensTable.GetToken(CurrentLexem.LexemId) == "BEGIN")
                    {
                        Errors.RemoveAt(Errors.Count - 1);
                        return ResultNode;
                    }
                }

            }
            else
            {
                if (TokensTable.GetToken(CurrentLexem.LexemId) == "BEGIN")
                {
                    Errors.RemoveAt(Errors.Count - 1);
                    return ResultNode;
                }
                else
                {
                    return null;
                }
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
                CurrentLexem = GetCurrentLexem(CodedTokensList.IndexOf(CurrentLexem));
                if (TokensTable.GetToken(CurrentLexem.LexemId) == ",")
                {
                    ResultNode.Childrens.Add(new Tree.Node
                    {
                        LexemType = "Delimiter",
                        Value = ",",
                        Childrens = null
                    });
                    CurrentLexem = GetCurrentLexem(CodedTokensList.IndexOf(CurrentLexem));
                    if(AddNode(Constant(CurrentLexem)))
                    {
                        ResultNode.Childrens.Add(CurrentNode);
                        return ResultNode;
                    }
                    return null;
                }
                else
                {
                    Errors.Add(ErrorGenerate(CurrentLexem, "',' expected, but '" + TokensTable.GetToken(CurrentLexem.LexemId) + "' found."));
                    return null;
                }
            }
            return null;
        }

        private Tree.Node Constant(Lexem CurrentLexem)
        {
            if (TokensTable.isConstant(CurrentLexem.LexemId))
            {
                return new Tree.Node
                {
                    LexemType = "Constant",
                    Value = TokensTable.GetToken(CurrentLexem.LexemId),
                    Childrens = null
                };
            }
            else
            {
                Errors.Add(ErrorGenerate(CurrentLexem, "Constant expected, but '" + TokensTable.GetToken(CurrentLexem.LexemId) + "' found."));
                return null;
            }
        }


        private Tree.Node Function(Lexem CurrentLexem)
        {
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
                CurrentLexem = GetCurrentLexem(CodedTokensList.IndexOf(CurrentLexem));
                if (TokensTable.GetToken(CurrentLexem.LexemId) == "=")
                {
                    ResultNode.Childrens.Add(new Tree.Node
                    {
                        LexemType = "Delimiter",
                        Value = "=",
                        Childrens = null
                    });
                    CurrentLexem = GetCurrentLexem(CodedTokensList.IndexOf(CurrentLexem));
                    if(AddNode(Constant(CurrentLexem)))
                    {
                        ResultNode.Childrens.Add(CurrentNode);
                        CurrentLexem = GetCurrentLexem(CodedTokensList.IndexOf(CurrentLexem));
                        if (TokensTable.GetToken(CurrentLexem.LexemId) == "\\")
                        {
                            ResultNode.Childrens.Add(new Tree.Node
                            {
                                LexemType = "Delimiter",
                                Value = "\\",
                                Childrens = null
                            });
                            CurrentLexem = GetCurrentLexem(CodedTokensList.IndexOf(CurrentLexem));
                            if(!AddNode(FunctionCharacteristic(CurrentLexem)))
                            {
                                return null;
                            }
                            ResultNode.Childrens.Add(CurrentNode);
                            CurrentLexem = GetCurrentLexem(CodedTokensList.IndexOf(CurrentLexem) + 2);//???
                            if(TokensTable.GetToken(CurrentLexem.LexemId) == ";")
                            {
                                ResultNode.Childrens.Add(new Tree.Node
                                {
                                    LexemType = "Delimiter",
                                    Value = ";",
                                    Childrens = null
                                });
                                StackCount += 8;///???
                                return ResultNode;
                            }
                            else
                            {
                                Errors.Add(ErrorGenerate(CurrentLexem, "';' expected, but " + TokensTable.GetToken(CurrentLexem.LexemId) + " found."));
                                return null;
                            }
                        }

                    }
                    return null;
                }
                else
                {
                    Errors.Add(ErrorGenerate(CurrentLexem, "'=' expected, but " + TokensTable.GetToken(CurrentLexem.LexemId) + " found."));
                    return null;
                }
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

        public Lexem GetCurrentLexem(int Index)
        {
            Index++;
            return CodedTokensList[Index];
        }

        public Lexem GetPreviousLexem(int Index)
        {
            Index--;
            return CodedTokensList[Index];
        }

        public Lexem GetStackLexem(int Index)
        {
            //Index++;
            if (Index + StackCount >= CodedTokensList.Count)
                return CodedTokensList[CodedTokensList.Count - 1];
            return CodedTokensList[Index + StackCount];
        }

        private bool CheckForErrors()
        {
            if (Errors.Count != 0)
                return false;
            return true;
        }

        public List<Error> GetErrors()
        {   
            return Errors;
        }
    }
}
