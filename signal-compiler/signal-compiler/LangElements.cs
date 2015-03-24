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
            Whitespace,
            Delimiter,
            CommentStart,
            Letters,
            Digits,
            Error,
            Additional
        }

        public static IEnumerable<char> Whitespace;
        public static IEnumerable<char> Delimiter;
        public static IEnumerable<char> Letters;
        public static IEnumerable<char> Digits;
        public static IEnumerable<char> Additional;
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
                "PROGRAM",
                "END",
                "BEGIN",
                "DEFFUNC"
            };
            Delimiter = new[]
            {
                ';',
                '\\',
                ',',
                '='
            };
            Additional = new[]
            {
                '*',
                ')',
                '('
            };
            Letters = GenCharEnumerable('A', 'Z');
            Digits = GenCharEnumerable('0', '9');

            GenerateAttributes();
        }      
            
        public static void GenerateAttributes()
        {
            Attributes = new LangElementsTypes[255];
            for (int i = 0; i < 255; i++)
            {
                Attributes[i] = DetectAttribute((char)i);
            }
        }

        public static bool CheckKeyword(string PossibleKeyword)
        {
            if(Keywords.Contains(PossibleKeyword))
            {
                return true;
            }
            return false;
        }

        private static LangElementsTypes DetectAttribute(char cur)
        {

            if (Delimiter.Contains(cur))
            {
                return LangElementsTypes.Delimiter;
            }
            if (Digits.Contains(cur))
            {
                return LangElementsTypes.Digits;
            }
            if (Letters.Contains(cur))
            {
                return LangElementsTypes.Letters;
            }
            if (Whitespace.Contains(cur))
            {
                return LangElementsTypes.Whitespace;
            }

            if (cur == CommentStart[0])
            {
                return LangElementsTypes.CommentStart;
            }

            return LangElementsTypes.Error;
        }

        private static IEnumerable<char> GenCharEnumerable(char start, char fin)
        {
            return Enumerable.Range(start, (fin - start) + 1).Select(c => (char)c);
        }
    }
}
