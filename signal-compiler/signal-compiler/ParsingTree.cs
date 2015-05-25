using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace signalcompiler
{
    public class Tree
    {
        public Node Root { get; set; }
        public class Node
        {
            public string LexemType { get; set; }
            public string Value { get; set; }
            public List<Node> Childrens { get; set; }
            public override string ToString()
            {
                if (Childrens != null && Childrens.Any())
                {
                    var childrenStrBuilder = new StringBuilder();
                    foreach (var child in Childrens)
                    {
                        childrenStrBuilder.AppendFormat("{0} ", child);
                    }
                    return string.Format("[{0} {1} {2}]", LexemType, Value, childrenStrBuilder);
                }
                return string.Format("[{0} {1}]", LexemType, Value);
            }
        }
    }
}
