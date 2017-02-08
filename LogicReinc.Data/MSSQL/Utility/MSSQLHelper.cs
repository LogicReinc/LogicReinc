using LogicReinc.Data.SQL.Attributes;
using LogicReinc.Data.SQL.Utility;
using LogicReinc.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Data.MSSQL.Utility
{
    public class MSSQLHelper : ISQLHelper
    {
        public static MSSQLHelper Instance { get; } = new MSSQLHelper();

        public bool IsSupportedType(Type type)
        {
            return SQLHelper.IsSupportedType(type);
        }

        public string GetSqlType(Type t, ColumnAttribute attribute)
        {
            if(attribute != null)
            {
                if (attribute.IsAutoGuid)
                    return "char(32)";
                if (attribute.Type != null)
                    t = attribute.Type;
            }

            if (t == typeof(bool))
                return "bit";
            if (t == typeof(string))
                return "varchar(max)";
            if (t == typeof(char))
                return "char(1)";
            if (t == typeof(byte) || t == typeof(short) || t == typeof(int) || t == typeof(long))
                return "integer";
            if (t == typeof(double))
                return "decimal";
            if (t == typeof(DateTime))
                return "datetime";
            throw new Exception($"Type {t.Name} has not been implemented for sql");
        }
        public string ToSqlValue(object input, Type type = null)
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
