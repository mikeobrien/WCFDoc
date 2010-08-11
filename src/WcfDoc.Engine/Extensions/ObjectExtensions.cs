using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfDoc.Engine.Extensions
{
    public static class ObjectExtensions
    {
        public static T Convert<T>(this object value, T defaultValue)
        {
            if (value == null || value == DBNull.Value || 
                (typeof(T) != typeof(string) && value is string && (string)value == string.Empty))
                return defaultValue;
            else
                return (T)System.Convert.ChangeType(value, typeof(T));
        }
    }
}
