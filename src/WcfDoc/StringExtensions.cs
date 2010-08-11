using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfDoc
{
    public static class StringExtensions
    {
        public static string[] GetWords(this string text, int maxLength)
        {
            List<string> words = new List<string>();
            int position = 0;
            if (text.Length > maxLength)
            {
                for (int index = 0; index <= text.Length - 1; index++)
                {
                    if (text[index] == ' ' || index - position >= maxLength || index == text.Length - 1 || text[index] == '\n')
                    {
                        words.Add(
                            text.Substring(
                                position + (position == 0 ? 0 : 1), 
                                index - position - (text[index] == ' ' && position != 0 ? 1 : 0)
                                ).Trim());
                        if (text[index] == '\n') words.Add("\n");
                        position = index;
                    }
                }
            }
            else words.Add(text);

            return words.ToArray();
        }
    }
}
