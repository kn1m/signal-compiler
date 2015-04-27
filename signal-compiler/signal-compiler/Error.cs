using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace signalcompiler
{
    class Error
    {
        public int CodeLineNumber { get; set; }
        public int CodeColumnNumber { get; set; }
        public string CodeErrorType { get; set; }

        public override string ToString()
        {
            return string.Format("Line: {0}, Column: {1}, Message: {2}", CodeLineNumber, CodeColumnNumber, CodeErrorType);
        }
    }
}
