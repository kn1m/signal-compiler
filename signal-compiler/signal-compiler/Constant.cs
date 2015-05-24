using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace signalcompiler
{
    public class Constant
    {
        public int Value { get; set; }
        public Position LexemPosition { get; set; }

        public class Position
        {
            public int Line { get; set; }
            public int Column { get; set; }
            public override string ToString()
            {
                return string.Format("Line: {0}, Column: {1}", Line, Column);
            }
        }   
    }
}
