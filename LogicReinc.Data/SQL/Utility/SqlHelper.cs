using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Data.SQL.Utility
{
    public interface ISQLHelper
    {
        bool IsSupportedType(Type t);
        string GetSqlType(Type t);
        string ToSqlValue(object input, Type type = null);
    }

    public static class SQLHelper
    {
        private static List<Type> _supportedTypes = new List<Type>()
        {
            typeof(bool),
            typeof(byte),
            typeof(short), typeof(int), typeof(long),
            typeof(double),
            typeof(decimal),
            typeof(DateTime),
            typeof(string)
        };

        public static bool IsSupportedType(Type t)
        {
            return _supportedTypes.Contains(t);
        }
        
        public static string ToSQLValue(object input, Type type = null)
        {
            if (input == null)
                return "NULL";
            Type t = input.GetType();
            if (t == typeof(bool))
                return ((bool)input) ? "1" : "0";
            if (t == typeof(string))
                return $"'{input}'";
            if (t == typeof(byte) || t == typeof(char) || t == typeof(short) || t == typeof(int) || t == typeof(long))
                return $"{input}";
            if (t == typeof(double))
                return $"{input}".Replace(",", ".");
            if (t == typeof(DateTime))
                return $"'{((DateTime)input).ToString()}'";
            return "NULL";
        }
    }
}
