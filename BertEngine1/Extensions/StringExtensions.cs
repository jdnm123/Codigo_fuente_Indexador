using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Microsoft.ML.Models.BERT.Extensions
{
    public static class StringExtensions
    {
        public static string CleanNames(string input)
        {
            Regex r = new Regex("(?:[^a-z ]|(?<=['\"])s)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            string v = r.Replace(input, " ");
            v = v.Replace(" ", "");
            while (v.Contains("  "))
            {
                v = v.Replace("  ", " ");
            }
            return v.ToLower().TrimEnd(' ').TrimStart(' ');
        }

        public static IEnumerable<string> SplitAndKeep(this string s, params char[] delimiters)
        {
            int start = 0, index;

            while ((index = s.IndexOfAny(delimiters, start)) != -1)
            {
                if (index - start > 0)
                    yield return s.Substring(start, index - start);

                yield return s.Substring(index, 1);

                start = index + 1;
            }

            if (start < s.Length)
            {
                yield return s.Substring(start);
            }
        }
    }
}
