using LogicReinc.Data.SQL.Attributes;
using LogicReinc.Data.SQL.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Data.MySQL.Utility
{
    public class MySQLHelper : ISQLHelper
    {
        public static MySQLHelper Instance { get; } = new MySQLHelper();


        public bool IsSupportedType(Type t)
        {
            return SQLHelper.IsSupportedType(t);
        }

        public static string CreateConnectionString(string address, string user, string password, string database)
        {
            return $"Server={address};Database={database};Uid={user};Pwd={password};";
        }

        public string GetSqlType(Type t, ColumnAttribute attribute)
        {
            if (attribute != null)
            {
                if (attribute.Type != null)
                    t = attribute.Type;
                if (attribute.IsAutoGuid)
                    return "char(32)";
            }

            if (t == typeof(bool))
                return "bit";
            if (t == typeof(string))
                return "varchar(2000)";
            if (t == typeof(char))
                return "char(1)";
            if (t == typeof(byte) || t == typeof(int))//t == typeof(short) || t == typeof(int) || t == typeof(long))
                return "int";
            if (t == typeof(short))
                return "smallint";
            if (t == typeof(long))
                return "bigint";
            if (t == typeof(double) || t == typeof(decimal))
                return "decimal(20,10)";
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
