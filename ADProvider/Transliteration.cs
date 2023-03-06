using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ADProvider
{
    internal class Transliteration
    {
        static Dictionary<string, string> ua_en = new Dictionary<string, string>()
        {
            { "А", "A" }, { "а", "a" },
            { "Б", "B" }, { "б", "b" },
            { "В", "V" }, { "в", "v" },
            { "Г", "H" }, { "г", "h" },
            { "Ґ", "G" }, { "ґ","g" },
            { "Д", "D" }, { "д", "d" },
            { "Е", "E" }, { "е", "e" },
            { "Є", "Ye" }, { "є", "ie" },
            { "Ж", "Zh" }, { "ж", "zh" },
            { "З", "Z" }, { "з","z" },
            { "И", "Y" }, { "и", "y" },
            { "І", "I" }, { "і", "i" },
            { "Ї", "Yi" }, { "ї", "i" },
            { "Й", "Y" }, { "й","i" },
            { "К", "K" }, { "к", "k" },
            { "Л", "L" }, { "л", "l" },
            { "М", "M" }, { "м", "m" },
            { "Н", "N" }, { "н", "n" },
            { "О", "O" }, { "о", "o" },
            { "П", "P" }, { "п", "p" },
            { "Р", "R" }, { "р", "r" },
            { "С", "S" }, { "с", "s" },
            { "Т", "T" }, { "т", "t" },
            { "У", "U" }, { "у", "u" },
            { "Ф", "F" }, { "ф", "f" },
            { "Х", "Kh" }, { "х","kh" },
            { "Ц", "Ts" }, { "ц", "ts" },
            { "Ч", "Ch" }, { "ч", "ch" },
            { "Ш", "Sh" }, { "ш", "sh" },
            { "Щ", "Shch" }, { "щ", "shch" },
            { "Ю", "Yu" }, { "ю", "iu" },
            { "Я", "Ya" }, { "я", "ia"},
            { "ь", "" },
            { "'", "" }
        };
        static public string Transliterate(string text)
        {
            string result = text;
            foreach (var item in ua_en)
            {
                result = result.Replace(item.Key, item.Value);
            }

            foreach (var item in result)
            {
                if (!(('a' <= item && item <= 'z') || ('A' <= item && item <= 'Z') || item == ' '))
                    throw new ArgumentOutOfRangeException("text", $"Symbol '{item}' is not English");
            }

            return result;
        }

        static public Models.UserFullName Transliterate(Models.UserFullName name)
        {
            return new Models.UserFullName(
                Transliterate(name.FirstName),
                Transliterate(name.MiddleName),
                Transliterate(name.SurName)
            );
        }
    }
}
