using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace signalcompiler
{
    public static class LangElements
    {
        public enum LangElementsTypes
        {
            Keywords,
            Whitespace,
            Delimiter,
            MultiDelimiter,
            CommentStart,
            Idintifier,
            Error
        }

        public static IEnumerable<char> Whitespace;
        public static IEnumerable<char> Delimiter;
        public static IEnumerable<char> Letters;
        public static IEnumerable<char> Digits;
        public static IEnumerable<string> Keywords;
        public static string CommentStart = "(*";
        public static string CommentEnd = "*)";

        public static LangElementsTypes[] Attributes;




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
            Delimiter = new[]
            {
                '*',
                '/',
                ';'
            };

        }      
            
        public static void GenerateAttributes(char cur)
        {

        }

        
        private static LangElementsTypes DetectAttribute(char cur)
        {
            if (Whitespace.Contains(cur))
            {
                return LangElementsTypes.Whitespace;
            }
            if (Delimiter.Contains(cur))
            {
                return LangElementsTypes.Delimiter;
            }




            return LangElementsTypes.Error;
        }
         


    }
}
