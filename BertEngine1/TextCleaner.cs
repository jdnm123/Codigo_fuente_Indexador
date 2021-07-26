using System;
using System.Collections.Generic;
using System.Linq;

namespace BertEngine
{
    public static class TextCleaner
    {
        private static List<char> validChars = new List<char>
        { 
            'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o',
            'p', 'a', 's', 'd', 'f', 'g', 'h', 'j', 'k',
            'l', 'ñ', 'z', 'x', 'c', 'v', 'b', 'n', 'm',
            'á', 'é', 'í', 'ó', 'ú'
        };

        /// <summary>
        /// Limpia el texto del pdf y lo convierte en texto limpio para BERT.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string CleanBert(string input)
        {
            input = input.ToLower().Replace("|", " ");
            char[] delims = new[] { '\r', '\n' };
            string v = string.Join(" "/*Environment.NewLine.ToLower()*/,
                string.Join(" ", input.Split("".ToCharArray())
                .Where(x => x != "" && validChars.Contains(x[0]))).Split(delims,
                StringSplitOptions.RemoveEmptyEntries).Where(x => !string.IsNullOrWhiteSpace(x)
                && !x.Contains("|") && x != "república de colombia" && x != "republica de colombia"));
            while (v.Contains("  "))
            {
                v = v.Replace("  ", " ");
            }
            return v.ToLower().TrimEnd(' ').TrimStart(' ');
        }
    }
}
