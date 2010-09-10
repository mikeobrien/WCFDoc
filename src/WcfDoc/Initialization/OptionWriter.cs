using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfDoc.Initialization
{
    public class OptionWriter 
    {
        private readonly Type[] _optionGroups;
        private readonly int _totalWidth;
        private readonly bool _seperated;

        private const int Margin = 2;

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
            var output = new StringBuilder();

            foreach (var optionGroup in _optionGroups)
            {
                var options = GetOptionAttributes(optionGroup);
                var columnWidth = GetMaxNameColumnWidth(2, options);
                var descriptionColumnWidth = _totalWidth - (Margin * 3) - columnWidth;

                var optionGroupAttribute = GetOptionGroupAttribute(optionGroup);

                output.AppendFormat("{0} - {1}\n\n" , optionGroupAttribute.Name, optionGroupAttribute.Description);

                foreach (var option in options)
                {
                    var flagColumnLines = GetLines(columnWidth, string.Format("/{0}", option.Name));
                    var descriptionColumnLines = GetLines(descriptionColumnWidth, option.Description);

                    for (var index = 0; index < Math.Max(flagColumnLines.Length, descriptionColumnLines.Length); index++)
                    {
                        if (index < flagColumnLines.Length)
                            output.Append(new string(' ', Margin) + flagColumnLines[index]);
                        else
                            output.Append(new string(' ', columnWidth + Margin));

                        output.Append(new string(' ', Margin));

                        output.AppendFormat("{0}\n",
                                            index < descriptionColumnLines.Length
                                                ? descriptionColumnLines[index]
                                                : new string(' ', descriptionColumnWidth));
                    }
                    if (_seperated) output.Append("\n");
                }
            }

            return output.ToString();
        }

        private static string[] GetLines(int maxWidth, string text)
        {
            var lines = new List<string>();

            if (text != null)
            {
                var words = text.GetWords(maxWidth);
                var buffer = string.Empty;

                foreach (var word in words)
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
                        buffer = word != "\n" ? word : string.Empty;
                    }
                }
                lines.Add(buffer.PadRight(maxWidth));
            }

            return lines.ToArray();
        }

        private static int GetMaxNameColumnWidth(int offset, IEnumerable<OptionAttribute> options)
        {
            return Math.Max(options.Max(o => o.Name.Length + offset), 15);
        }

        private static OptionGroupAttribute GetOptionGroupAttribute(Type optionGroup)
        {
            var attributes = optionGroup.GetCustomAttributes(typeof(OptionGroupAttribute), true);
            if (attributes.Length == 0) return null;
            return (OptionGroupAttribute)attributes[0];
        }

        private static IEnumerable<OptionAttribute> GetOptionAttributes(Type optionGroup)
        {
            var properties = optionGroup.GetProperties();

            return (from property in properties
                    select property.GetCustomAttributes(typeof (OptionAttribute), true)
                    into attributes where attributes.Length != 0 select (OptionAttribute) attributes[0]).ToList();
        }
    }
}
