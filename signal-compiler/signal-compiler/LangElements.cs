using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace signalcompiler
{
    public static class LangElements
    {
        enum LangElementsTypes
        {
            Keywords,
            Whitespace,
            Delimiter,
            MultiDelimiter,
            CommentStart,
            CommentEnd
        }

        public static IEnumerable<char> Whitespace;
        public static IEnumerable<string> Keywords;
        public static string CommentStart = "(*";
        public static string CommentEnd = "*)";





        static LangElements()
        {
            Whitespace = new[]
            {
                '\x9', //horizontal tab
                '\xA', //LF
                '\xB', //vertical tab
                '\xC', //new page
                '\xD', //CR
                '\x20' //space
            };
            Keywords = new[] 
            {
                "PROGRAM", "END", "BEGIN"
            };
        }      
            
        




    }
}
