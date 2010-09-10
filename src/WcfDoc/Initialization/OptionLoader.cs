using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace WcfDoc.Initialization
{
    public static class OptionLoader 
    {
        public static void Load(object optionGroup, string commandLine)
        {
            var parameters = GetParameters(commandLine);
            var options = GetOptionAttributes(optionGroup.GetType());

            foreach (var option in options)
            {
                if (parameters.ContainsKey(option.Key))
                    option.Value.SetValue(optionGroup, parameters[option.Key], null);
            }
        }

        private static Dictionary<string, string> GetParameters(string commandLine)
        {
            var parameters = new Dictionary<string, string>();
            const string commandLineRegEx = @"(?:\s*)(?<=[-|/])(?<name>\w*)\s*[:|=]\s*(""((?<value>.*?)"")|(?<value>[\w]*))";
            var regex = new Regex(commandLineRegEx);

            var matches = regex.Matches(commandLine);

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
            var options = new Dictionary<string, PropertyInfo>();

            var properties = optionGroup.GetProperties();

            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(typeof(OptionAttribute), true);
                if (attributes.Length == 0) continue;
                options.Add(((OptionAttribute)attributes[0]).Name, property);
            }

            return options;
        }
    }
}
