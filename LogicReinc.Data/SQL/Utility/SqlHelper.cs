using LogicReinc.Data.SQL.Attributes;
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
        string GetSqlType(Type t, ColumnAttribute attribute);
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
    }
}
