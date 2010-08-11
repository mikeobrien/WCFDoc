using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using WcfDoc;
using System.Text.RegularExpressions;

namespace WcfDoc.Initialization
{
    public static class OptionLoader 
    {
        public static void Load(object optionGroup, string commandLine)
        {
            Dictionary<string, string> parameters = GetParameters(commandLine);
            Dictionary<string, PropertyInfo> options = GetOptionAttributes(optionGroup.GetType());

            foreach (KeyValuePair<string, PropertyInfo> option in options)
            {
                if (parameters.ContainsKey(option.Key))
                    option.Value.SetValue(optionGroup, parameters[option.Key], null);
            }
        }

        private static Dictionary<string, string> GetParameters(string commandLine)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string commandLineRegEx = 
                @"(?:\s*)(?<=[-|/])(?<name>\w*)\s*[:|=]\s*(""((?<value>.*?)"")|(?<value>[\w]*))";
            Regex regex = new Regex(commandLineRegEx);

            MatchCollection matches = regex.Matches(commandLine);

            foreach (Match match in matches)
                if (match.Groups.Count >= 2 && 
                    !parameters.ContainsKey(match.Groups["name"].Value))
                        parameters.Add( 
                                match.Groups["name"].Value, 
                                match.Groups["value"].Value);
            return parameters;
        }

        private static Dictionary<string, PropertyInfo> GetOptionAttributes(Type optionGroup)
        {
            Dictionary<string, PropertyInfo> options = new Dictionary<string, PropertyInfo>();

            PropertyInfo[] properties = optionGroup.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                object[] attributes = property.GetCustomAttributes(typeof(OptionAttribute), true);
                if (attributes == null || attributes.Length == 0) continue;
                options.Add(((OptionAttribute)attributes[0]).Name, property);
            }

            return options;
        }
    }
}
