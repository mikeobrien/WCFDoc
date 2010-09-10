using System.Collections.Generic;

namespace WcfDoc
{
    public static class StringExtensions
    {
        public static string[] GetWords(this string text, int maxLength)
        {
            var words = new List<string>();
            var position = 0;
            if (text.Length > maxLength)
            {
                for (var index = 0; index <= text.Length - 1; index++)
                {
                    if (((text[index] != ' ' && index - position < maxLength) && index != text.Length - 1) &&
                        text[index] != '\n') continue;

                    words.Add(
                        text.Substring(
                            position + (position == 0 ? 0 : 1), 
                            index - position - (text[index] == ' ' && position != 0 ? 1 : 0)
                            ).Trim());

                    if (text[index] == '\n') words.Add("\n");
                    position = index;
                }
            }
            else words.Add(text);

            return words.ToArray();
        }
    }
}
