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
    }
}
