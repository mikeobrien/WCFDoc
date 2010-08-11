using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using WcfDoc;

namespace WcfDoc.Initialization
{
    public class OptionWriter 
    {
        private Type[] _optionGroups;
        private int _totalWidth;
        private bool _seperated;

        private const int MARGIN = 2;

        public OptionWriter(Type[] optionGroups, int totalWidth, bool seperated)
        {
            _optionGroups = optionGroups;
            _totalWidth = totalWidth;
            _seperated = seperated;
        }

        public override string ToString()
        {
            return Render();
        }

        private string Render()
        {
            StringBuilder output = new StringBuilder();

            foreach (Type optionGroup in _optionGroups)
            {
                List<OptionAttribute> options = GetOptionAttributes(optionGroup);
                int columnWidth = GetMaxNameColumnWidth(2, options);
                int descriptionColumnWidth = _totalWidth - (MARGIN * 3) - columnWidth;

                OptionGroupAttribute optionGroupAttribute = GetOptionGroupAttribute(optionGroup);

                output.AppendFormat("{0} - {1}\n\n" , optionGroupAttribute.Name, optionGroupAttribute.Description);

                foreach (OptionAttribute option in options)
                {
                    string[] flagColumnLines = GetLines(columnWidth, string.Format("/{0}", option.Name));
                    string[] descriptionColumnLines = GetLines(descriptionColumnWidth, option.Description);

                    for (int index = 0; index < Math.Max(flagColumnLines.Length, descriptionColumnLines.Length); index++)
                    {
                        if (index < flagColumnLines.Length)
                            output.Append(new string(' ', MARGIN) + flagColumnLines[index]);
                        else
                            output.Append(new string(' ', columnWidth + MARGIN));

                        output.Append(new string(' ', MARGIN));

                        if (index < descriptionColumnLines.Length)
                            output.AppendFormat("{0}\n" , descriptionColumnLines[index]);
                        else
                            output.AppendFormat("{0}\n" , new string(' ', descriptionColumnWidth));
                    }
                    if (_seperated) output.Append("\n");
                }
            }

            return output.ToString();
        }

        private string[] GetLines(int maxWidth, string text)
        {
            List<string> lines = new List<string>();

            if (text != null)
            {
                string[] words = text.GetWords(maxWidth);
                string buffer = string.Empty;

                foreach (string word in words)
                {
                    if (word.Length == maxWidth)
                        lines.Add(word);
                    else if (word.Length + buffer.Length <= maxWidth && word != "\n")
                    {
                        if (buffer.Length > 0)
                            buffer += string.Format(" {0}", word);
                        else
                            buffer += word;
                    }
                    else
                    {
                        lines.Add(buffer.PadRight(maxWidth));
                        if (word != "\n")
                            buffer = word;
                        else
                            buffer = string.Empty;
                    }
                }
                lines.Add(buffer.PadRight(maxWidth));
            }

            return lines.ToArray();
        }

        private int GetMaxNameColumnWidth(int offset, List<OptionAttribute> options)
        {
            int maxWidth = 0;

            foreach (OptionAttribute option in options)
            {
                maxWidth = Math.Max(maxWidth, option.Name.Length + offset);
            }

            return Math.Max(maxWidth, 15);
        }

        private static OptionGroupAttribute GetOptionGroupAttribute(Type optionGroup)
        {
            object[] attributes = optionGroup.GetCustomAttributes(typeof(OptionGroupAttribute), true);
            if (attributes == null || attributes.Length == 0) 
                return null;
            else
                return (OptionGroupAttribute)attributes[0];

        }

        private static List<OptionAttribute> GetOptionAttributes(Type optionGroup)
        {
            List<OptionAttribute> options = new List<OptionAttribute>();

            PropertyInfo[] properties = optionGroup.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                object[] attributes = property.GetCustomAttributes(typeof(OptionAttribute), true);
                if (attributes == null || attributes.Length == 0) continue;
                options.Add((OptionAttribute)attributes[0]);
            }

            return options;
        }
    }
}
