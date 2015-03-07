using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace signalcompiler
{
    class Error
    {
        public string CodeLine { get; set; }
        public int CodeLineNumber { get; set; }
        public string CodeErrorType { get; set; }
    }
}
